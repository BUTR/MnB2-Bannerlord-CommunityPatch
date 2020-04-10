using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public class SByteOptionViewModel : NumericOptionViewModel {

    private sealed class Data : INumericOptionData {

      private readonly Option<sbyte> _option;

      private sbyte _value;

      public Data(Option<sbyte> option) {
        _option = option;
        _value = _option;
      }

      public float GetDefaultValue()
        => 0;

      public void Commit()
        => _option.Set(_value);

      public float GetValue()
        => _value;

      public void SetValue(float value)
        => _value = (sbyte) value;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

      public float GetMinValue()
        => _option.GetMetadata("Min") is sbyte f ? f : sbyte.MinValue;

      public float GetMaxValue()
        => _option.GetMetadata("Max") is sbyte f ? f : sbyte.MaxValue;

      public bool GetIsDiscrete()
        => !(_option.GetMetadata("IsDiscrete") is bool b) || b;

    }

    private static Data _data;

    public SByteOptionViewModel(Option<sbyte> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}