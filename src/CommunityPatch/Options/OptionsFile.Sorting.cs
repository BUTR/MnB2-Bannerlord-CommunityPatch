using System;

namespace CommunityPatch {

  public partial class OptionsFile {

    public override int CompareTo(OptionsStore other)
      => throw new NotImplementedException();

    public int CompareTo(OptionsFile other) {
      if (ReferenceEquals(this, other))
        return 0;
      if (ReferenceEquals(null, other))
        return 1;

      return string.Compare(_path, other._path, StringComparison.Ordinal);
    }

    public static bool operator <(OptionsFile left, OptionsFile right)
      => left.CompareTo(right) < 0;

    public static bool operator >(OptionsFile left, OptionsFile right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(OptionsFile left, OptionsFile right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(OptionsFile left, OptionsFile right)
      => left.CompareTo(right) >= 0;

  }

}