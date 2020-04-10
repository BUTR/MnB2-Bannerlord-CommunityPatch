using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public class ByteOptionViewModel : NumericOptionViewModel {

    private sealed class Data : INumericOptionData {

      private readonly Option<byte> _option;

      private byte _value;

      public Data(Option<byte> option) {
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
        => _value = (byte) value;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

      public float GetMinValue()
        => _option.GetMetadata("Min") is byte f ? f : byte.MinValue;

      public float GetMaxValue()
        => _option.GetMetadata("Max") is byte f ? f : byte.MaxValue;

      public bool GetIsDiscrete()
        => !(_option.GetMetadata("IsDiscrete") is bool b) || b;

    }

    private static Data _data;

    public ByteOptionViewModel(Option<byte> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}