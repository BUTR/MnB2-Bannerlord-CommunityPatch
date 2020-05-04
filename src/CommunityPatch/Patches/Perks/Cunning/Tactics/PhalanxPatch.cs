using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class PhalanxPatch : SimulateHitPatch<PhalanxPatch> {

    public PhalanxPatch() : base("5vs3qlQ8") {
    }

    private static readonly MethodInfo PatchMethodInfo = typeof(PhalanxPatch).GetMethod(nameof(Postfix), Public | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref int __result, CharacterObject strikerTroop, CharacterObject strikedTroop, PartyBase strikerParty) {
      if (strikerTroop.IsInfantry && strikedTroop.IsMounted)
        ApplyPerk(ref __result, ActivePatch.Perk, strikerParty);
    }

  }

}