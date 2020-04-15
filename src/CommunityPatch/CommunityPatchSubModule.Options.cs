using System;
using System.Collections.Generic;
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

        default: throw new NotImplementedException(selected);
      }
    }

  }

}