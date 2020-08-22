#if !AFTER_E1_4_3

using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace CommunityPatch.Behaviors {

  public class CommunityPatchCampaignBehavior : CampaignBehaviorBase {

    private static readonly AccessTools.FieldRef<MobileParty, int> PartyPureSpeedLastCheckVersionField
      = AccessTools.FieldRefAccess<MobileParty,int>("_partyPureSpeedLastCheckVersion");

    private PerkObject _perk;

    public PerkObject Perk => _perk ??= PerkObjectHelpers.Load("PB5iowxh");
    public override void RegisterEvents()
      => CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, OnPerkOpened);

    private void OnPerkOpened(Hero hero, PerkObject openedPerk) {
      if (hero != null && openedPerk == Perk && hero.PartyBelongedTo != null)
        PartyPureSpeedLastCheckVersionField(hero.PartyBelongedTo) = -1;
    }

    public override void SyncData(IDataStore dataStore) {
    }

  }

}

#endif