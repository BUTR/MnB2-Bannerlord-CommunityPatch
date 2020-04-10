using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public abstract class EnumOptionViewModelBase : StringOptionDataVM {

    protected EnumOptionViewModelBase(string optionNameId, OptionsVM optsVm, ISelectionOptionData data)
      : base(optsVm, data,
        optionNameId.Localized(),
        $"{optionNameId}Description".Localized(true)) {
    }

  }

}