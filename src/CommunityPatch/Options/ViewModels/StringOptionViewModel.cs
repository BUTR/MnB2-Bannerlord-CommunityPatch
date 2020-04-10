using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public class StringOptionViewModel {

    private sealed class Data : ActionOptionData {

      public readonly TextInquiryData TextInqueryData;

      //protected string _value; // currently no callback to save, so just save on input complete

      public Data(Action prompt, Func<string, bool> validation, StringOption option, string optionNameId, bool shouldInputBeObfuscated, StringOptionViewModel model)
        : base((ManagedOptions.ManagedOptionsType) (-1), prompt)
        => TextInqueryData = new TextInquiryData(
          optionNameId.Localized(option.Name.ToSentenceCase()).ToString(),
          null,
          true,
          false,
          $"{optionNameId}Confirm".Localized(LocalizedOk).ToString(),
          null,
          s => {
            option.Set(s);
            model.Refresh();
          },
          null,
          shouldInputBeObfuscated,
          validation
        );

    }

    protected static readonly TextObject LocalizedOk = "oHaWR73d".Localized("Ok");

    private readonly ActionOptionDataVM _vm;

    private readonly Data _data;

    public StringOptionViewModel(StringOption option, OptionsVM optsVm, bool shouldInputBeObfuscated = false) {
      var optionNameId = $"{option.Store.Name}{option.Namespace}{option.Name}";

      _data = new Data(
        () => InformationManager.ShowTextInquiry(_data.TextInqueryData),
        Validation,
        option,
        optionNameId,
        shouldInputBeObfuscated,
        this
      );
      _vm = new ActionOptionDataVM(_data.OnAction, optsVm, _data,
        optionNameId.Localized(),
        new RuntimeTextObject(() => option),
        $"{optionNameId}Description".Localized(true)
      );
    }

    protected virtual bool Validation(string arg) => true;

    public void Refresh()
      => _vm.RefreshValues();

    public static implicit operator ActionOptionDataVM(StringOptionViewModel vm)
      => vm._vm;

    public static implicit operator GenericOptionDataVM(StringOptionViewModel vm)
      => vm._vm;

  }

}