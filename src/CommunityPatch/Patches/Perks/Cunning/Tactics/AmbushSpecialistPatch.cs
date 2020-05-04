using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class AmbushSpecialistPatch : SimulateHitPatch<AmbushSpecialistPatch> {
    public AmbushSpecialistPatch() : base("1fQxfkLX") {}

    private static readonly MethodInfo PatchMethodInfo = typeof(AmbushSpecialistPatch).GetMethod(nameof(Postfix), Public | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    
    public static void Postfix(ref int __result, CharacterObject strikerTroop, PartyBase strikerParty) {
      if (strikerTroop.IsArcher && IsInForest(strikerParty))
        ApplyPerk(ref __result, ActivePatch.Perk, strikerParty);
    }

    private static bool IsInForest(PartyBase party) {
      if (party.MobileParty == null) return false;
      var faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(party.MobileParty.CurrentNavigationFace);
      return faceTerrainType == TerrainType.Forest;
    }
  }
}