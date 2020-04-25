using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public static class RaidingHelper {

    private const float RaidMinHitDamage = 0.05f;

    private const float PredictionForHitMinDamage = 0.049f;

    public static readonly FieldInfo IsFinishCalled = typeof(MapEvent).GetField("_isFinishCalled", NonPublic | Instance | DeclaredOnly);

    public static readonly FieldInfo NextSettlementDamage = typeof(MapEvent).GetField("_nextSettlementDamage", NonPublic | Instance | DeclaredOnly);

    public static readonly PropertyInfo SettlementHitPoints = typeof(Settlement).GetProperty("SettlementHitPoints", NonPublic | Public | Instance | DeclaredOnly);

    public static readonly MethodInfo TargetMethodInfo = typeof(MapEvent).GetMethod("Update", NonPublic | Instance | DeclaredOnly);

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225190
        0xCD, 0xFE, 0x93, 0xE9, 0x4B, 0xC8, 0x7E, 0x94,
        0xA1, 0x7A, 0xA5, 0x16, 0x39, 0xEF, 0xC1, 0xD6,
        0x88, 0x09, 0x15, 0x13, 0xF7, 0xCB, 0x1C, 0x6C,
        0x8D, 0xE1, 0x1A, 0x96, 0x71, 0x39, 0x59, 0xE6
      }
    };

    public static bool IsNotRaidingEvent(MapEvent __instance) {
      if (IsFinished(__instance)) return true;
      if (__instance.DiplomaticallyFinished) return true;
      if (__instance.EventType != MapEvent.BattleTypes.Raid) return true;
      if (__instance.DefenderSide.TroopCount > 0) return true;

      return __instance.AttackerSide.LeaderParty?.MobileParty == null;
    }

    public static bool IsTheRaidHitNotHappeningNow(MapEvent __instance, out float damageAccumulated) {
      var damage = ((float) Math.Sqrt(__instance.AttackerSide.TroopCount) + 5f) / 500f * (float) CampaignTime.DeltaTime.ToHours;
      var resistance = __instance.MapEventSettlement.SettlementHitPoints > 0.75f ? 2f : 1f;
      var realDamage = damage / resistance;
      damageAccumulated = GetDamageAccumulated(__instance) + realDamage;
      return damageAccumulated < PredictionForHitMinDamage;
    }

    public static void SetHitDamage(MapEvent mapEvent, float hitDamage)
      => NextSettlementDamage.SetValue(mapEvent, Math.Max(hitDamage, RaidingHelper.RaidMinHitDamage));

    public static void IncreaseSettlementHitPoints(MapEvent mapEvent, float extraHitPoints) {
      var currentHp = (float) SettlementHitPoints.GetValue(mapEvent.MapEventSettlement);
      SettlementHitPoints.SetValue(mapEvent.MapEventSettlement, currentHp + extraHitPoints);
    }

    private static bool IsFinished(MapEvent mapEvent)
      => (bool) IsFinishCalled.GetValue(mapEvent);

    private static float GetDamageAccumulated(MapEvent mapEvent)
      => (float) NextSettlementDamage.GetValue(mapEvent);

  }

}