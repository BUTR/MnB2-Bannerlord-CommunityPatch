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

  public sealed class LordConversationsCampaignBehaviorPatch : IPatch {

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

    // fixed e1.0.6 ?
    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.0
        0x1B, 0x33, 0x04, 0x96, 0x22, 0xAB, 0x44, 0x73,
        0x64, 0x0B, 0x00, 0xD4, 0x55, 0xD4, 0xB8, 0x7F,
        0xD3, 0xE6, 0xDB, 0x0B, 0x96, 0x20, 0x52, 0x72,
        0xB7, 0x45, 0xF1, 0x0F, 0x8C, 0x51, 0x48, 0x46
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