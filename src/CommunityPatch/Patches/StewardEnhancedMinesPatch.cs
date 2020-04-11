using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Library;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class StewardEnhancedMinesPatch : PatchBase<StewardEnhancedMinesPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo =
      Type.GetType("TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultSettlementTaxModel, TaleWorlds.CampaignSystem")?
        .GetMethod("CalculateVillageTaxFromIncome", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardEnhancedMinesPatch).GetMethod("Postfix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "6oE7rB6q");

    public override bool IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk == null)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        //e 1.0.10
        0x9c,0xef,0x1b,0xea,0x89,0xf0,0x47,0xcd,
        0xe7,0xda,0x5c,0x76,0x9b,0xb7,0x3f,0x17,
        0x87,0xe3,0x91,0x2a,0x21,0xce,0xfb,0x8e,
        0x6e,0xfc,0xe3,0xf3,0x92,0xeb,0x9d,0xa4
      }) || 
      hash.SequenceEqual(new byte[] {
        0x69,0x6f,0x6f,0xbc,0x27,0x36,0x72,0x47,
        0x54,0x6c,0x67,0x83,0x3c,0xd1,0xed,0x5e,
        0xd4,0x1a,0x42,0x51,0x28,0x80,0x5e,0x52,
        0x12,0xa6,0x21,0xdb,0x02,0x13,0x4c,0x05
      });
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );
      // most of the properties of skills have private setters, yet Initialize is public
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 0.5f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        SkillEffect.EffectIncrementType.AddFactor
      );
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once t
    public static void Postfix(ref int __result, Village village, int marketIncome) {
      var perk = ActivePatch._perk;
      if (!(village.Bound?.OwnerClan?.Leader?.GetPerkValue(perk) ?? false)) {
        return;
      }
      if (village.VillageType.PrimaryProduction.IsFood) 
        return;
      var explainedNumber = new ExplainedNumber(__result, null);
      explainedNumber.AddFactor(perk.PrimaryBonus,perk.Description);
      __result = (int) explainedNumber.ResultNumber;
    }

  }

}