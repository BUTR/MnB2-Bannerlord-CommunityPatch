using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    internal static readonly OptionsFile Options = new CommunityPatchOptionsFile();

    internal static readonly Option<bool> DisableIntroVideo
      = Options.GetOption<bool>(nameof(DisableIntroVideo));

    internal static readonly Option<bool> RecordFirstChanceExceptions
      = Options.GetOption<bool>(nameof(RecordFirstChanceExceptions));

    internal static readonly Option<bool> DontGroupThirdPartyMenuOptions
      = Options.GetOption<bool>(nameof(DontGroupThirdPartyMenuOptions));

    internal void ShowOptions() {
      // ReSharper disable once UseObjectOrCollectionInitializer
      var elements = new List<InquiryElement>();

      elements.Add(new InquiryElement(
        nameof(DisableIntroVideo),
        DisableIntroVideo ? "Enable Intro Videos" : "Disable Intro Videos",
        null
      ));

      elements.Add(new InquiryElement(
        nameof(RecordFirstChanceExceptions),
        RecordFirstChanceExceptions ? "Ignore First Chance Exceptions" : "Record First Chance Exceptions",
        null
      ));

      elements.Add(new InquiryElement(
        nameof(DontGroupThirdPartyMenuOptions),
        DontGroupThirdPartyMenuOptions ? "Group 3rd Party Menu Options" : "Don't Group 3rd Party Menu Options",
        null));

      elements.Add(new InquiryElement(
        nameof(Diagnostics.GenerateReport),
        "Copy Diagnostics to Clipboard",
        null
      ));
#if DEBUG
      elements.Add(new InquiryElement(
        "IntentionallyUnhandled",
        "Throw Unhandled Exception",
        null
      ));
      elements.Add(new InquiryElement(
        nameof(CauseStackOverflow),
        "Cause Stack Overflow",
        null
      ));
#endif
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        new TextObject("{=CommunityPatchOptions}Community Patch Options").ToString(),
        new TextObject("{=PickAnOption}Pick an option:").ToString(),
        elements,
        true,
        true,
        new TextObject("{=BAaS5Dkc}Apply").ToString(),
        null,
        HandleOptionChoice,
        null
      ));
    }

    private void HandleOptionChoice(List<InquiryElement> list) {
      var selected = (string) list[0].Identifier;
      switch (selected) {
        case nameof(DisableIntroVideo):
          DisableIntroVideo.Set(!DisableIntroVideo);
          ShowMessage($"Intro Videos: {(DisableIntroVideo ? "Disabled" : "Enabled")}.");
          Options.Save();
          break;

        case nameof(RecordFirstChanceExceptions):
          RecordFirstChanceExceptions.Set(!RecordFirstChanceExceptions);
          ShowMessage($"Record FCEs: {(RecordFirstChanceExceptions ? "Enabled" : "Disabled")}.");
          Options.Save();
          break;

        case nameof(Diagnostics.GenerateReport):
          Diagnostics.GenerateReport();
          break;

        case nameof(DontGroupThirdPartyMenuOptions):
          DontGroupThirdPartyMenuOptions.Set(!DontGroupThirdPartyMenuOptions);
          ShowMessage($"3rd Party Menu Options: {(DontGroupThirdPartyMenuOptions ? "Loose" : "Grouped")}.");
          Options.Save();
          break;

#if DEBUG
        case nameof(CauseStackOverflow):
          CauseStackOverflow();
          break;
#endif

        default: throw new NotImplementedException(selected);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    // ReSharper disable once FunctionRecursiveOnAllPaths
    private void CauseStackOverflow()
      => CauseStackOverflow();

  }

}