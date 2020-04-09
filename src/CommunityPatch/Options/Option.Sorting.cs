using System;
using System.Collections.Generic;

namespace CommunityPatch {

  public partial class Option<TOption> {

    public int CompareTo(Option<TOption> other) {
      if (ReferenceEquals(this, other))
        return 0;
      if (ReferenceEquals(null, other))
        return 1;

      var optionsComparison = _options.CompareTo(other._options);
      if (optionsComparison != 0)
        return optionsComparison;

      var namespaceComparison = string.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
      if (namespaceComparison != 0)
        return namespaceComparison;

      return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public static bool operator <(Option<TOption> left, Option<TOption> right)
      => Comparer<Option<TOption>>.Default.Compare(left, right) < 0;

    public static bool operator >(Option<TOption> left, Option<TOption> right)
      => Comparer<Option<TOption>>.Default.Compare(left, right) > 0;

    public static bool operator <=(Option<TOption> left, Option<TOption> right)
      => Comparer<Option<TOption>>.Default.Compare(left, right) <= 0;

    public static bool operator >=(Option<TOption> left, Option<TOption> right)
      => Comparer<Option<TOption>>.Default.Compare(left, right) >= 0;

  }

}