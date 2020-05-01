using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class EvisceratorPatch : PatchBase<EvisceratorPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(AgentMoraleInteractionLogic).GetMethod("OnAgentRemoved", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(EvisceratorPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xDC, 0x73, 0xE5, 0x9D, 0x02, 0x06, 0x68, 0x6B,
        0x9F, 0x87, 0xE2, 0xD1, 0xE4, 0x96, 0x9E, 0xB7,
        0xA5, 0xA1, 0x21, 0xCA, 0x9E, 0xC5, 0x06, 0xE9,
        0xB2, 0xD3, 0x58, 0x35, 0x5B, 0x27, 0x14, 0x72
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "C2CwsC91");

    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;
      if (_perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      _perk.Modify(-30f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Prefix(AgentMoraleInteractionLogic __instance, Agent affectedAgent, Agent affectorAgent, AgentState agentState) {
      var affectorCharacter = (CharacterObject) affectorAgent?.Character;

      if (affectorCharacter == null) return false;
      if (affectedAgent.Character == null) return false;
      if (agentState != AgentState.Killed && agentState != AgentState.Unconscious) return false;
      if (affectedAgent.Team == null) return false;

      var moralChangesTuple = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeAfterAgentKilled(affectedAgent);
      var moralChangeFriend = moralChangesTuple.Item1;
      var moralChangeEnemy = moralChangesTuple.Item2;

      if (!AnyMoralChanges(moralChangeEnemy, moralChangeFriend)) return false;

      if (affectorCharacter.GetPerkValue(ActivePatch._perk))
        moralChangeFriend += moralChangeFriend * ActivePatch._perk.PrimaryBonus / -100;

      __instance.ApplyAoeMoraleEffect(
        affectedAgent.GetWorldPosition(),
        affectorAgent.GetWorldPosition(),
        affectedAgent.Team,
        moralChangeFriend,
        moralChangeEnemy,
        10f,
        a => a.IsActive() && a.Formation != null && a.Formation == affectedAgent.Formation,
        a => a.IsActive() && a.Formation != null && a.Formation == affectorAgent.Formation);

      return false;
    }

    private static bool AnyMoralChanges(float moralChangeEnemy, float moralChangeFriend) {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      if (moralChangeEnemy != 0f) return true;

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      return moralChangeFriend != 0f;
    }

  }

}