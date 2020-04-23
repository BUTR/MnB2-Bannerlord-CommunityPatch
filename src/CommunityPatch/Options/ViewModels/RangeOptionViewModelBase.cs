using System;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.Library;

namespace CommunityPatch.Options {

  public abstract class RangeOptionViewModelBase<T> : OptionViewModelBase<T> where T : IEquatable<T>, IComparable<T> {

    private static readonly object FallbackMaxValue = typeof(T)
      .GetField("MaxValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
      ?.GetRawConstantValue();

    protected RangeOptionViewModelBase(Option option) : base(option) {
      try {
        Minimum = (T) option.GetMetadata("Min");
      }
      catch {
        Minimum = default;
      }

      try {
        Maximum = (T) option.GetMetadata("Max");
      }
      catch {
        Maximum = FallbackMaxValue is null
          ? throw new NotSupportedException("Range options must have a maximum value.")
          : (T) FallbackMaxValue;
      }
    }

    protected RangeOptionViewModelBase(Option option, T min, T max) : base(option) {
      Minimum = min;
      Maximum = max;
    }

    private T _min;

    [DataSourceProperty]
    public T Minimum {
      get => _min;
      set => UpdateWithNotify(ref _min, value);
    }

    private T _max;

    [DataSourceProperty]
    public T Maximum {
      get => _max;
      set => UpdateWithNotify(ref _max, value);
    }

  }

}