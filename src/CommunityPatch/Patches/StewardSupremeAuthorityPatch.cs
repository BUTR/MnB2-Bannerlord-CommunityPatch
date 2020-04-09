using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  internal class StewardSupremeAuthorityPatch : PatchBase<StewardSupremeAuthorityPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultClanPoliticsModel).GetMethod("CalculateInfluenceChangeInternal", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(StewardSupremeAuthorityPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private PerkObject _perk;

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "SFjspNSf");
    
    public override bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
          // e.1.0.7
          0x92, 0xC7, 0xF7, 0xE5, 0x5E, 0xC8, 0xEB, 0xE2,
          0x6A, 0x25, 0xAD, 0x3F, 0x95, 0x79, 0xD6, 0xA0,
          0x66, 0x8B, 0x9A, 0x50, 0x37, 0xDD, 0x65, 0xAC,
          0x01, 0x3E, 0xEF, 0x02, 0x78, 0x11, 0x80, 0xD0
        })
        || hash.SequenceEqual(new byte[] {
          // e.1.0.8
          0xF5, 0x76, 0xD7, 0x2B, 0x9A, 0x8E, 0x79, 0xB8,
          0x3A, 0xAE, 0xD5, 0xCF, 0xCE, 0xD4, 0x28, 0x14,
          0xA5, 0x1B, 0xB0, 0x68, 0x47, 0xD7, 0xF0, 0xA5,
          0x4E, 0xFD, 0x48, 0x33, 0x32, 0xDF, 0x2F, 0x5E
        });
    }

    public override void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));

      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    private static void Postfix(Clan clan, ref ExplainedNumber influenceChange) {
      var perk = ActivePatch._perk;
      
      var ruler = clan?.Kingdom?.Ruler;
      var leader = clan?.Leader;
      if (ruler != null && ruler == leader && ruler.GetPerkValue(perk)) {
        influenceChange.Add(perk.PrimaryBonus, perk.Name);
      }
    }
  }

}