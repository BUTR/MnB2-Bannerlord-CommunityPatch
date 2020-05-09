using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public sealed class CompanionCavalryPatch : PerkPatchBase<CompanionCavalryPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(AgentMoraleInteractionLogic).GetMethod("OnAgentRemoved", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(CompanionCavalryPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xDC, 0x73, 0xE5, 0x9D, 0x02, 0x06, 0x68, 0x6B,
        0x9F, 0x87, 0xE2, 0xD1, 0xE4, 0x96, 0x9E, 0xB7,
        0xA5, 0xA1, 0x21, 0xCA, 0x9E, 0xC5, 0x06, 0xE9,
        0xB2, 0xD3, 0x58, 0x35, 0x5B, 0x27, 0x14, 0x72
      }
    };

    public CompanionCavalryPatch() : base("UqzavawD") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      Perk.Modify(.10f, SkillEffect.EffectIncrementType.AddFactor);
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(AgentMoraleInteractionLogic __instance, Agent affectedAgent, Agent affectorAgent, AgentState agentState) {
      var affectorCharacter = (CharacterObject) affectorAgent?.Character;
      var affectorLeaderCharacter = (CharacterObject) affectorAgent?.Team?.Leader?.Character;

      if (affectorCharacter == null) return;
      if (affectorLeaderCharacter?.GetPerkValue(ActivePatch.Perk) != true) return;
      if (!affectorAgent.HasMount) return;
      if (affectedAgent.Character == null) return;
      if (affectedAgent.Team == null) return;
      if (agentState != AgentState.Killed && agentState != AgentState.Unconscious) return;

      var moralChangesTuple = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeAfterAgentKilled(affectedAgent);
      var moralChangeFriend = moralChangesTuple.Item1 * ActivePatch.Perk.PrimaryBonus;

      __instance.ApplyAoeMoraleEffect(
        affectedAgent.GetWorldPosition(),
        affectorAgent.GetWorldPosition(),
        affectedAgent.Team,
        moralChangeFriend,
        0f,
        10f,
        a => a.IsActive() && a.Formation != null && a.Formation == affectedAgent.Formation,
        a => a.IsActive() && a.Formation != null && a.Formation == affectorAgent.Formation);
    }

  }

}