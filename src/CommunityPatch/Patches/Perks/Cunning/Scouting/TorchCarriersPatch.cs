using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class TorchCarriersPatch : PartySpeedSubPatch<TorchCarriersPatch> {
    public TorchCarriersPatch() : base("JJm1ZmPx") { }

    protected override bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed)
      => Campaign.Current.IsNight;
  }
}
