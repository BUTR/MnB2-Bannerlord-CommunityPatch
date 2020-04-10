using System;
using System.Collections.Generic;

namespace CommunityPatch {

  public sealed partial class StringOption {

    public override int CompareTo(object obj) {
      if (!(obj is Option opt))
        return GetHashCode() - obj.GetHashCode();

      if (opt is StringOption genOpt)
        return CompareTo(genOpt);

      return opt.CompareTo(opt);
    }

    public int CompareTo(StringOption other) {
      if (ReferenceEquals(this, other))
        return 0;
      if (ReferenceEquals(null, other))
        return 1;

      var optionsComparison = Store.CompareTo(other.Store);
      if (optionsComparison != 0)
        return optionsComparison;

      var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
      if (namespaceComparison != 0)
        return namespaceComparison;

      return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public override int CompareTo(Option other) {
      if (ReferenceEquals(this, other))
        return 0;
      if (ReferenceEquals(null, other))
        return 1;

      var optionsComparison = Store.CompareTo(other.Store);
      if (optionsComparison != 0)
        return optionsComparison;

      var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
      if (namespaceComparison != 0)
        return namespaceComparison;

      var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
      if (nameComparison != 0)
        return nameComparison;

      return Comparer<Type>.Default.Compare(typeof(StringOption), other.GetType());
    }

    public static bool operator <(StringOption left, StringOption right)
      => left.CompareTo(right) < 0;

    public static bool operator >(StringOption left, StringOption right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(StringOption left, StringOption right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(StringOption left, StringOption right)
      => left.CompareTo(right) >= 0;

  }

}