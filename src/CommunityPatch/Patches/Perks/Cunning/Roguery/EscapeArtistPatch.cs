using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class EscapeArtistPatch : PatchBase<EscapeArtistPatch> {
    
    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultPlayerCaptivityModel).GetMethod("CheckTimeElapsedMoreThanHours", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static readonly MethodInfo PatchMethodInfo = typeof(EscapeArtistPatch).GetMethod(nameof(Prefix), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    private PerkObject _perk;

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xC0, 0xD9, 0x79, 0x81, 0x79, 0xBE, 0x1B, 0x3B,
        0x81, 0x35, 0xF5, 0x25, 0xD7, 0x7D, 0xE9, 0xA4,
        0x59, 0x37, 0xC7, 0xDA, 0x7E, 0x71, 0xE4, 0xF8,
        0xC5, 0x5D, 0x78, 0x49, 0x93, 0x3F, 0x7E, 0x27
      }
    };

    public override void Reset()
      => _perk = PerkObject.FindFirst(x => x.Name.GetID() == "hJZDOSQ0");

    public override bool? IsApplicable(Game game) {
      if (_perk == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      var textObjStrings = TextObject.ConvertToStringList(
        new List<TextObject> {
          _perk.Name,
          _perk.Description
        }
      );
      
      _perk.Initialize(
        textObjStrings[0],
        textObjStrings[1],
        _perk.Skill,
        (int) _perk.RequiredSkillValue,
        _perk.AlternativePerk,
        _perk.PrimaryRole, 0.3f,
        _perk.SecondaryRole, _perk.SecondaryBonus,
        SkillEffect.EffectIncrementType.AddFactor
      );
      
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }
    
    // ReSharper disable once RedundantAssignment
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Prefix(ref bool __result, CampaignTime eventBeginTime, float hoursToWait) {
      var perk = ActivePatch._perk;
      var elapsedHoursUntilNow = eventBeginTime.ElapsedHoursUntilNow;
      var randomNumber = PlayerCaptivity.RandomNumber;
      var perkReduction = 1f - (Hero.MainHero.GetPerkValue(perk) ? perk.PrimaryBonus : 0f);
      
      var totalHoursToWait = hoursToWait * (0.5 + randomNumber) * perkReduction;
      __result = totalHoursToWait < elapsedHoursUntilNow;
      
      return false;
    }
  }
}