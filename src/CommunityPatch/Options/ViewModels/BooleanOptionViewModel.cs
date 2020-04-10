using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public class BooleanOptionViewModel : BooleanOptionViewModelBase {

    private sealed class Data : IBooleanOptionData {

      private readonly Option<bool> _option;

      private bool _value;

      public Data(Option<bool> option) {
        _option = option;
        _value = _option;
      }

      public float GetDefaultValue()
        => 0;

      public void Commit()
        => _option.Set(_value);

      public float GetValue()
        => _value ? 1 : 0;

      public void SetValue(float value)
        => _value = value < float.Epsilon;

      public object GetOptionType() => (ManagedOptions.ManagedOptionsType) (-1);

      public bool IsNative() => false;

    }

    private static Data _data;

    public BooleanOptionViewModel(Option<bool> option, OptionsVM optsVm)
      : base($"{option.Store.Name}{option.Namespace}{option.Name}", optsVm, _data = new Data(option)) {
    }

  }

}