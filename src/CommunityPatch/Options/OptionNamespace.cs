using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public partial class OptionNamespace : IEquatable<OptionNamespace>, IComparable<OptionNamespace> {

    private readonly OptionsStore _options;

    public readonly string Namespace;

    public OptionNamespace([NotNull] OptionsStore options, [CanBeNull] string ns) {
      _options = options;
      Namespace = ns;
    }

    public Option<TOption> GetOption<TOption>([NotNull] string name) where TOption : unmanaged, IEquatable<TOption>
      => new Option<TOption>(_options, Namespace, name);

    public bool Equals(OptionNamespace other) {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return Equals(_options, other._options)
        && Namespace == other.Namespace;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is OptionNamespace optNs)
        return Equals(optNs);

      return false;
    }

    public override int GetHashCode() {
      unchecked {
        return ((_options != null ? _options.GetHashCode() : 0) * 397)
          ^ (Namespace != null ? Namespace.GetHashCode() : 0);
      }
    }

    public static bool operator ==(OptionNamespace left, OptionNamespace right)
      => Equals(left, right);

    public static bool operator !=(OptionNamespace left, OptionNamespace right)
      => !Equals(left, right);

  }

}