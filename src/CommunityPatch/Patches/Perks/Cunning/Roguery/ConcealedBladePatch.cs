using Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;


namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class ConcealedBladePatch : EquipDraggedNonCivilianItemPatch<ConcealedBladePatch> {

    private static bool IsOneHandedWeapon(SPItemVM item) {
      var itemType = item?.ItemRosterElement.EquipmentElement.Item?.ItemType;
      
      if (itemType == null) {
        return false;
      }
      
      return itemType == ItemObject.ItemTypeEnum.OneHandedWeapon;
    }
    
    private static void UpdateCurrentCharacterIfPossiblePrefix(SPInventoryVM __instance, out CharacterObject __state)
      => StorePreviousCharacter(__instance, out __state); 

    private static void UpdateCurrentCharacterIfPossiblePostfix(SPInventoryVM __instance, CharacterObject __state)
      => KeepEquipableStateOfNonCivilianPerkItemsIfPreviousAndDisplayedCharacterHavePerkOrNot(__instance, __state, IsOneHandedWeapon);

    private static void InitializeInventoryPostfix(SPInventoryVM __instance)
      => FlipEquipableStateOfNonCivilianPerkItemsIfDisplayedCharacterHasPerkOrNot(__instance, IsOneHandedWeapon);
    
    private static void AfterTransferPostfix(SPInventoryVM __instance)
      => FlipEquipableStateOfNonCivilianPerkItemsIfDisplayedCharacterHasPerkOrNot(__instance, IsOneHandedWeapon);

    public ConcealedBladePatch() : base("uW7jNpaT") {
    }

  }

}