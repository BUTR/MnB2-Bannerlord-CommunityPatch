using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public sealed class TrustedCommandersPatch : PatchBase<TrustedCommandersPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultArmyManagementCalculationModel).GetMethod("CalculateCohesionChange", Public | Instance | DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(TrustedCommandersPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.2.1.226961
        0xFF, 0x28, 0x27, 0xC0, 0x56, 0xD1, 0x13, 0x01,
        0x4D, 0x06, 0x90, 0xA4, 0xE3, 0xF7, 0x0A, 0x85,
        0x00, 0xBB, 0x7A, 0x42, 0xD3, 0x82, 0x21, 0xFD,
        0x26, 0xA4, 0x9E, 0xB8, 0xA4, 0x22, 0xFB, 0xB9
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "6ETg3maz");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );

      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 50f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );
      
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref float __result, Army army, StatExplainer explanation = null) {
      var perk = ActivePatch._perk;
      var reduction = CalculatePerkReduction(army, perk);
      var flatReduction = __result * reduction;
      var finalCohesion = __result - flatReduction;

      __result = finalCohesion;
      
      if (flatReduction.IsDifferentFrom(0f))
        explanation?.AddLine(perk.Name.ToString(), -flatReduction);
    }

    private static float CalculatePerkReduction(Army army, PerkObject perk)
    {
      var partyMemberPower = new ExplainedNumber(1f);

      foreach (var party in army.Parties)
        PerkHelper.AddPerkBonusForParty(perk, party, ref partyMemberPower);

      var partyMemberFactorBonus = perk.PrimaryBonus / 100f;
      var additiveReduction = partyMemberPower.ResultNumber - partyMemberPower.BaseNumber;
      var partyMemberCount = (int) (additiveReduction / partyMemberFactorBonus);

      return CalculateMultiplicativeBonus(partyMemberFactorBonus, partyMemberCount);
    }

    private static float CalculateMultiplicativeBonus(float partyMemberBonus, int partyMemberCount) {
      var finalRate = 0f;
      var lastRate = 1f;
      
      for (var i = 0; i < partyMemberCount; i++) {
        var nextRate = partyMemberBonus * lastRate;
        finalRate += nextRate;
        lastRate = nextRate;
      }

      return finalRate;
    }
  }

}