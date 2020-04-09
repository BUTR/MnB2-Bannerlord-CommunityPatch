using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Tomlyn;
using Tomlyn.Syntax;
using Path = System.IO.Path;

namespace CommunityPatch {

  public sealed partial class OptionsFile : OptionsStore, IEquatable<OptionsFile>, IComparable<OptionsFile> {

    public bool Equals(OptionsFile other)
      => _path == other._path;

    public override bool Equals(OptionsStore other)
      => other is OptionsFile file && Equals(file);

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is OptionsFile file)
        return Equals(file);
      if (obj is OptionsStore store)
        return Equals(store);

      return false;
    }

    public override int GetHashCode()
      => (_path != null ? _path.GetHashCode() : 0);

    public static bool operator ==(OptionsFile left, OptionsFile right)
      => Equals(left, right);

    public static bool operator !=(OptionsFile left, OptionsFile right)
      => !Equals(left, right);

    private readonly string _path;

    private readonly DocumentSyntax _toml;

    [PublicAPI]
    public OptionsFile(string fileName) {
      _path = Path.Combine(PathHelpers.GetConfigsDir(), fileName);
      if (!File.Exists(_path)) {
        _toml = new DocumentSyntax();
        return;
      }

      var bytes = File.ReadAllBytes(_path);
      _toml = Toml.Parse(bytes, _path);
    }

    [PublicAPI]
    public override void Save() {
      using var sw = new StreamWriter(_path, false, Encoding.UTF8, 65536) {NewLine = "\n"};
      foreach (var kv in _toml.KeyValues)
        sw.WriteLine(kv.ToString().Trim('\n'));

      foreach (var table in _toml.Tables) {
        sw.WriteLine();
        sw.Write(table.OpenBracket.Text);
        sw.Write(table.Name.Key.ToString());
        sw.WriteLine(table.CloseBracket.Text);

        foreach (var kv in table.Items)
          sw.WriteLine(kv.ToString().Trim('\n'));
      }

      sw.WriteLine();
    }

    [PublicAPI]
    [CanBeNull]
    public KeyValueSyntax GetConfig([NotNull] string key)
      => _toml.KeyValues
        .FirstOrDefault(kv => kv.Key.Key.ToString().Trim() == key);

    [PublicAPI]
    [CanBeNull]
    public KeyValueSyntax GetConfig([NotNull] TableSyntaxBase table, [NotNull] string key)
      => table.Items
        .FirstOrDefault(kv => kv.Key.Key.ToString().Trim() == key);

    [PublicAPI]
    [CanBeNull]
    public TableSyntaxBase GetTable([NotNull] string key)
      => _toml.Tables
        .FirstOrDefault(t => t.Name.Key.ToString().Trim() == key);

    [PublicAPI]
    [NotNull]
    public KeyValueSyntax GetOrCreateConfig([NotNull] string key) {
      var kvs = GetConfig(key);
      if (kvs != null)
        return kvs;

      kvs = new KeyValueSyntax(key, new BareKeySyntax());
      _toml.KeyValues.Add(kvs);

      return kvs;
    }

    [PublicAPI]
    [NotNull]
    public KeyValueSyntax GetOrCreateConfig([NotNull] TableSyntaxBase t, [NotNull] string key) {
      var kvs = GetConfig(t, key);
      if (kvs != null)
        return kvs;

      kvs = new KeyValueSyntax(key, new BareKeySyntax());
      _toml.KeyValues.Add(kvs);

      return kvs;
    }

    [PublicAPI]
    [NotNull]
    public TableSyntaxBase GetOrCreateNamespace([NotNull] string key) {
      var t = GetTable(key);
      if (t != null)
        return t;

      t = new TableSyntax(key);
      _toml.Tables.Add(t);

      return t;
    }

    [PublicAPI]
    public void DeleteConfig([NotNull] string key)
      => _toml.KeyValues.RemoveChildren(GetConfig(key));

    public override void Set<T>(string key, T value) {
      var keyValueSyntax = GetOrCreateConfig(key);
      Set(keyValueSyntax, value);
    }

    public override void Set<T>(string ns, string key, T value) {
      if (ns == null) {
        Set(key, value);
        return;
      }

      var t = GetOrCreateNamespace(ns);
      var kv = GetOrCreateConfig(t, key);
      Set(kv, value);
    }

    private static void Set<T>(KeyValueSyntax entry, T value) {
      long iVal;
      switch (value) {
        // @formatter:off
        case bool v: entry.Value = new BooleanValueSyntax(v); break;
        case string v: entry.Value = new StringValueSyntax(v); break;
        case float v: entry.Value = new FloatValueSyntax(v); break;
        case double v: entry.Value = new FloatValueSyntax(v); break;
        case sbyte v: iVal = v; goto setIVal;
        case short v: iVal = v; goto setIVal;
        case int v: iVal = v; goto setIVal;
        case long v: iVal = v; goto setIVal;
        case byte v: iVal = v; goto setIVal;
        case ushort v: iVal = v; goto setIVal;
        case uint v: iVal = v; goto setIVal;
        case ulong v: iVal = (long)v; goto setIVal;
        // @formatter:on
        default:
          throw new NotImplementedException(typeof(T).FullName);
      }

      return;

      setIVal:
      entry.Value = new IntegerValueSyntax(iVal);
    }

    public override T Get<T>(string key) {
      var cfg = GetConfig(key);
      return cfg == null ? default : Get<T>(cfg);
    }

    public override T Get<T>(string ns, string key) {
      if (ns == null)
        return Get<T>(key);

      var t = GetTable(ns);
      if (t == null) return default;

      var kv = GetConfig(t, key);
      return kv == null ? default : Get<T>(kv);
    }

    private static T Get<T>(KeyValueSyntax cfg) {
      var t = typeof(T);

      if (t == typeof(string)) {
        string v;
        if (cfg.Value is StringValueSyntax svs)
          v = svs.Value;
        else
          throw new NotImplementedException(cfg.Value.GetType().FullName);

        return Unsafe.As<string, T>(ref v);
      }

      if (t == typeof(bool)) {
        var v = ReadBoolean(cfg);
        return Unsafe.As<bool, T>(ref v);
      }

      if (t == typeof(sbyte) || t == typeof(short) || t == typeof(int) || t == typeof(long)) {
        var v = ReadInteger(cfg);
        return Unsafe.As<long, T>(ref v);
      }

      if (t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong)) {
        var v = unchecked((ulong) ReadInteger(cfg));
        return Unsafe.As<ulong, T>(ref v);
      }

      if (t == typeof(float)) {
        var v = (float) ReadDouble(cfg);
        return Unsafe.As<float, T>(ref v);
      }

      if (t == typeof(double)) {
        var v = ReadDouble(cfg);
        return Unsafe.As<double, T>(ref v);
      }

      throw new NotImplementedException(t.FullName);
    }

    private static bool ReadBoolean(KeyValueSyntax cfg) {
      var tk = ((BooleanValueSyntax) cfg.Value).Token.TokenKind;
      return tk switch {
        TokenKind.True => true,
        TokenKind.False => false,
        _ => throw new NotImplementedException(tk.ToString())
      };
    }

    private static long ReadInteger(KeyValueSyntax cfg)
      => long.Parse(((IntegerValueSyntax) cfg.Value).Token.Text);

    private static double ReadDouble(KeyValueSyntax cfg) {
      var st = ((FloatValueSyntax) cfg.Value).Token;

      return st.TokenKind switch {
        TokenKind.Float => double.Parse(st.Text),
        TokenKind.Integer => long.Parse(st.Text),
        TokenKind.Nan => double.NaN,
        TokenKind.PositiveNan => double.NaN,
        TokenKind.NegativeNan => double.NaN,
        TokenKind.Infinite => double.PositiveInfinity,
        TokenKind.PositiveInfinite => double.PositiveInfinity,
        TokenKind.NegativeInfinite => double.NegativeInfinity,
        _ => throw new NotImplementedException(st.TokenKind.ToString())
      };
    }

  }

}