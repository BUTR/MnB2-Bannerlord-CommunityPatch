using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class MerryMenPatch : PerkPatchBase<MerryMenPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo BanditsJoinTargetMethodInfo =
      typeof(BanditsCampaignBehavior).GetMethod("conversation_bandits_will_join_player_on_condition", NonPublic | DeclaredOnly | Instance);

    private static readonly MethodInfo BanditsJoinPatchMethodInfo = typeof(MerryMenPatch).GetMethod(nameof(PrefixBanditsJoin), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return BanditsJoinTargetMethodInfo;
    }

    private static readonly byte[][] BanditsJoinHashes = {
      new byte[] {
        // e1.1.0.225190
        0xB1, 0x5A, 0x5C, 0xF7, 0x1A, 0x50, 0x55, 0xFF,
        0x13, 0x70, 0xD2, 0xA5, 0x69, 0x27, 0x60, 0x8F,
        0x4A, 0xF9, 0x8E, 0x46, 0x8B, 0xE1, 0x96, 0xD9,
        0x1E, 0x4F, 0x7B, 0x2B, 0x43, 0x74, 0x8B, 0x4C
      }
    };

public MerryMenPatch() : base("ssljPTUr") {}

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var banditsJoinPatchInfo = Harmony.GetPatchInfo(BanditsJoinTargetMethodInfo);
      if (AlreadyPatchedByOthers(banditsJoinPatchInfo)) return false;

      var banditsJoinHash = BanditsJoinTargetMethodInfo.MakeCilSignatureSha256();
      return banditsJoinHash.MatchesAnySha256(BanditsJoinHashes);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(BanditsJoinTargetMethodInfo, new HarmonyMethod(BanditsJoinPatchMethodInfo));
      Applied = true;
    }

    
    public static bool PrefixBanditsJoin(ref bool __result)
      => ApplyPerkToMakeRecruitable(ref __result);

    private static bool ApplyPerkToMakeRecruitable(ref bool intimidated) {
      var perk = ActivePatch.Perk;
      if (MobileParty.MainParty.LeaderHero?.GetPerkValue(perk) != true) return true;

      intimidated = PartyBaseHelper.DoesSurrenderIsLogicalForParty(
        MobileParty.ConversationParty,
        MobileParty.MainParty,
        // requires that the bandits be merely less than 100% of our strength to surrender.
        1f);

      return false;
    }

  }

}