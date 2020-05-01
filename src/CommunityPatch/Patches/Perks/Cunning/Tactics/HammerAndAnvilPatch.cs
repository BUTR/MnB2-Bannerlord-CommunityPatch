using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class HammerAndAnvilPatch : SimulateHitPatch<HammerAndAnvilPatch> {
    protected override string PerkId => "drLxFmla";

    private static readonly MethodInfo PatchMethodInfo = typeof(HammerAndAnvilPatch).GetMethod(nameof(Postfix), Public | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref int __result, CharacterObject strikerTroop, CharacterObject strikedTroop, PartyBase strikerParty) {
      if (strikerTroop.IsMounted && strikedTroop.IsArcher && !strikedTroop.IsMounted)
        ApplyPerk(ref __result, ActivePatch.Perk, strikerParty);
    }
  }
}