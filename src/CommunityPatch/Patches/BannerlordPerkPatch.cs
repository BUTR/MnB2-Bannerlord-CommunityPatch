using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches {

  class BannerlordPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(PartyBase).GetMethod("get_PartySizeLimit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(BannerlordPatch).GetMethod(nameof(PartySizeLimitPatched), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        null,
        new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetCilBytes();
      if (bytes == null) return false;

      var hash = bytes.GetSha256();
      return hash.SequenceEqual(new byte[] {
        0xAE, 0xF7, 0x29, 0x0C, 0x6D, 0x5D, 0xFD, 0xE2,
        0x4D, 0x32, 0x24, 0x35, 0x1D, 0x18, 0x3D, 0x8E,
        0x40, 0x5E, 0xD3, 0xDA, 0x11, 0xC4, 0x31, 0x92,
        0x6B, 0x75, 0xAA, 0xB5, 0xEC, 0x3B, 0x9F, 0x2F
      });
    }

    private static void PartySizeLimitPatched(PartyBase __instance, ref int __result) {
      __result += BannerlordPerkExtra(__instance.LeaderHero);
    }

    public static int BannerlordPerkExtra(Hero hero) {
      if (hero == null || !hero.GetPerkValue(DefaultPerks.Steward.Bannerlord))
        return 0;

      return hero.Clan.Settlements.Count() * 2;
    }
    
    public void Reset() {}

  }

}