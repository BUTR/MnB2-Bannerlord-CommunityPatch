using System;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public abstract class OptionsStore : IEquatable<OptionsStore>, IComparable<OptionsStore> {

    public abstract void Save();

    public abstract void Set<T>(string key, T value);

    public abstract void Set<T>(string ns, string key, T value);

    public abstract T Get<T>(string key);

    public abstract T Get<T>(string ns, string key);

    public OptionNamespace GetNamespace(string ns)
      => new OptionNamespace(this, ns);

    public Option<TOption> GetOption<TOption>([NotNull] string key) where TOption : unmanaged, IEquatable<TOption>
      => GetOption<TOption>(null, key);

    public Option<TOption> GetOption<TOption>([CanBeNull] string ns, [NotNull] string key) where TOption : unmanaged, IEquatable<TOption>
      => new Option<TOption>(this, ns, key);

    public abstract bool Equals(OptionsStore other);

    public abstract int CompareTo(OptionsStore other);
    
    public static bool operator <(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) < 0;

    public static bool operator >(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(OptionsStore left, OptionsStore right)
      =>left.CompareTo(right) >= 0;
  }

}