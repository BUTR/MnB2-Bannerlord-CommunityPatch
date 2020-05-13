using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class ArmorcraftPatch : PerkPatchBase<ArmorcraftPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Agent).GetMethod(nameof(Agent.GetBaseArmorEffectivenessForBodyPart), Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ArmorcraftPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public ArmorcraftPatch() : base("zi4GGkEj") {
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x8E, 0x0F, 0x93, 0x90, 0xFE, 0x14, 0x18, 0xE9,
        0x4C, 0x5B, 0x37, 0x6E, 0x8E, 0x81, 0x17, 0xF7,
        0xA9, 0x6C, 0xA1, 0x02, 0x2E, 0x85, 0x06, 0x78,
        0x69, 0x8A, 0x83, 0x59, 0xE4, 0x75, 0xDA, 0x00
      }
    };

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(.1f);
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    public static void Postfix(ref float __result, ref Agent __instance) {
      if (!__instance.IsHuman) return;
      if (!__instance.IsHero) return;
      if (!(__instance.Character is CharacterObject character)) return;

      var hero = character.HeroObject;
      if (hero == null) return;

      var perk = ActivePatch.Perk;
      if (hero.GetPerkValue(perk))
        __result += __result * perk.PrimaryBonus;
    }

  }

}