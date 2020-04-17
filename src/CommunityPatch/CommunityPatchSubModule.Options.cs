using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public partial class CommunityPatchSubModule {

    internal static readonly OptionsFile Options = new OptionsFile(nameof(CommunityPatch) + ".txt");

    internal static bool DisableIntroVideo {
      get => Options.Get<bool>(nameof(DisableIntroVideo));
      set => Options.Set(nameof(DisableIntroVideo), value);
    }

    internal static bool RecordFirstChanceExceptions {
      get => Options.Get<bool>(nameof(RecordFirstChanceExceptions));
      set => Options.Set(nameof(RecordFirstChanceExceptions), value);
    }

    internal static bool SuppressTerminalTickExceptions {
      get => Options.Get<bool>(nameof(SuppressTerminalTickExceptions));
      set => Options.Set(nameof(SuppressTerminalTickExceptions), value);
    }

    internal static bool DontGroupThirdPartyMenuOptions {
      get => Options.Get<bool>(nameof(DontGroupThirdPartyMenuOptions));
      set => Options.Set(nameof(DontGroupThirdPartyMenuOptions), value);
    }

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
        nameof(SuppressTerminalTickExceptions),
        SuppressTerminalTickExceptions ? "Don't Suppress Terminal Tick Exceptions" : "Suppress Terminal Tick Exceptions",
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
          DisableIntroVideo = !DisableIntroVideo;
          ShowMessage($"Intro Videos: {(DisableIntroVideo ? "Disabled" : "Enabled")}.");
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

        case nameof(SuppressTerminalTickExceptions):
          SuppressTerminalTickExceptions = !SuppressTerminalTickExceptions;
          ShowMessage($"Terminal Tick Exceptions: {(SuppressTerminalTickExceptions ? "Suppressed" : "Allowed")}.");
          Options.Save();
          break;

        case nameof(DontGroupThirdPartyMenuOptions):
          DontGroupThirdPartyMenuOptions = !DontGroupThirdPartyMenuOptions;
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