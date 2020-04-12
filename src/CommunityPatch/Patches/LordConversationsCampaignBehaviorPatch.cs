using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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
      // ReSharper disable once PossibleNullReferenceException
      = Type.GetType("SandBox.LordConversationsCampaignBehavior, SandBox, Version=1.0.0.0, Culture=neutral")
        .GetMethod("conversation_player_want_to_end_service_as_mercenary_on_condition",
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo
      = typeof(LordConversationsCampaignBehaviorPatch)
        .GetMethod(nameof(Postfix),
          BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }
    
    private static readonly byte[][] Hashes = {
      new byte[] {
        0x31, 0x81, 0x47, 0x2b, 0x6a, 0xde, 0xc8, 0x26,
        0x37, 0x68, 0xb3, 0x81, 0x0a, 0x47, 0x57, 0x51,
        0x37, 0x30, 0xa4, 0xa4, 0xb3, 0xde, 0xa7, 0x59,
        0x1a, 0x75, 0x90, 0x8a, 0x18, 0xdf, 0xa7, 0x2b
      }
    };

    public bool IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatched(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once InconsistentNaming
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Postfix(out bool __result) {
      __result = Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction
        && Hero.OneToOneConversationHero.Clan != Hero.MainHero.Clan
        && Hero.MainHero.Clan.IsUnderMercenaryService;
    }

    public void Reset() {
    }

  }

}