using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class StewardAgrarianPatch : IPatch {

    public bool Applied { get; private set; }

    private readonly PerkObject _perk
        = PerkObject.FindFirst(x => x.Name.GetID() == "XNc2NIGL");

    public bool IsApplicable(Game game)
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (_perk.PrimaryBonus == 0.3f)
        return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xb4, 0x8e, 0x91, 0x0e, 0x9f, 0x71, 0xc0, 0xf8,
        0x15, 0xa7, 0x63, 0xb0, 0x0b, 0x56, 0x76, 0xd2,
        0x02, 0xb5, 0xa7, 0x61, 0x3a, 0x52, 0x58, 0x23,
        0x5e, 0xbf, 0x3f, 0xb6, 0xe4, 0x93, 0xee, 0xa1
      });
    }

    private static readonly MethodInfo TargetMethodInfo = typeof(Town).GetMethod("get_FoodChange");
    private static readonly MethodInfo PatchMethodInfo = typeof(StewardAgrarianPatch).GetMethod("GetTownFoodStocksChangePatch");
    public void Apply(Game game) {
      // Dear TaleWorlds; Value should probably be publicly exposed, maybe by a method
      // and maybe marked [Obsolete] if you want to avoid your developers doing dirty deeds
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
        SkillEffect.PerkRole.Governor, 0.3f,
        _perk.PrimaryRole, _perk.PrimaryBonus,
        SkillEffect.EffectIncrementType.AddFactor
      );
      private static void GetTownFoodStocksChangePatch(Town __town, StatExplainer __result) {
        if(__town.Governor.GetPerkValue(DefaultPerks.Steward.Agrarian))
      }
      Applied = true;
    }

  }

}