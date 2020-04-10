using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public abstract class BooleanOptionViewModelBase : BooleanOptionDataVM {

    protected BooleanOptionViewModelBase(string optionNameId, OptionsVM optsVm, IBooleanOptionData data)
      : base(optsVm, data,
        optionNameId.Localized(),
        $"{optionNameId}Description".Localized(true)) {
    }

  }

}