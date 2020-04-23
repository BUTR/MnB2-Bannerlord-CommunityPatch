using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CommunityPatch.Options {

  public abstract class Option : IOption {

    [NotNull]
    public OptionsStore Store { get; }

    [CanBeNull]
    public string Namespace { get; }

    [NotNull]
    public string Name { get; }

    public abstract int CompareTo(object obj);

    public abstract int CompareTo(Option obj);

    protected Option([NotNull] OptionsStore store, [CanBeNull] string ns, [NotNull] string name) {
      Store = store;
      Namespace = ns;
      Name = name;
    }

    public virtual bool IsEnum => false;

    public virtual Type EnumType => null;

    public object GetMetadata(string metadataType)
      => Store.GetMetadata(this, metadataType);

    public override int GetHashCode() {
      unchecked {
        var hashCode = Store.GetHashCode();
        hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ Name.GetHashCode();
        return hashCode;
      }
    }

    public virtual void Set(object value)
      => throw new NotImplementedException();

  }

  [PublicAPI]
  public partial class Option<TOption> : Option, IOption<TOption>, IComparable<Option<TOption>>, IEquatable<Option<TOption>> where TOption : unmanaged {

    public Option([NotNull] OptionsStore store, [CanBeNull] string ns, [NotNull] string name)
      : base(store, ns, name) {
    }

    protected TOption Value {
      get => Store.Get<TOption>(Namespace, Name);
      set => Store.Set(Namespace, Name, value);
    }

    public static bool operator ==(Option<TOption> option, TOption value)
      => option?.Value.Equals(value) ?? value.Equals(null);

    public static bool operator !=(Option<TOption> option, TOption value)
      => !(option == value);

    public static implicit operator TOption(Option<TOption> option)
      => option.Value;

    public void Set(TOption value)
      => Value = value;

    public override void Set(object value)
      => Set((TOption) value);

    public bool Equals(Option<TOption> other)
      => other != null
        && Store.Equals(other.Store)
        && Namespace == other.Namespace
        && Name == other.Name;

    public bool Equals(TOption other)
      => Value.Equals(other);

    public bool Equals(IOption<TOption> other)
      => other is Option<TOption> o && Equals(o);

    public override bool Equals(object obj)
      => !ReferenceEquals(null, obj)
        && (ReferenceEquals(this, obj)
          || obj switch {
            Option<TOption> opt => Equals(opt),
            TOption val => Equals(val),
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

  public partial class Option<TOption, TEnum> : Option<TOption>, IComparable<Option<TOption, TEnum>>, IEquatable<Option<TOption, TEnum>>, IEquatable<TEnum>
    where TOption : unmanaged
    where TEnum : Enum {

    public Option([NotNull] OptionsStore store, [CanBeNull] string ns, [NotNull] string name)
      : base(store, ns, name) {
    }

    public bool Equals(Option<TOption, TEnum> other)
      => other != null
        && Store.Equals(other.Store)
        && Namespace == other.Namespace
        && Name == other.Name;

    protected new TEnum Value {
      get {
        var v = base.Value;
        return Unsafe.As<TOption, TEnum>(ref v);
      }
      set {
        var v = value;
        var e = Unsafe.As<TEnum, TOption>(ref v);
        Store.Set(Namespace, Name, e);
      }
    }

    public virtual Type EnumType => typeof(TEnum);

    public void Set(TEnum value)
      => Value = value;

    public static bool operator ==(Option<TOption, TEnum> option, TEnum value)
      => option?.Value.Equals(value) ?? value == null;

    public static bool operator !=(Option<TOption, TEnum> option, TEnum value)
      => !(option == value);

    public static implicit operator TEnum(Option<TOption, TEnum> option)
      => option.Value;

    public bool Equals(TEnum other)
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
        var hashCode = Store.GetHashCode();
        hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ Name.GetHashCode();
        return hashCode;
      }
    }

    public override bool IsEnum => true;

  }

}