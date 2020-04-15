using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using StoryMode;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using Module = TaleWorlds.MountAndBlade.Module;

namespace CommunityPatch {

  [PublicAPI]
  public partial class CommunityPatchSubModule : MBSubModuleBase {
    // may need to update this if vanilla's value changes.
    // vanilla has it in StoryMode.Behaviors.FirstPhaseCampaignBehavior.FirstPhaseTimeLimitInYears
    private const int FirstPhaseTimeLimitInYears = 10;

    internal static CommunityPatchSubModule Current
      => Module.CurrentModule.SubModules.OfType<CommunityPatchSubModule>().FirstOrDefault();

    [PublicAPI]
    internal static CampaignGameStarter CampaignGameStarter;

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

      CleanUpMainMenu();

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

    private List<InitialStateOption> _modOptionsMenus;

    internal List<InitialStateOption> Get3rdPartyOptionsMenus()
      => _modOptionsMenus ??= Module.CurrentModule.GetInitialStateOptions()
        .Where(Is3rdPartyOption)
        .Concat(_groupedOptionsMenus ?? (IEnumerable<InitialStateOption>) Array.Empty<InitialStateOption>())
        .ToList();

    private static bool _alreadyCleanedUpMainMenu = false;

    internal List<InitialStateOption> _groupedOptionsMenus;

    private void CleanUpMainMenu() {
      if (_alreadyCleanedUpMainMenu)
        return;

      _alreadyCleanedUpMainMenu = true;
      var menu = Module.CurrentModule.GetInitialStateOptions().ToArray();
      if (menu.Length > 8) {
        if (_groupedOptionsMenus != null)
          return;

        var skippedFirst = false;
        _groupedOptionsMenus = new List<InitialStateOption>();
        for (var i = 0; i < menu.Length; i++) {
          var item = menu[i];
          if (!Is3rdPartyOption(item))
            continue;

          if (!skippedFirst) {
            skippedFirst = true;
            continue;
          }

          _groupedOptionsMenus.Add(item);
          menu[i] = null;
        }

        Module.CurrentModule.ClearStateOptions();
        foreach (var opt in menu)
          if (opt != null)
            Module.CurrentModule.AddInitialStateOption(opt);

        Module.CurrentModule.AddInitialStateOption(new InitialStateOption(
          "MoreOptions",
          new TextObject("{=MoreOptions}More Options"),
          10001, ShowMoreMainMenuOptions, false));
      }
    }

    private static bool Is3rdPartyOption(InitialStateOption item) {
      var act = (Action) InitOptActField.GetValue(item);
      var optAsm = act.Method.DeclaringType?.Assembly;
      var optAsmName = optAsm?.GetName().Name;
      if (optAsmName == null)
        return false;
      if (optAsmName.StartsWith("TaleWorlds."))
        return false;
      if (optAsmName.StartsWith("SandBox."))
        return false;
      if (optAsmName.StartsWith("SandBoxCore."))
        return false;
      if (optAsmName.StartsWith("StoryMode."))
        return false;

      return true;
    }

    internal void ShowMoreMainMenuOptions()
      => ShowOptions(_groupedOptionsMenus);

