using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  // NOTE: precision lost, maybe use StringOptionViewModel instead?
  public class IntOptionViewModel : NumericOptionViewModel {

    private sealed class Data : INumericOptionData {

      private readonly Option<int> _option;

      private int _value;

      public Data(Option<int> option) {
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
        => _value = (int) value;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

      public float GetMinValue()
        => _option.GetMetadata("Min") is int f ? f : int.MinValue;

      public float GetMaxValue()
        => _option.GetMetadata("Max") is int f ? f : int.MaxValue;

      public bool GetIsDiscrete()
        => !(_option.GetMetadata("IsDiscrete") is bool b) || b;

    }

    private static Data _data;

    public IntOptionViewModel(Option<int> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}