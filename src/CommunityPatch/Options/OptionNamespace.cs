using System;
using System.Reflection;
using InlineIL;
using JetBrains.Annotations;
using static System.Reflection.BindingFlags;
using static InlineIL.IL;
using static InlineIL.IL.Emit;

namespace CommunityPatch.Options {

  [PublicAPI]
  public partial class OptionNamespace : IEquatable<OptionNamespace>, IComparable<OptionNamespace> {

    private readonly OptionsStore _store;

    public readonly string Namespace;

    public OptionNamespace([NotNull] OptionsStore store, [CanBeNull] string ns) {
      _store = store;
      Namespace = ns;
    }

    public IOption<TOption> GetOption<TOption>([NotNull] string name) {
      var type = typeof(TOption);
      if (type == typeof(string))
        return (IOption<TOption>) (object) new StringOption(_store, Namespace, name);

      if (type.IsEnum) {
        try {
          Push(this);
          Push(_store);
          Push(Namespace);
          Push(name);
          Call(new MethodRef(typeof(OptionNamespace), nameof(GetEnumOption), 1, typeof(TOption)));
          return Return<IOption<TOption>>();
        }
        catch (ArgumentException ex) {
          // oh well
          throw new NotImplementedException("Unsupported enum option type.", ex);
        }
      }

      try {
        Push(this);
        Push(_store);
        Push(Namespace);
        Push(name);
        Call(new MethodRef(typeof(OptionNamespace), nameof(GetTypedOption), 1, typeof(TOption)));
        return Return<IOption<TOption>>();
      }
      catch (ArgumentException ex) {
        // oh well
        throw new NotImplementedException("Unsupported option type.", ex);
      }
    }

    public Option<TOption> GetTypedOption<TOption>([NotNull] string name) where TOption : unmanaged, IEquatable<TOption>
      => new Option<TOption>(_store, Namespace, name);

    public Option<TOption> GetEnumOption<TOption>([NotNull] string name, Type tEnum) where TOption : unmanaged, IEquatable<TOption>
      => (Option<TOption>) Activator.CreateInstance(typeof(Option<,>).MakeGenericType(typeof(TOption), tEnum), _store, Namespace, name);

    public Option<TOption> GetEnumOption<TOption, TEnum>([NotNull] string name) where TOption : unmanaged, IEquatable<TOption> where TEnum : Enum
      => new Option<TOption, TEnum>(_store, Namespace, name);

    public StringOption GetStringOption([NotNull] string name)
      => new StringOption(_store, Namespace, name);

    public bool Equals(OptionNamespace other) {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return Equals(_store, other._store)
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
        return ((_store != null ? _store.GetHashCode() : 0) * 397)
          ^ (Namespace != null ? Namespace.GetHashCode() : 0);
      }
    }

    public static bool operator ==(OptionNamespace left, OptionNamespace right)
      => Equals(left, right);

    public static bool operator !=(OptionNamespace left, OptionNamespace right)
      => !Equals(left, right);

  }

}