using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CommunityPatch {

  [PublicAPI]
  public static class MobilePartyHelper {

    public static bool IsInTerrainType(this MobileParty mobileParty, TerrainType terrainType) {
      var faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
      return faceTerrainType == terrainType;
    }
    
  }

}