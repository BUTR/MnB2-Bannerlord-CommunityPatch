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

  internal class StewardAgrarianPatch : PatchBase<StewardAgrarianPatch> {

    public override bool Applied { get; protected set; }
    
    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultVillageProductionCalculatorModel).GetMethod(nameof(DefaultVillageProductionCalculatorModel.CalculateDailyFoodProductionAmount), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(StewardAgrarianPatch).GetMethod("Postfix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "XNc2NIGL");
    public override bool IsApplicable(Game game)
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk.PrimaryBonus != 0f)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0x08,0x72,0x20,0x9c,0x9a,0x23,0x12,0xdf,
        0xd7,0xa2,0x83,0x20,0xa5,0x89,0xd7,0x6a,
        0x67,0xb3,0x35,0x22,0x66,0xb1,0xea,0x2c,
        0x21,0x00,0x4f,0x45,0x3c,0x74, 0x04,0x5e
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
        _perk.PrimaryRole, 0.3f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        SkillEffect.EffectIncrementType.AddFactor
      );
      if (Applied) return;
            
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
        
    }
    
    public static void Postfix(ref int __result, Village village) {
      var perk = ActivePatch._perk;
      if (!(village.Bound?.Town?.Governor?.GetPerkValue(perk) ?? false)) {
        return;
      }

      __result += (int) (__result * perk.PrimaryBonus);
    }
    

  }

}