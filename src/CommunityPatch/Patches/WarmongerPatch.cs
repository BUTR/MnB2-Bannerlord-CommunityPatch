using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class WarmongerPatch : PatchBase<WarmongerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultArmyManagementCalculationModel).GetMethod("CalculatePartyInfluenceCost", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(WarmongerPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "ldk9Xvod");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;
      
      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        // e.1.0.9
        0x89, 0x81, 0x49, 0x0B, 0x2D, 0x71, 0x5D, 0x56, 
        0xAE, 0x3C, 0x96, 0x5A, 0xF4, 0x9D, 0xBC, 0x9D, 
        0xE3, 0x0E, 0x59, 0xFD, 0x57, 0x85, 0x2B, 0xA4, 
        0x5E, 0xFD, 0x63, 0xE2, 0x9A, 0x5D, 0xAE, 0x31
      });
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedParameter.Local
    private static void Postfix(ref int __result, MobileParty armyLeaderParty, MobileParty party) {
      var perk = ActivePatch._perk;
      if (!(armyLeaderParty.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      __result = (int) Math.Round(__result * (1 + perk.PrimaryBonus));
    }

  }

}