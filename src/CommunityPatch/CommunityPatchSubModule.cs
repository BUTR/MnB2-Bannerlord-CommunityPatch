using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {

    internal static readonly Harmony Harmony = new Harmony(nameof(CommunityPatch));

    internal static readonly LinkedList<Exception> RecordedFirstChanceExceptions
      = new LinkedList<Exception>();

    internal static readonly LinkedList<Exception> RecordedUnhandledExceptions
      = new LinkedList<Exception>();

    internal static readonly OptionsFile Options = new OptionsFile(nameof(CommunityPatch) + ".txt");

    [PublicAPI]
    internal static CampaignGameStarter CampaignGameStarter;

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

      if (IsEarlierThanVersionE108 && DisableIntroVideo) {
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

    private bool _ticked = false;

    protected override void OnApplicationTick(float dt) {
      if (!_ticked) {
        _ticked = true;
        SynchronizationContext.Current.Post(_ => {
          LoadDelayedSubModules();
        }, null);
        base.OnApplicationTick(dt);
      }

      // other stuff?
    }

    private static void ShowMessage(string msg)
      => InformationManager.DisplayMessage(new InformationMessage(msg));

    private void ShowModOptions() {
      var elements = new List<InquiryElement>();

      if (IsEarlierThanVersionE108) {
        elements.Add(new InquiryElement(
          nameof(DisableIntroVideo),
          DisableIntroVideo ? "Enable Intro Videos" : "Disable Intro Videos",
          null
        ));
      }

      elements.Add(new InquiryElement(
        nameof(RecordFirstChanceExceptions),
        RecordFirstChanceExceptions ? "Ignore First Chance Exceptions" : "Record First Chance Exceptions",
        null
      ));
      elements.Add(new InquiryElement(
        nameof(CopyDiagnosticsToClipboard),
        "Copy Diagnostics to Clipboard",
        null
      ));
      elements.Add(new InquiryElement(
        "IntentionallyUnhandled",
        "Throw Unhandled Exception",
        null
      ));
      InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        "Mod Options",
        "Community Patch Mod Options:",
        elements,
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
              ShowMessage($"Record FCEs: {(RecordFirstChanceExceptions ? "Enabled" : "Disabled")}.");
              Options.Save();
              break;
            case nameof(CopyDiagnosticsToClipboard):
              CopyDiagnosticsToClipboard();
              break;
            default:
              throw new NotImplementedException(selected);
          }
        }, null));
    }

    public override void OnCampaignStart(Game game, object starterObject) {
      if (starterObject is CampaignGameStarter cgs)
        CampaignGameStarter = cgs;

      base.OnCampaignStart(game, starterObject);
    }

    public override void OnGameInitializationFinished(Game game) {
      ApplyPatches(game);

      base.OnGameInitializationFinished(game);
    }

    public static IDictionary<Type, IPatch> ActivePatches
      = new Dictionary<Type, IPatch>();

    private static void ApplyPatches(Game game) {
      //ActivePatches.Clear();

      foreach (var patch in Patches) {
        try {
          patch.Reset();

          if (!patch.IsApplicable(game))
            continue;

          try {
            patch.Apply(game);
          }
          catch (Exception ex) {
            Error(ex, $"Error while applying patch: {patch.GetType().Name}");
          }
        }
        catch (Exception ex) {
          Error(ex, $"Error while checking if patch is applicable: {patch.GetType().Name}");
        }

        var patchApplied = patch.Applied;
        if (patchApplied)
          ActivePatches[patch.GetType()] = patch;

        ShowMessage($"{(patchApplied ? "Applied" : "Skipped")} Patch: {patch.GetType().Name}");
      }
    }

    private static LinkedList<IPatch> _patches;

    private static readonly ApplicationVersion VersionE108 = new ApplicationVersion(ApplicationVersionType.EarlyAccess, 1, 0, 8);

    private static readonly ApplicationVersionComparer VersionComparer = new ApplicationVersionComparer();

    private static bool IsEarlierThanVersionE108
      => VersionComparer.Compare(VersionE108, ModuleInfo.GetModules().First(x => x.IsNative()).Version) > 0;

    private static LinkedList<IPatch> Patches {
      get {
        if (_patches != null)
          return _patches;

        var patchType = typeof(IPatch);
        _patches = new LinkedList<IPatch>();

        foreach (var type in typeof(CommunityPatchSubModule).Assembly.GetTypes()) {
          if (!patchType.IsAssignableFrom(type))
            continue;
          if (patchType == type)
            continue;

          try {
            _patches.AddLast((IPatch) Activator.CreateInstance(type, true));
          }
          catch (TargetInvocationException tie) {
            Error(tie.InnerException, $"Failed to create instance of patch: {type.FullName}");
          }
          catch (Exception ex) {
            Error(ex, $"Failed to create instance of patch: {type.FullName}");
          }
        }

        return _patches;
      }
    }

  }

}