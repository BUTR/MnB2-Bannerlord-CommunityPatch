using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Tactics {

  public class OneStepAheadPatch : PerkPatchBase<OneStepAheadPatch> {

    public override bool Applied { get; protected set; }

    private static readonly Type MapNavigationHandlerType = Type.GetType("SandBox.View.Map.MapNavigationHandler, SandBox.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

    private static readonly MethodInfo TargetMethodInfo = MapNavigationHandlerType?.GetMethod("get_PartyEnabled", Public | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(OneStepAheadPatch).GetMethod(nameof(Postfix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.2.1.226961
        0x77, 0x65, 0x41, 0xC8, 0x63, 0xEC, 0xA9, 0x04,
        0x8C, 0x06, 0xDB, 0xEF, 0x05, 0x97, 0xE0, 0x66,
        0x61, 0x20, 0x7D, 0xBC, 0x1C, 0xA8, 0xB5, 0x87,
        0x88, 0x5F, 0xE5, 0x30, 0xC4, 0xF4, 0xB6, 0x8C
      }
    };

    private static readonly Func<object, bool> MapNavHandlerPartyActiveGetter = MapNavigationHandlerType?.GetMethod("get_PartyActive", Public | Instance | DeclaredOnly)
      ?.BuildInvoker<Func<object, bool>>();

    public OneStepAheadPatch() : base("V6mvBGDV") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (TargetMethodInfo == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, postfix: new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Postfix(ref bool __result, ref object __instance) {
      if (__result) return;
      if (MapNavHandlerPartyActiveGetter(__instance)) return;
      if (Hero.MainHero.HeroState == Hero.CharacterStates.Prisoner) return;
      if (MobileParty.MainParty.MapEvent == null) return;

      var perk = ActivePatch.Perk;
      __result = Hero.MainHero.GetPerkValue(perk);
    }

  }

}