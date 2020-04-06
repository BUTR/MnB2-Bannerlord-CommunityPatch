using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static CommunityPatch.HarmonyHelpers;
using Harmony = HarmonyLib.Harmony;

namespace CommunityPatch.Patches {

  public class LordConversationsCampaignBehaviorPatch : IPatch {

    public bool Applied { get; private set; }

    private static readonly MethodInfo TargetMethodInfo
      // using assembly qualified name here
      = Type.GetType("SandBox.LordConversationsCampaignBehavior, SandBox, Version=1.0.0.0, Culture=neutral")
        .GetMethod("conversation_player_want_to_end_service_as_mercenary_on_condition",
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo
      = typeof(LordConversationsCampaignBehaviorPatch)
        .GetMethod(nameof(Postfix),
          BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatched(patchInfo))
        return false;

      var bytes = TargetMethodInfo.GetMethodBody()?.GetILAsByteArray();
      if (bytes == null) return false;

      using var hasher = SHA256.Create();
      var hash = hasher.ComputeHash(bytes);
      return hash.SequenceEqual(new byte[] {
        0x31, 0x81, 0x47, 0x2b, 0x6a, 0xde, 0xc8, 0x26,
        0x37, 0x68, 0xb3, 0x81, 0x0a, 0x47, 0x57, 0x51,
        0x37, 0x30, 0xa4, 0xa4, 0xb3, 0xde, 0xa7, 0x59,
        0x1a, 0x75, 0x90, 0x8a, 0x18, 0xdf, 0xa7, 0x2b
      });
    }

    public void Apply(Game game) {
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    static void Postfix(out bool __result) {
      __result = Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction
        && Hero.OneToOneConversationHero.Clan != Hero.MainHero.Clan
        && Hero.MainHero.Clan.IsUnderMercenaryService;
    }

  }

}