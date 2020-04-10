using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  // NOTE: precision lost, maybe use StringOptionViewModel instead?
  public class ULongOptionViewModel : NumericOptionViewModel {

    private sealed class Data : INumericOptionData {

      private readonly Option<ulong> _option;

      private ulong _value;

      public Data(Option<ulong> option) {
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
        => _value = (ulong) value;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

      public float GetMinValue()
        => _option.GetMetadata("Min") is ulong f ? f : ulong.MinValue;

      public float GetMaxValue()
        => _option.GetMetadata("Max") is ulong f ? f : ulong.MaxValue;

      public bool GetIsDiscrete()
        => !(_option.GetMetadata("IsDiscrete") is bool b) || b;

    }

    private static Data _data;

    public ULongOptionViewModel(Option<ulong> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}