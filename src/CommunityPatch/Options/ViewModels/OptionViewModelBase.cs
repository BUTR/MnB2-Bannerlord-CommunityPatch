using System;
using System.Reflection;
using TaleWorlds.Library;

namespace CommunityPatch.Options {

  public abstract class OptionViewModelBase : LabelledViewModelBase {

    protected readonly Option Option;

    protected OptionViewModelBase(Option option) {
      Option = option;
      Name = $"{option.Store.Name}{option.Namespace}{option.Name}".Localized().ToString();
      Description = (
        (option.GetMetadata("DescriptionId") as string)?.Localized(false)
        ?? $"{option.Store.Name}{option.Namespace}{option.Name}Description".Localized(true)
      ).ToString();
    }

    public abstract void Save();

  }

  public abstract class OptionViewModelBase<T> : OptionViewModelBase where T : IEquatable<T> {

    protected OptionViewModelBase(Option option) : base(option)
      => PropertyChanged += (sender, args) => {
        if (args.PropertyName == nameof(Value))
          OnPropertyChanged(nameof(StringValue));
      };

    private T _val;

    [DataSourceProperty]
    public T Value {
      get => _val;
      set => UpdateWithNotify(ref _val, value);
    }

    [DataSourceProperty]
    public string StringValue {
      get {
        if (Option.IsEnum)
          return Enum.GetName(Option.EnumType, Value) ?? "";
        else
          return Value?.ToString() ?? "";
      }
    }

    public override void Save() {
      switch (Option) {
        case IOption<T> to:
          to.Set(Value);
          break;
        case IOption o:
          o.Set(Value);
          break;
        default: throw new NotImplementedException();
      }
    }

  }

}