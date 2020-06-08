using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

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
      },
      new byte[] {
        // e1.2.1.227732
        0x66, 0x9E, 0x30, 0x3E, 0x20, 0xE3, 0x96, 0xD9,
        0x33, 0x7B, 0x6E, 0xCE, 0x13, 0xF8, 0x58, 0x1F,
        0x0A, 0xEA, 0x88, 0x0F, 0xF5, 0x9B, 0x3E, 0x59,
        0xFD, 0x35, 0x49, 0xCD, 0x12, 0x36, 0x49, 0xF3
      },
      new byte[] {
        // e1.3.0.227640
        0x83, 0xC6, 0xCA, 0xA2, 0x42, 0xD9, 0x77, 0x85,
        0xF7, 0x6D, 0x66, 0x52, 0x09, 0x60, 0x4F, 0x72,
        0x44, 0x61, 0x92, 0x30, 0x8A, 0x6B, 0x33, 0x2C,
        0xE4, 0x3E, 0x2E, 0x45, 0x38, 0x90, 0x36, 0xCD
      },
      new byte[] {
        // e1.4.0.228531
        0xDC, 0x6F, 0x5A, 0x4E, 0xB0, 0x69, 0xC6, 0x28,
        0xEB, 0x5B, 0xC5, 0x6E, 0xA3, 0xAC, 0x94, 0xF7,
        0x4B, 0x2D, 0x29, 0xDA, 0xE7, 0x9B, 0x60, 0xE8,
        0x5A, 0xB7, 0x57, 0x19, 0xBB, 0x11, 0xB3, 0x9A
      },
      new byte[] {
        // e1.4.1.229326
        0xBD, 0x24, 0x06, 0x05, 0x57, 0xEB, 0x9E, 0xA7,
        0x03, 0x04, 0x5A, 0x8D, 0x08, 0x62, 0x4E, 0x59,
        0xAE, 0x86, 0x8F, 0x7B, 0xEE, 0xF6, 0x6D, 0x38,
        0x95, 0x65, 0x34, 0x0B, 0x71, 0x9C, 0x4E, 0xDF
      },
      new byte[] {
        // e1.4.2.231233
        0xE6, 0x12, 0x1B, 0xFA, 0x2A, 0x1E, 0x53, 0x62,
        0x8C, 0xC9, 0x52, 0xC2, 0x71, 0xFF, 0x52, 0x27,
        0xD1, 0x24, 0xFC, 0xD4, 0xE8, 0x95, 0xAB, 0xAA,
        0x2E, 0xAC, 0xC0, 0x82, 0xA2, 0x37, 0x2C, 0xD2
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