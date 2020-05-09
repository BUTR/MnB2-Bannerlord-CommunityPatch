using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class TacticalSuperiorityPatch : SimulateHitPatch<TacticalSuperiorityPatch> {

    public TacticalSuperiorityPatch() : base("gBKb8DoH") {
    }

    private static readonly MethodInfo PatchMethodInfo = typeof(TacticalSuperiorityPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref int __result, PartyBase strikerParty)
      => ApplyPerk(ref __result, ActivePatch.Perk, strikerParty);

  }

}