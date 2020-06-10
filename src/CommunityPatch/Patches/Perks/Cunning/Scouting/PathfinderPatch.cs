using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches.Perks.Cunning.Scouting {
  public sealed class PathfinderPatch : PartySpeedSubPatch<PathfinderPatch> {
    public PathfinderPatch() : base("d2qGHXyx") { }

    protected override bool IsPerkConditionFulfilled(MobileParty mobileParty, float baseSpeed, ExplainedNumber finalSpeed)
      => Campaign.Current.IsDay;

  }
}
