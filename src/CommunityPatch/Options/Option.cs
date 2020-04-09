using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public partial class Option<TOption> : IComparable<Option<TOption>>, IEquatable<Option<TOption>>, IEquatable<TOption> where TOption : unmanaged {

    [NotNull]
    private readonly OptionsStore _options;

    [CanBeNull]
    public readonly string Namespace;

    [NotNull]
    public readonly string Name;

    public string QualifiedName
      => !string.IsNullOrEmpty(Namespace) ? $"[{Namespace}] {Name}" : Name;

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

    public bool Equals(Option<TOption> other)
      => other != null
        && _options.Equals(other._options)
        && Namespace == other.Namespace
        && Name == other.Name;

    public bool Equals(TOption other)
      => Value.Equals(other);

    public override bool Equals(object obj)
      => !ReferenceEquals(null, obj)
        && (ReferenceEquals(this, obj)
          || obj switch {
            Option<TOption> opt => Equals(opt),
            TOption val => Equals(val),
            _ => false
          });

    public override int GetHashCode() {
      unchecked {
        var hashCode = _options.GetHashCode();
        hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ Name.GetHashCode();
        return hashCode;
      }
    }

    public override string ToString()
      => $"{QualifiedName} = {Value}";

  }

}