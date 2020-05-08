using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class EscapeArtistPatch : PerkPatchBase<EscapeArtistPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPlayerCaptivityModel).GetMethod("CheckTimeElapsedMoreThanHours", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo = typeof(EscapeArtistPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xC0, 0xD9, 0x79, 0x81, 0x79, 0xBE, 0x1B, 0x3B,
        0x81, 0x35, 0xF5, 0x25, 0xD7, 0x7D, 0xE9, 0xA4,
        0x59, 0x37, 0xC7, 0xDA, 0x7E, 0x71, 0xE4, 0xF8,
        0xC5, 0x5D, 0x78, 0x49, 0x93, 0x3F, 0x7E, 0x27
      }
    };

    public EscapeArtistPatch() : base("hJZDOSQ0") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      Perk.Modify(0.3f, SkillEffect.EffectIncrementType.AddFactor);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    // ReSharper disable once RedundantAssignment

    public static bool Prefix(ref bool __result, CampaignTime eventBeginTime, float hoursToWait) {
      var perk = ActivePatch.Perk;
      var elapsedHoursUntilNow = eventBeginTime.ElapsedHoursUntilNow;
      var randomNumber = PlayerCaptivity.RandomNumber;
      var perkReduction = 1f - (Hero.MainHero.GetPerkValue(perk) ? perk.PrimaryBonus : 0f);

      var totalHoursToWait = hoursToWait * (0.5 + randomNumber) * perkReduction;
      __result = totalHoursToWait < elapsedHoursUntilNow;

      return false;
    }

  }

}