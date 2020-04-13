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

  public class OptionsFile {

    private readonly string _path;

    private readonly DocumentSyntax _toml;

    [PublicAPI]
    public OptionsFile(string fileName) {
      _path = Path.Combine(PathHelpers.GetBinSubDir(), fileName);
      if (!File.Exists(_path)) {
        _toml = new DocumentSyntax();
        return;
      }

      var bytes = File.ReadAllBytes(_path);
      _toml = Toml.Parse(bytes, _path);
    }

    [PublicAPI]
    public void Save() {
      using var sw = new StreamWriter(_path, false, Encoding.UTF8, 65536) {NewLine = "\n"};
      foreach (var kv in _toml.KeyValues)
        sw.WriteLine(kv.ToString().Trim('\n'));
      sw.WriteLine();
    }

    [PublicAPI]
    [CanBeNull]
    public KeyValueSyntax GetConfig([NotNull] string key)
      => _toml.KeyValues
        .FirstOrDefault(kv => kv.Key.Key.ToString().Trim() == key);

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
    public void DeleteConfig([NotNull] string key)
      => _toml.KeyValues.RemoveChildren(GetConfig(key));

    public void Set<T>(string key, T value) {
      var cfg = GetOrCreateConfig(key);
      long iVal;
      switch (value) {
        // @formatter:off
        case bool v: cfg.Value = new BooleanValueSyntax(v); break;
        case string v: cfg.Value = new StringValueSyntax(v); break;
        case float v: cfg.Value = new FloatValueSyntax(v); break;
        case double v: cfg.Value = new FloatValueSyntax(v); break;
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
      cfg.Value = new IntegerValueSyntax(iVal);
    }

    public T Get<T>(string key) {
      var cfg = GetConfig(key);
      if (cfg == null)
        return default;

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