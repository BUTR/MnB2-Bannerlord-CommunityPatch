using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  public sealed partial class StringOption : Option, IComparable<StringOption>, IEquatable<StringOption>, IEquatable<string> {

    public StringOption([NotNull] OptionsStore store, [CanBeNull] string ns, [NotNull] string name) : base(store, ns, name) {
    }

    private string Value {
      get => Store.Get<string>(Namespace, Name);
      set => Store.Set(Namespace, Name, value);
    }

    public static implicit operator string(StringOption option)
      => option.Value;

    public static bool operator ==(StringOption option, string value)
      => option?.Value.Equals(value) ?? value == null;

    public static bool operator !=(StringOption option, string value)
      => !(option == value);

    public void Set(string value)
      => Value = value;

    public bool Equals(StringOption other)
      => other != null
        && Store.Equals(other.Store)
        && Namespace == other.Namespace
        && Name == other.Name;

    public bool Equals(string other)
      => Value.Equals(other);

    public override bool Equals(object obj)
      => !ReferenceEquals(null, obj)
        && (ReferenceEquals(this, obj)
          || obj switch {
            StringOption opt => Equals(opt),
            string val => Equals(val),
            _ => false
          });

    public override string ToString()
      => $"{Namespace}:{Name} = {Value}";
    public override int GetHashCode() {
      unchecked {
        var hashCode = Store.GetHashCode();
        hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ Name.GetHashCode();
        return hashCode;
      }
    }

  }

}