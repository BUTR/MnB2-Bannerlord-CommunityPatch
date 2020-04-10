using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  // NOTE: precision lost, maybe use StringOptionViewModel instead?
  public class DoubleOptionViewModel : NumericOptionViewModel {

    private sealed class Data : INumericOptionData {

      private readonly Option<double> _option;

      private double _value;

      public Data(Option<double> option) {
        _option = option;
        _value = _option;
      }

      public float GetDefaultValue()
        => 0;

      public void Commit()
        => _option.Set(_value);

      public float GetValue()
        => (float) _value;

      public void SetValue(float value)
        => _value = value;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

      public float GetMinValue()
        => (float) (_option.GetMetadata("Min") is double f ? f : double.MinValue);

      public float GetMaxValue()
        => (float) (_option.GetMetadata("Max") is double f ? f : double.MaxValue);

      public bool GetIsDiscrete()
        => _option.GetMetadata("IsDiscrete") is bool b && b;

    }

    private static Data _data;

    public DoubleOptionViewModel(Option<double> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}