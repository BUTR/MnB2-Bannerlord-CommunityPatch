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
      },
      new byte[] {
        // e1.3.0.227640
        0x6E, 0xFA, 0x83, 0xD2, 0x0F, 0x3C, 0xE2, 0x7D,
        0x52, 0xE8, 0xF6, 0x8A, 0x31, 0x74, 0xAD, 0xFA,
        0xBB, 0xC0, 0x13, 0xD5, 0xBB, 0x97, 0x9C, 0x21,
        0x21, 0x90, 0x8A, 0x51, 0x2C, 0x39, 0xE4, 0x96
      },
      new byte[] {
        // e1.4.1.229326
        0x0A, 0xBE, 0x61, 0xF1, 0x55, 0x36, 0x54, 0x07,
        0x98, 0xA1, 0xA9, 0xCB, 0xDF, 0x92, 0x28, 0x3E,
        0x54, 0xB5, 0x37, 0x1C, 0x21, 0xB8, 0x78, 0x8D,
        0xCF, 0xA9, 0x80, 0xE1, 0x67, 0x08, 0x38, 0x86
      },
      new byte[] {
        // e1.4.3.237794
        0x62, 0xB3, 0xC3, 0x30, 0x7C, 0x11, 0x2F, 0xD0,
        0x94, 0xDF, 0x30, 0x4A, 0x88, 0x59, 0x9F, 0x5A,
        0x08, 0xE1, 0x8D, 0x1B, 0x34, 0xDA, 0xA7, 0x60,
        0x13, 0x53, 0x69, 0xC3, 0x47, 0xD4, 0x4C, 0xF2
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