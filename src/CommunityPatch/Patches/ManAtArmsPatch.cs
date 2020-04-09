using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  sealed class ManAtArmsPatch : PatchBase<ManAtArmsPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPartySizeLimitModel).GetMethod("CalculateMobilePartyMemberSizeLimit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(ManAtArmsPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "WVLzi1fa");

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
          // e.1.0.7
          0x4B, 0x26, 0xD4, 0x1E, 0xF7, 0xCF, 0x5B, 0x15,
          0xE1, 0x24, 0x74, 0x8D, 0xE9, 0x46, 0x36, 0x80,
          0x6A, 0x91, 0x65, 0x5D, 0x7A, 0x6C, 0x3F, 0x43,
          0xD2, 0x7B, 0x80, 0xA7, 0x3E, 0xF0, 0x10, 0xF6
        })
        || hash.SequenceEqual(new byte[] {
          // e.1.0.8
          0xB5, 0xEE, 0x39, 0xE3, 0xF3, 0xDF, 0x4C, 0xE2,
          0xC0, 0xAF, 0xD3, 0x1B, 0x5F, 0x6D, 0x36, 0x11,
          0x76, 0x0B, 0xA3, 0xA4, 0x45, 0xB1, 0xF8, 0x57,
          0x72, 0xA3, 0x60, 0x08, 0xC4, 0x44, 0x22, 0x89
        })        
        || hash.SequenceEqual(new byte[] {
          // e.1.0.9
          0x1a,0xb6,0xf4,0xca,0xac,0xb6,0x6a,0x88,
          0x93,0xf4,0xde,0x2b,0x5b,0xa2,0x4a,0x45,
          0x64,0xc6,0x26,0x37,0x69,0x7c,0x03,0x7c,
          0xf7,0x53,0x85,0xfc,0x14,0x54,0x5d,0x72
        });
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(ref int __result, MobileParty party, StatExplainer explanation) {
      var perk = ActivePatch._perk;
      if (!(party.LeaderHero?.GetPerkValue(perk) ?? false))
        return;

      var extra = party.LeaderHero.Clan.Settlements.Count() * perk.PrimaryBonus;
      if (extra > 0) {
        var explainedNumber = new ExplainedNumber(__result, explanation);
        explainedNumber.Add(party.LeaderHero.Clan.Settlements.Count() * perk.PrimaryBonus, perk.Name);
        __result = (int) explainedNumber.ResultNumber;
      }
    }

  }

}