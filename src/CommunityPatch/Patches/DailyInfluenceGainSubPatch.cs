using System;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Patches {

  public abstract class DailyInfluenceGainSubPatch<TPatch> : PerkPatchBase<TPatch>, IDailyInfluenceGain where TPatch : DailyInfluenceGainSubPatch<TPatch> {

    protected DailyInfluenceGainSubPatch(string perkId) : base(perkId) { }

    protected DailyInfluenceGainSubPatch(string perkId, Func<PerkObject, bool> disambiguation) : base(perkId, disambiguation) { }

    public abstract void ModifyDailyInfluenceGain(Clan clan, ref ExplainedNumber influenceChange);

  }

}