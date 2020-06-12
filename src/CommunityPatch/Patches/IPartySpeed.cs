using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {
  public interface IPartySpeed {
    void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed);
    
  }
}
