using System;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public abstract class InfluenceGainEachDaySubPatch<TPatch> : PerkPatchBase<TPatch>, IInfluenceGainEachDaySubPatch where TPatch : InfluenceGainEachDaySubPatch<TPatch> {

    protected InfluenceGainEachDaySubPatch(string perkId) : base(perkId) { }

    protected InfluenceGainEachDaySubPatch(string perkId, Func<PerkObject, bool> disambiguation) : base(perkId, disambiguation) { }

    public abstract void AddInfluenceGainBonus(Clan clan, ref ExplainedNumber influenceChange);

  }

}