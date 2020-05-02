using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch {

  public static class SiegeEngineConstructionExtraDataManager {

    private static readonly ConditionalWeakTable<SiegeEvent.SiegeEngineConstructionProgress, SiegeEngineConstructionData> Links
      = new ConditionalWeakTable<SiegeEvent.SiegeEngineConstructionProgress, SiegeEngineConstructionData>();

    public static void SetMaxHitPoints(SiegeEvent.SiegeEngineConstructionProgress construction, float maxHp)
      => GetDataOrCreate(construction).MaxHitPoints = maxHp;

    public static float GetMaxHitPoints(SiegeEvent.SiegeEngineConstructionProgress construction)
      => GetDataOrCreate(construction).MaxHitPoints;

    private static SiegeEngineConstructionData GetDataOrCreate(SiegeEvent.SiegeEngineConstructionProgress construction) {
      Links.TryGetValue(construction, out var data);
      return data ?? CreateEmptyData(construction);
    }

    private static SiegeEngineConstructionData CreateEmptyData(SiegeEvent.SiegeEngineConstructionProgress construction) {
      var data = new SiegeEngineConstructionData {
        MaxHitPoints = construction.SiegeEngine.MaxHitPoints
      };
      Links.Add(construction, data);
      return data;
    }

  }

  public class SiegeEngineConstructionData {

    public float MaxHitPoints { get; set; }

  }

}