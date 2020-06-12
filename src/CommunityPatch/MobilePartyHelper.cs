using System;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatch {

  [PublicAPI]
  public static class MobilePartyHelper {

    public static bool IsInTerrainType(this MobileParty mobileParty, TerrainType terrainType) {
      var faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
      return faceTerrainType == terrainType;
    }

    public static bool IsInSnowyTerrain(this MobileParty mobileParty) {
      var atmosphereModel =
        Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, mobileParty.GetPosition());
      return atmosphereModel.SnowInfo.Density > float.Epsilon;
    }
    
    public static bool IsInHillTerrain(this MobileParty mobileParty) {
      var partyPosition = mobileParty.GetPosition();

      var forward3dPosition = Vec2AsMapSceneVec3(new Vec2(partyPosition.x, partyPosition.y + 1));
      var backward3dPosition = Vec2AsMapSceneVec3(new Vec2(partyPosition.x, partyPosition.y - 1));
      var left3dPoint = Vec2AsMapSceneVec3(new Vec2(partyPosition.x - 1, partyPosition.y));
      var right3dPoint = Vec2AsMapSceneVec3(new Vec2(partyPosition.x + 1, partyPosition.y));

      return Math.Abs(forward3dPosition.z - backward3dPosition.z) >= 0.3f || Math.Abs(left3dPoint.z - right3dPoint.z) >= 0.3f;
    }

    private static Vec3 Vec2AsMapSceneVec3(Vec2 vec2) {
      var height = 0f;
      Campaign.Current.MapSceneWrapper.GetHeightAtPoint(vec2, ref height);
      return new Vec3(vec2, height);
    }
    
  }

}