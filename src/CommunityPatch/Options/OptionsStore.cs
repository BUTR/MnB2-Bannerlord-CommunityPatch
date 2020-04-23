using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Options {

  [PublicAPI]
  public abstract class OptionsStore : IEquatable<OptionsStore>, IComparable<OptionsStore> {

    private static readonly ISet<OptionsStore> RegisteredForGui
      = new HashSet<OptionsStore>();

    private static readonly ICollection<OptionsStore> RegisteredForGuiSequence
      = new LinkedList<OptionsStore>();

    protected OptionsStore(bool initDeclaredOptionMembers) {
      if (!initDeclaredOptionMembers)
        return;

      foreach (var mi in GetOptionMembers()) {
        switch (mi) {
          // @formatter:off
          case FieldInfo fi: fi.SetValue(this, CreateMemberOption(fi.FieldType, mi)); break;
          case PropertyInfo pi: pi.SetValue(this, CreateMemberOption(pi.PropertyType, mi)); break;
          // @formatter:on
        }
      }
    }

    private Option CreateMemberOption(Type t, MemberInfo mi) {
      var ctor = GetOptionConstructor(t);
      return ctor != null
        ? (Option) ctor.Invoke(null, new object[] {this, null, mi.Name})
        : null;
    }

    private static ConstructorInfo GetOptionConstructor(Type t)
      => t.GetConstructor(new[] {typeof(OptionsStore), typeof(string), typeof(string)});

    public static bool RegisterForGui(OptionsStore options) {
      if (!RegisteredForGui.Add(options))
        return false;

      RegisteredForGuiSequence.Add(options);
      return true;
    }

    internal static IEnumerable<OptionsStore> GetRegisteredForGui() {
      foreach (var options in RegisteredForGuiSequence)
        yield return options;
    }

    public virtual string Name
      => throw new NotImplementedException("The Name member of your options store implementation is missing.");

    public abstract bool IsEmpty { get; }

    [PublicAPI]
    public object GetMetadata(Option option, string metadataType)
      => GetOptionMetadata(option.Namespace, option.Name, metadataType);

    [PublicAPI]
    public virtual object GetOptionMetadata(string ns, string key, string metadataType) => null;

    [PublicAPI]
    public virtual IEnumerable<Option> GetKnownOptions() {
      foreach (var mi in GetOptionMembers()) {
        switch (mi) {
          // @formatter:off
          case FieldInfo fi: yield return (Option) fi.GetValue(this); break;
          case PropertyInfo pi: yield return (Option) pi.GetValue(this); break;
          // @formatter:on
        }
      }
    }

    private IEnumerable<MemberInfo> GetOptionMembers() {
      foreach (var mi in GetType().GetMembers(Public | Instance | FlattenHierarchy)) {
        switch (mi) {
          case FieldInfo fi: {
            if (typeof(Option).IsAssignableFrom(fi.FieldType))
              yield return fi;

            break;
          }
          case PropertyInfo pi: {
            if (typeof(Option).IsAssignableFrom(pi.PropertyType))
              yield return pi;

            break;
          }
        }
      }
    }

    [PublicAPI]
    public abstract void Save();

    [PublicAPI]
    public abstract void Set<T>(string key, T value);

    [PublicAPI]
    public abstract void Set<T>(string ns, string key, T value);

    [PublicAPI]
    public abstract T Get<T>(string key);

    [PublicAPI]
    public abstract T Get<T>(string ns, string key);

    [PublicAPI]
    public OptionNamespace GetNamespace(string ns)
      => new OptionNamespace(this, ns);

    [PublicAPI]
    public Option<TOption> GetOption<TOption>([NotNull] string key) where TOption : unmanaged, IEquatable<TOption>
      => GetOption<TOption>(null, key);

    [PublicAPI]
    public Option<TOption> GetOption<TOption>([CanBeNull] string ns, [NotNull] string key) where TOption : unmanaged, IEquatable<TOption>
      => new Option<TOption>(this, ns, key);

    [PublicAPI]
    public StringOption GetStringOption([NotNull] string key)
      => GetStringOption(null, key);

    [PublicAPI]
    public StringOption GetStringOption([CanBeNull] string ns, [NotNull] string key)
      => new StringOption(this, ns, key);

    public abstract bool Equals(OptionsStore other);

    public abstract int CompareTo(OptionsStore other);

    public static bool operator <(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) < 0;

    public static bool operator >(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) > 0;

    public static bool operator <=(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) <= 0;

    public static bool operator >=(OptionsStore left, OptionsStore right)
      => left.CompareTo(right) >= 0;

  }

}