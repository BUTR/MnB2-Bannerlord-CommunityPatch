using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class SlaveTraderPatch : PatchBase<SlaveTraderPatch> {
    
    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(CharacterObject).GetMethod(nameof(CharacterObject.PrisonerRansomValue), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(SlaveTraderPatch).GetMethod(nameof(Postfix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xB4, 0xEA, 0x03, 0x63, 0x1F, 0x2B, 0xA6, 0x8C,
        0x4A, 0x2E, 0x86, 0xFE, 0x67, 0xCD, 0x82, 0x4D,
        0x79, 0x6B, 0xA4, 0xDC, 0x38, 0xC3, 0xCE, 0xE7,
        0x8A, 0x98, 0x20, 0x9D, 0xC2, 0x2E, 0x1E, 0x88
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "jNbTBxEW");

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
        _perk.PrimaryRole, .20f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        _perk.IncrementType
      );

      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref int __result, Hero sellerHero = null) {
      if (sellerHero == null) return;

      var perk = ActivePatch._perk;
      if (!sellerHero.GetPerkValue(perk)) return;

      __result += (int) (__result * perk.PrimaryBonus);
    }
  }
}