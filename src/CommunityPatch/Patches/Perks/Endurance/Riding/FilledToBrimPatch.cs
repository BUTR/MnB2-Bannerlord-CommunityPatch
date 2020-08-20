#if !AFTER_E1_4_3

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess,1,4, 3)]
  public sealed class FilledToBrimPatch : PerkPatchBase<FilledToBrimPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultInventoryCapacityModel).GetMethod("CalculateInventoryCapacity", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(FilledToBrimPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.11
        0x91, 0x8F, 0x69, 0x36, 0xA9, 0x4F, 0xE9, 0x5E,
        0x4C, 0x11, 0x97, 0x25, 0x80, 0xBF, 0x34, 0x50,
        0x72, 0x06, 0xBC, 0xD7, 0x77, 0x11, 0x96, 0x7B,
        0x10, 0x79, 0xA1, 0xD4, 0x8D, 0x6B, 0x8F, 0x43
      },
      new byte[] {
        // e1.4.1.229326
        0x88, 0x34, 0x5C, 0x3F, 0x55, 0xD2, 0xFD, 0x57,
        0x3E, 0x49, 0x0C, 0xA0, 0xA3, 0x52, 0xEA, 0xBE,
        0xF2, 0x60, 0x2D, 0x23, 0x4F, 0x52, 0x90, 0xFA,
        0xCA, 0x6A, 0x47, 0x7C, 0xA8, 0xC5, 0x54, 0x53
      },
      new byte[] {
        // e1.4.1.230527
        0xCB, 0x62, 0x4D, 0xDD, 0x3F, 0x7E, 0x3D, 0x69,
        0x8F, 0xF1, 0x72, 0x6D, 0x5F, 0x1C, 0xDD, 0xEE,
        0x5F, 0x69, 0x8F, 0x63, 0xDE, 0xB1, 0xE8, 0xA9,
        0x96, 0x2C, 0xE4, 0xFD, 0xDD, 0xA0, 0xD6, 0xC0
      }
    };

    public FilledToBrimPatch() : base("jikaakdy") {
    }

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref int __result, MobileParty mobileParty, StatExplainer explanation, bool includeFollowers) {
      var perk = ActivePatch.Perk;

      var heroesHavePerk = mobileParty.MemberRoster?
        .Count(x =>
          x.Character != null
          && x.Character.IsHero
          && x.Character.HeroObject != null
          && x.Character.HeroObject.GetPerkValue(perk)
        ) ?? 0;

      var extra = 20 * mobileParty.Party.NumberOfPackAnimals * heroesHavePerk + (includeFollowers ? mobileParty.AttachedParties.Sum(x => x.Party.NumberOfPackAnimals) : 0);
      if (extra <= 0)
        return;
      
      AddExplainedBonus(ref __result, extra, explanation);
    }
    
    private static void AddExplainedBonus(ref int inventoryCapacityLimit, int extra, StatExplainer explanation) {
      var explainedNumber = new ExplainedNumber(inventoryCapacityLimit, explanation);
      var baseLine = explanation?.Lines?.FindLast(explainedLine => explainedLine?.Name == "Base");
      if (baseLine != null)
        explanation.Lines.Remove(baseLine);

      explainedNumber.Add(extra, ActivePatch.Perk.Name);
      inventoryCapacityLimit = (int) explainedNumber.ResultNumber;
    }
    
  }

}

#endif