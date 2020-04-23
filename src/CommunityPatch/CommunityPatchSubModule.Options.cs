using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityPatch.Options;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    internal static readonly CommunityPatchOptionsFile Options = new CommunityPatchOptionsFile();

    internal void ShowOptions() {
      // ReSharper disable once UseObjectOrCollectionInitializer
      var elements = new List<InquiryElement>();

      var disableIntroVideo = Options.DisableIntroVideo;
      elements.Add(new InquiryElement(
        nameof(disableIntroVideo),
        disableIntroVideo ? "Enable Intro Videos" : "Disable Intro Videos",
        null
      ));

      var recordFirstChanceExceptions = Options.RecordFirstChanceExceptions;
      elements.Add(new InquiryElement(
        nameof(recordFirstChanceExceptions),
        recordFirstChanceExceptions ? "Ignore First Chance Exceptions" : "Record First Chance Exceptions",
        null
      ));

      var dontGroupThirdPartyMenuOptions = Options.DontGroupThirdPartyMenuOptions;
      elements.Add(new InquiryElement(
        nameof(dontGroupThirdPartyMenuOptions),
        dontGroupThirdPartyMenuOptions ? "Group 3rd Party Menu Options" : "Don't Group 3rd Party Menu Options",
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
      var disableIntroVideo = Options.DisableIntroVideo;
      var recordFirstChanceExceptions = Options.RecordFirstChanceExceptions;
      var dontGroupThirdPartyMenuOptions = Options.DontGroupThirdPartyMenuOptions;
      switch (selected) {
        case nameof(disableIntroVideo):
          disableIntroVideo.Set(!disableIntroVideo);
          ShowMessage($"Intro Videos: {(disableIntroVideo ? "Disabled" : "Enabled")}.");
          Options.Save();
          break;

        case nameof(recordFirstChanceExceptions):
          recordFirstChanceExceptions.Set(!recordFirstChanceExceptions);
          ShowMessage($"Record FCEs: {(recordFirstChanceExceptions ? "Enabled" : "Disabled")}.");
          Options.Save();
          break;

        case nameof(Diagnostics.GenerateReport):
          Diagnostics.GenerateReport();
          break;

        case nameof(dontGroupThirdPartyMenuOptions):
          dontGroupThirdPartyMenuOptions.Set(!dontGroupThirdPartyMenuOptions);
          ShowMessage($"3rd Party Menu Options: {(dontGroupThirdPartyMenuOptions ? "Loose" : "Grouped")}.");
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