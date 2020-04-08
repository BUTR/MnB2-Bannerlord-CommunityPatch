using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public class Option<TOption> where TOption : unmanaged, IEquatable<TOption> {

    [NotNull]
    private readonly OptionsStore _options;

    [CanBeNull]
    public readonly string Namespace;

    [NotNull]
    public readonly string Name;

    public string QualifiedName
      => !string.IsNullOrEmpty(Namespace) ? $"{Namespace}.{Name}" : Name;

    public Option([NotNull] OptionsStore options, [CanBeNull] string ns, [NotNull] string name) {
      _options = options;
      Namespace = ns;
      Name = name;
    }

    private TOption Value {
      get => _options.Get<TOption>(Namespace, Name);
      set => _options.Set(Namespace, Name, value);
    }

    public static bool operator ==(Option<TOption> option, TOption value)
      => option?.Value.Equals(value) ?? value.Equals(null);

    public static bool operator !=(Option<TOption> option, TOption value)
      => !(option == value);

    public static implicit operator TOption(Option<TOption> option)
      => option.Value;

    public void Set(TOption value)
      => Value = value;

  }

}