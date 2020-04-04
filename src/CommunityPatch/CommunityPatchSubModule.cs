using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

    internal static readonly LinkedList<Exception> RecordedFirstChanceExceptions
      = new LinkedList<Exception>();

    internal static readonly LinkedList<Exception> RecordedUnhandledExceptions
      = new LinkedList<Exception>();

    internal static readonly OptionsFile Options = new OptionsFile("CommunityPatch.txt");

    internal static bool DisableIntroVideo {
      get => Options.Get<bool>(nameof(DisableIntroVideo));
      set => Options.Set(nameof(DisableIntroVideo), value);
    }

    internal static bool RecordFirstChanceExceptions {
      get => Options.Get<bool>(nameof(RecordFirstChanceExceptions));
      set => Options.Set(nameof(RecordFirstChanceExceptions), value);
    }

    public override void BeginGameStart(Game game)
      => base.BeginGameStart(game);

    public override bool DoLoading(Game game)
      => base.DoLoading(game);

    protected override void OnBeforeInitialModuleScreenSetAsRoot() {
      var module = Module.CurrentModule;

      {
        // remove the space option that DeveloperConsole module adds
        var spaceOpt = module.GetInitialStateOptionWithId("Space");
        if (spaceOpt != null) {
          var opts = module.GetInitialStateOptions()
            .Where(opt => opt != spaceOpt).ToArray();
          module.ClearStateOptions();
          foreach (var opt in opts)
            module.AddInitialStateOption(opt);
        }
      }

      if (DisableIntroVideo) {
        try {
          typeof(Module)
            .GetField("_splashScreenPlayed", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(module, true);
        }
        catch (Exception ex) {
          Error(ex, "Couldn't disable intro video.");
        }
      }

      base.OnBeforeInitialModuleScreenSetAsRoot();
    }

    protected override void OnSubModuleLoad() {
      var module = Module.CurrentModule;
      module.AddInitialStateOption(new InitialStateOption(
        "ModOptions",
        new TextObject("Mod Options"),
        10001,
        ShowModOptions,
        false
      ));
      base.OnSubModuleLoad();
    }

    private static void ShowMessage(string msg)
      => InformationManager.DisplayMessage(new InformationMessage(msg));

    private void ShowModOptions()
      => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        "Mod Options",
        "Community Patch Mod Options:",
        new List<InquiryElement> {
          new InquiryElement(
            nameof(DisableIntroVideo),
            DisableIntroVideo ? "Enable Intro Videos" : "Disable Intro Videos",
            null
          ),
          new InquiryElement(
            nameof(RecordedFirstChanceExceptions),
            DisableIntroVideo ? "Record First Chance Exceptions" : "Ignore First Chance Exceptions",
            null
          ),
          new InquiryElement(
            nameof(CopyDiagnosticsToClipboard),
            "Copy Diagnostics to Clipboard",
            null
          )
        },
        true,
        true,
        "Apply",
        "Return",
        list => {
          var selected = (string) list[0].Identifier;
          switch (selected) {
            case nameof(DisableIntroVideo):
              DisableIntroVideo = !DisableIntroVideo;
              ShowMessage($"Intro Videos: {(DisableIntroVideo ? "Disabled" : "Enabled")}.");
              Options.Save();
              break;
            case nameof(RecordFirstChanceExceptions):
              RecordFirstChanceExceptions = !RecordFirstChanceExceptions;
              ShowMessage($"Record FCEs: {(RecordFirstChanceExceptions ? "Disabled" : "Enabled")}.");
              Options.Save();
              break;
            case nameof(CopyDiagnosticsToClipboard):
              CopyDiagnosticsToClipboard();
              break;
            default:
              throw new NotImplementedException(selected);
          }
        }, null));

    public override void OnGameInitializationFinished(Game game) {
      var patchType = typeof(IPatch);
      var patches = new LinkedList<IPatch>();

      foreach (var type in typeof(CommunityPatchSubModule).Assembly.GetTypes()) {
        if (!patchType.IsAssignableFrom(type))
          continue;
        if (patchType == type)
          continue;

        try {
          patches.AddLast((IPatch) Activator.CreateInstance(type, true));
        }
        catch (TargetInvocationException tie) {
          Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
        }
        catch (Exception ex) {
          Error(ex, $"Failed to create instance of patch: {type.FullName}");
        }
      }

      foreach (var patch in patches) {
        try {
          if (!patch.IsApplicable(game))
            continue;

          try {
            patch.Apply(game);
          }
          catch (Exception ex) {
            Error(ex, $"Error while applying patch: {patch.GetType().FullName}");
          }
        }
        catch (Exception ex) {
          Error(ex, $"Error while checking if patch is applicable: {patch.GetType().FullName}");
        }
      }

      base.OnGameInitializationFinished(game);
    }

  }

}