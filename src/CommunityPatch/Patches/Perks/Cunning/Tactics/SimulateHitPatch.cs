using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public abstract class SimulateHitPatch<TPatch> : PatchBase<TPatch> where TPatch : PatchBase<TPatch> {
    
    public override bool Applied { get; protected set; }

    protected static readonly MethodInfo TargetMethodInfo = typeof(DefaultCombatSimulationModel).GetMethod(nameof(DefaultCombatSimulationModel.SimulateHit), Public | Instance | DeclaredOnly);

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0x01, 0x52, 0x68, 0x11, 0x93, 0xB2, 0xA1, 0x81,
        0x78, 0x07, 0xBE, 0x35, 0xBB, 0x79, 0x66, 0x53,
        0x0B, 0x62, 0x52, 0xD2, 0x3F, 0xAA, 0xB5, 0xC0,
        0x69, 0xBC, 0x13, 0xFF, 0x40, 0xD4, 0x49, 0x4C
      }
    };
    
    protected PerkObject Perk;
    
    public override void Reset()
      => Perk = PerkObject.FindFirst(x => x.Name.GetID() == PerkId);
    
    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }
    
    protected static void ApplyPerk(ref int totalDamage, PerkObject perk, PartyBase strikerParty) {
      if (strikerParty.LeaderHero?.GetPerkValue(perk) != true) return;
      totalDamage += (int) (totalDamage * perk.PrimaryBonus);
    }
    
    protected abstract string PerkId { get; }
  }
}