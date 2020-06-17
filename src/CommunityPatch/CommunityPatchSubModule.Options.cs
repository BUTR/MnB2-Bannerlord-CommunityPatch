using System;
using System.Collections.Generic;
using CommunityPatch.Patches;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.PatchApplicabilityHelper;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    internal static readonly OptionsFile Options = new OptionsFile(nameof(CommunityPatch) + ".txt");

    internal static bool DisableIntroVideo {
      get => Options.Get<bool>(nameof(DisableIntroVideo));
      set => Options.Set(nameof(DisableIntroVideo), value);
    }

    internal static bool EnableTalkToOtherLordsInAnArmy {
      get => Options.Get<bool>(nameof(EnableTalkToOtherLordsInAnArmy));
      set => Options.Set(nameof(EnableTalkToOtherLordsInAnArmy), value);
    }

    internal static bool RecordFirstChanceExceptions {
      get => Options.Get<bool>(nameof(RecordFirstChanceExceptions));
      set => Options.Set(nameof(RecordFirstChanceExceptions), value);
    }

    internal static bool DontGroupThirdPartyMenuOptions {
      get => Options.Get<bool>(nameof(DontGroupThirdPartyMenuOptions));
      set => Options.Set(nameof(DontGroupThirdPartyMenuOptions), value);
    }

    internal static bool QuartermasterIsClanWide {
      get => Options.Get<bool>(nameof(QuartermasterIsClanWide));
      set => Options.Set(nameof(QuartermasterIsClanWide), value);
    }

    internal void ShowOptions() {
      // ReSharper disable once UseObjectOrCollectionInitializer
      var elements = new List<InquiryElement>();

      elements.Add(new InquiryElement(
        nameof(DisableIntroVideo),
        DisableIntroVideo ? "Enable Intro Videos" : "Disable Intro Videos",
        null
      ));

#if !AFTER_E1_4_2
      elements.Add(new InquiryElement(
        nameof(EnableTalkToOtherLordsInAnArmy),
        EnableTalkToOtherLordsInAnArmy ? "Disable Talk To Other Lords In An Army" : "Enable Talk To Other Lords In An Army",
        null
      ));
#endif

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
        nameof(QuartermasterIsClanWide),
        QuartermasterIsClanWide ? "Make Quartermaster Party Specific" : "Make Quartermaster Clan Wide",
        null));

      elements.Add(new InquiryElement(
        nameof(Diagnostics.GenerateReport),
        "Generate Diagnostics Report",
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
        null,
        elements,
        true,
#if AFTER_E1_4_1
          1,
#else
        true,
#endif
        new TextObject("{=BAaS5Dkc}Apply").ToString(),
        null,
        HandleOptionChoice,
        null
      ));
    }

    private void HandleOptionChoice(List<InquiryElement> list) {
      if (list.IsEmpty()) return;
      var selected = (string) list[0].Identifier;
      switch (selected) {
        case nameof(DisableIntroVideo):
          DisableIntroVideo = !DisableIntroVideo;
          ShowMessage($"Intro Videos: {(DisableIntroVideo ? "Disabled" : "Enabled")}.");
          Options.Save();
          break;

        case nameof(EnableTalkToOtherLordsInAnArmy):
          EnableTalkToOtherLordsInAnArmy = !EnableTalkToOtherLordsInAnArmy;
          ShowMessage($"Talk To Other Lords In An Army: {(EnableTalkToOtherLordsInAnArmy ? "Enabled" : "Disabled")}.");
          Options.Save();
          break;

        case nameof(RecordFirstChanceExceptions):
          RecordFirstChanceExceptions = !RecordFirstChanceExceptions;
          ShowMessage($"Record FCEs: {(RecordFirstChanceExceptions ? "Enabled" : "Disabled")}.");
          Options.Save();
          break;

        case nameof(Diagnostics.GenerateReport):
          Diagnostics.GenerateReport();
          break;

        case nameof(DontGroupThirdPartyMenuOptions):
          DontGroupThirdPartyMenuOptions = !DontGroupThirdPartyMenuOptions;
          ShowMessage($"3rd Party Menu Options: {(DontGroupThirdPartyMenuOptions ? "Loose" : "Grouped")}.");
          Options.Save();
          break;

        case nameof(QuartermasterIsClanWide):
          QuartermasterIsClanWide = !QuartermasterIsClanWide;
          ShowMessage($"Quartermaster Effects: {(QuartermasterIsClanWide ? "Clan-Wide" : "Party Only")}.");
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

    // ReSharper disable once FunctionRecursiveOnAllPaths
    private void CauseStackOverflow()
      => CauseStackOverflow();

  }

}