    internal void ShowOptions(List<InitialStateOption> moreOptions)
      => InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
        new TextObject("{=MoreOptions}More Options").ToString(),
        null,
        moreOptions.Select(x => new InquiryElement(x.Id, x.Name.ToString(), null)
          )
          .ToList(),
        true,
        true,
        new TextObject("{=Open}Open").ToString(),
        null,
        picked => {
          var item = picked.FirstOrDefault();
          if (item == null)
            return;

          SynchronizationContext.Current.Post(_ => {
            moreOptions
              .FirstOrDefault(x => string.Equals(x.Id, (string) item.Identifier, StringComparison.Ordinal))
              ?.DoAction();
          }, null);
        }, null));

    private static readonly FieldInfo InitOptActField = typeof(InitialStateOption).GetField("_action", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override void OnSubModuleLoad() {
      var module = Module.CurrentModule;

      try {
        Harmony.PatchAll(typeof(CommunityPatchSubModule).Assembly);
      }
      catch (Exception ex) {
        Error(ex, "Could not apply all generic attribute-based harmony patches.");
      }

      module.AddInitialStateOption(new InitialStateOption(
        "CommunityPatchOptions",
        new TextObject("{=CommunityPatchOptions}Community Patch Options"),
        9998,
        ShowOptions,
        false
      ));

      base.OnSubModuleLoad();
    }

    internal static void ShowMessage(string msg) {
      InformationManager.DisplayMessage(new InformationMessage(msg));
      Print(msg);
    }

    public override void OnCampaignStart(Game game, object starterObject) {
      if (starterObject is CampaignGameStarter cgs)
        CampaignGameStarter = cgs;

      base.OnCampaignStart(game, starterObject);
    }

    private static bool IsFirstStoryPhase() {
      return FirstPhase.Instance != null
               && SecondPhase.Instance == null;
    }

    private void InitStoryQuestsTimeoutVisibility(Game game) {
      if (!(game.GameType is StoryMode.CampaignStoryMode))
        return;

      CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(
        this,
        new Action<QuestBase>(OnQuestStarted)
      );
      foreach (QuestBase quest
               in Campaign.Current.QuestManager.Quests.ToList<QuestBase>()) {
        SetStoryVisibleTimeoutIfNeeded(quest);
      }
    }

    private static void SetStoryVisibleTimeoutIfNeeded(QuestBase quest) {
      if (!IsFirstStoryPhase() || !quest.IsSpecialQuest || !quest.IsOngoing)
        return;

      // set visible timeout to be when vanilla would have (silently) timed out the quest, 
      // minus a day to make sure the quest doesn't somehow timeout due to vanilla behavior 
      // before this.
      CampaignTime newDueTime = FirstPhase.Instance.FirstPhaseStartTime
                                  + CampaignTime.Years(FirstPhaseTimeLimitInYears)
                                  - CampaignTime.Days(1);
      if (quest.QuestDueTime != newDueTime) {
        quest.ChangeQuestDueTime(newDueTime);
        ShowNotification(new TextObject("{=!}Quest time remaining was updated."), "event:/ui/notification/quest_update");
      }
    }

    private static void ShowNotification(TextObject message, string soundEventPath = "") {
      InformationManager.AddQuickInformation(message, 0, null, soundEventPath);
    }

    class NextHourlyTickListener {
      private Action<QuestBase> _func;
      private QuestBase _quest;

      public NextHourlyTickListener(Action<QuestBase> func, QuestBase quest) {
        _func = func;
        _quest = quest;
        CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
      }

      private void OnHourlyTick() {
        _func.Invoke(_quest);
        CampaignEvents.HourlyTickEvent.ClearListeners(this);
      }
    };

    private void OnQuestStarted(QuestBase quest) {
      // defer our updates until some time has passed, because they depend on whether 
      // FirstPhase or SecondPhase is active, and story Phase information is only 
      // updated after all OnQuestStarted handlers have fired.
      new NextHourlyTickListener(new Action<QuestBase>(SetStoryVisibleTimeoutIfNeeded), quest);
    }

    public override void OnGameInitializationFinished(Game game) {
      ApplyPatches(game);
      InitStoryQuestsTimeoutVisibility(game);

      base.OnGameInitializationFinished(game);
    }

  }

  [HarmonyPatch(typeof(EscapeMenuVM), MethodType.Constructor, typeof(IEnumerable<EscapeMenuItemVM>), typeof(TextObject))]
  public static class GroupEscapeMenuOptionsPatch {

    public static FieldInfo EscapeMenuItemVmOnExecute = typeof(EscapeMenuItemVM)
      .GetField("_onExecute", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public static FieldInfo EscapeMenuItemVmIdentifier = typeof(EscapeMenuItemVM)
      .GetField("_identifier", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly object _groupEscMenuOptsKey = new object();

    public static void Postfix(EscapeMenuVM __instance, ref MBBindingList<EscapeMenuItemVM> ____menuItems, IEnumerable<EscapeMenuItemVM> items, TextObject title = null) {
      var list = ____menuItems.ToList();

      var customOptions = new List<EscapeMenuItemVM>();
      for (var i = 0; i < list.Count; i++) {
        var item = list[i];

        var act = (Action<object>) EscapeMenuItemVmOnExecute.GetValue(item);
        var actAsm = act.Method.DeclaringType?.Assembly;
        var optAsmName = actAsm?.GetName().Name;

        if (optAsmName == null)
          continue;
        if (optAsmName.StartsWith("TaleWorlds."))
          continue;
        if (optAsmName.StartsWith("SandBox."))
          continue;
        if (optAsmName.StartsWith("SandBoxCore."))
          continue;
        if (optAsmName.StartsWith("StoryMode."))
          continue;

        customOptions.Add(item);
        list[i] = null;
      }

      var newList = new MBBindingList<EscapeMenuItemVM>();

      foreach (var item in list) {
        if (item != null)
          newList.Add(item);
      }

      if (customOptions.Count <= 1) {
        newList.Add(new EscapeMenuItemVM(new TextObject("{=CommunityPatchOptions}Community Patch Options"),
          _ => CommunityPatchSubModule.Current.ShowOptions(), _groupEscMenuOptsKey, false));

        ____menuItems = newList;
        return;
      }

      newList.Add(new EscapeMenuItemVM(new TextObject("{=MoreOptions}More Options"), _ => {
        var options = new List<InquiryElement> {
          new InquiryElement(_groupEscMenuOptsKey, new TextObject("{=CommunityPatchOptions}Community Patch Options").ToString(), null)
        };

        foreach (var item in customOptions) {
          var id = EscapeMenuItemVmIdentifier.GetValue(item);
          options.Add(new InquiryElement(id, item.ActionText, null, !item.IsDisabled, null));
        }

        InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
          new TextObject("{=MoreOptions}More Options").ToString(),
          null,
          options,
          true,
          true,
          new TextObject("{=Open}Open").ToString(),
          null,
          selection => {
            var picked = selection.FirstOrDefault()?.Identifier;
            if (picked == _groupEscMenuOptsKey) {
              CommunityPatchSubModule.Current.ShowOptions();
              return;
            }

            foreach (var item in customOptions) {
              var id = EscapeMenuItemVmIdentifier.GetValue(item);
              if (picked != id)
                continue;

              item.ExecuteAction();
              return;
            }
          },
          null
        ), true);
      }, "MoreOptions", false));

      ____menuItems = newList;
    }

  }

}