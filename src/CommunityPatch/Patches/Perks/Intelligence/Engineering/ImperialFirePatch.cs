using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Engineering {

  public class ImperialFirePatch : PerkPatchBase<ImperialFirePatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo AttackerTargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.MapSiege.MapSiegeProductionVM, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetAllAttackerRangedMachines", NonPublic | Public | Instance | Static | DeclaredOnly);

    private static readonly MethodInfo DefenderTargetMethodInfo =
      Type.GetType("SandBox.ViewModelCollection.MapSiege.MapSiegeProductionVM, SandBox.ViewModelCollection, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")?
        .GetMethod("GetAllDefenderMachines", NonPublic | Public | Instance | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfoPostfix = typeof(ImperialFirePatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return AttackerTargetMethodInfo;
      yield return DefenderTargetMethodInfo;
    }

    private static readonly byte[][] AttackerHashes = {
      new byte[] {
        // e1.1.0.225190
        0xAC, 0x2D, 0x81, 0xE5, 0xC9, 0xA7, 0x97, 0x09,
        0xE0, 0xD6, 0x9A, 0x22, 0xC8, 0x86, 0x28, 0x1F,
        0x4A, 0x7B, 0x18, 0x9F, 0xCF, 0x27, 0xD8, 0xBC,
        0x84, 0xD2, 0xD4, 0x6D, 0xC1, 0x30, 0x36, 0x43
      }
    };

    private static readonly byte[][] DefenderHashes = {
      new byte[] {
        // e1.1.0.225190
        0x36, 0x8E, 0x5A, 0x15, 0x31, 0xDC, 0xB0, 0x6F,
        0x0C, 0xD1, 0x55, 0xAF, 0x85, 0xC6, 0xD5, 0x8F,
        0x60, 0x3F, 0x74, 0xDD, 0xF2, 0x4E, 0xC8, 0xD2,
        0xC9, 0xAA, 0xEA, 0xE8, 0x66, 0x60, 0xE7, 0x57
      }
    };

public ImperialFirePatch() : base("UaZSa5mY") {}

    public override bool? IsApplicable(Game game)
      // ReSharper disable once CompareOfFloatsByEqualityOperator
    {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus != 0.3f) return false;

      var attackerPatchInfo = Harmony.GetPatchInfo(AttackerTargetMethodInfo);
      if (AlreadyPatchedByOthers(attackerPatchInfo)) return false;

      var defenderPatchInfo = Harmony.GetPatchInfo(DefenderTargetMethodInfo);
      if (AlreadyPatchedByOthers(defenderPatchInfo)) return false;

      var attackerHash = AttackerTargetMethodInfo.MakeCilSignatureSha256();
      var defenderHash = DefenderTargetMethodInfo.MakeCilSignatureSha256();
      return attackerHash.MatchesAnySha256(AttackerHashes) && defenderHash.MatchesAnySha256(DefenderHashes);
    }

    public override void Apply(Game game) {
      Perk.Modify(1f, SkillEffect.EffectIncrementType.Add);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(AttackerTargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      CommunityPatchSubModule.Harmony.Patch(DefenderTargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfoPostfix));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Postfix(ref IEnumerable<SiegeEngineType> __result) {
      if (Hero.MainHero == null) return;
      if (AnyPartyMemberHasThePerkActive(Hero.MainHero.PartyBelongedTo)) return;

      __result = RemoveFireEngines(__result);
    }

    private static bool AnyPartyMemberHasThePerkActive(MobileParty party) {
      var perk = ActivePatch.Perk;
      var partyMemberValue = new ExplainedNumber(0f);
      PerkHelper.AddPerkBonusForParty(perk, party, ref partyMemberValue);

      if (party.Army?.Parties != null)
        foreach (var armyParty in party.Army?.Parties)
          PerkHelper.AddPerkBonusForParty(perk, armyParty, ref partyMemberValue);

      return partyMemberValue.ResultNumber.IsEqualOrGreaterThan(1f);
    }

    private static IEnumerable<SiegeEngineType> RemoveFireEngines(IEnumerable<SiegeEngineType> engineTypes)
      => engineTypes.Where(x =>
        x != DefaultSiegeEngineTypes.FireBallista &&
        x != DefaultSiegeEngineTypes.FireCatapult &&
        x != DefaultSiegeEngineTypes.FireOnager);

  }

}