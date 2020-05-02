using System.Reflection;
using TaleWorlds.CampaignSystem;
using static System.Reflection.BindingFlags;

namespace CommunityPatch.Behaviors {

  public class CommunityPatchCampaignBehavior : CampaignBehaviorBase {

    private static readonly FieldInfo PartyPureSpeedLastCheckVersionField = typeof(MobileParty).GetField("_partyPureSpeedLastCheckVersion", Instance | NonPublic);

    private PerkObject _perk;

    public override void RegisterEvents()
      => CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, OnPerkOpened);

    private void OnPerkOpened(Hero hero, PerkObject openedPerk) {
      _perk ??= PerkObject.FindFirst(x => x.Name.GetID() == "PB5iowxh");
      if (hero != null && openedPerk == _perk)
        PartyPureSpeedLastCheckVersionField.SetValue(hero.PartyBelongedTo, -1);
    }

    public override void SyncData(IDataStore dataStore) {
    }

  }

}