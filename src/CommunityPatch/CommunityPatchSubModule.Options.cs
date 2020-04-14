using System;
using System.Collections.Generic;
using TaleWorlds.Core;

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

    private void ShowOptions() {
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
        nameof(Diagnostics.CopyToClipboard),
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
        "Community Patch Options",
        "Pick an option:",
        elements,
        true,
        true,
        "Apply",
        "Return",
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

        case nameof(Diagnostics.CopyToClipboard):
          Diagnostics.CopyToClipboard();
          break;

        default: throw new NotImplementedException(selected);
      }
    }

  }

}