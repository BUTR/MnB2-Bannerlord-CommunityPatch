using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;

namespace CommunityPatch {

  public static class PlayerEncounterHelper {
    private static readonly Type PlayerEncounterType = typeof(PlayerEncounter);
    private static readonly Type MobilePartyType = typeof(MobileParty);
    private static readonly Type CampaignType = typeof(Campaign);
    private static readonly FieldInfo EncounteredParty = PlayerEncounterType.GetField("_encounteredParty", NonPublic | Instance | DeclaredOnly);
    private static readonly PropertyInfo CampaignMobilePartyLocator = CampaignType.GetProperty("MobilePartyLocator", NonPublic | Instance | DeclaredOnly);
    private static readonly MethodInfo MobilePartyCanAttack = MobilePartyType.GetMethod("CanAttack", NonPublic | Instance | DeclaredOnly);
    public static readonly MethodInfo TargetMethodInfo = PlayerEncounterType.GetMethod(nameof(PlayerEncounter.FindPartiesWhoWillJoinEvent), Public | Instance | DeclaredOnly);

    public static readonly byte[][] TargetHashes = {
      new byte[] {
        // e1.2.1.226961
        0xAD, 0xB4, 0x55, 0x5E, 0xCD, 0xC4, 0xE6, 0x5E,
        0x14, 0x60, 0x91, 0x82, 0xFD, 0xB5, 0xE2, 0x5C,
        0x65, 0xF0, 0x0D, 0x10, 0xDA, 0x79, 0x96, 0x7C,
        0xBC, 0x50, 0x29, 0x94, 0xF0, 0xB5, 0xE7, 0x76
      }
    };
    
    public static PartyBase GetEncounteredParty(PlayerEncounter encounter)
      => (PartyBase) EncounteredParty.GetValue(encounter);
    
    public static bool CanPartyAttack(MobileParty party, MobileParty target)
      => (bool) MobilePartyCanAttack.Invoke(party, new object[] {target});
    
    public static IEnumerable<MobileParty> FindPartiesAroundPosition(Vec2 position2D, float radius) {
      var locator = CampaignMobilePartyLocator.GetValue(Campaign.Current);
      var locatorType = locator.GetType();
      var findPartiesMethod = locatorType.GetMethod("FindPartiesAroundPosition", NonPublic | Instance | DeclaredOnly, null, new[] {typeof(Vec2), typeof(float)}, new ParameterModifier[] { }); 
      var results = findPartiesMethod?.Invoke(locator, new object[] {position2D, radius});
      return (IEnumerable<MobileParty>) results;
    }
  }

}