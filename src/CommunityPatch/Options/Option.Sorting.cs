using System;
using System.Collections.Generic;

namespace CommunityPatch.Options {

  public partial class Option<TOption> {

    public override int CompareTo(object obj) {
      if (!(obj is Option opt))
        return GetHashCode() - obj.GetHashCode();

      if (opt is Option<TOption> genOpt)
        return CompareTo(genOpt);

      return opt.CompareTo(opt);
    }

    public int CompareTo(Option<TOption> other) {
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

      return Comparer<Type>.Default.Compare(typeof(Option<TOption>), other.GetType());
    }

    public static bool operator <(Option<TOption> left, Option<TOption> right)
      => left.CompareTo(right) < 0;

    public static bool operator >(Option<TOption> left, Option<TOption> right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(Option<TOption> left, Option<TOption> right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(Option<TOption> left, Option<TOption> right)
      => left.CompareTo(right) >= 0;

    public int CompareTo(IOption<TOption> other) {
      if (other is Option<TOption> o)
        return CompareTo(o);

      return GetHashCode() - other.GetHashCode();
    }

  }

  public partial class Option<TOption, TEnum> {

    public int CompareTo(Option<TOption, TEnum> other)
      => ((Option<TOption>) this).CompareTo(other);

  }

}