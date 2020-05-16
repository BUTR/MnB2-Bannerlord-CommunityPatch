using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {
  public interface IPartySpeedSubPatch : IPatch {
    PerkObject Perk { get; }

    void ModifyFinalSpeed(MobileParty mobileParty, float baseSpeed, ref ExplainedNumber finalSpeed);
  }
}
