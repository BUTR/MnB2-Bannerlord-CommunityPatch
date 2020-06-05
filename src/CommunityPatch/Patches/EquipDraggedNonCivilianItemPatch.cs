using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static System.Reflection.BindingFlags;
using static CommunityPatch.PatchApplicabilityHelper;

namespace CommunityPatch.Patches {
  public static class EquipDraggedNonCivilianItemPatch {

    internal static readonly MethodInfo UpdateCurrentCharacterIfPossibleMethodInfo = typeof(SPInventoryVM)
      .GetMethod("UpdateCurrentCharacterIfPossible", NonPublic | Instance | DeclaredOnly);

    internal static readonly MethodInfo InitializeInventoryMethodInfo = typeof(SPInventoryVM)
      .GetMethod("InitializeInventory", NonPublic | Instance | DeclaredOnly);
    
    internal static readonly MethodInfo AfterTransferMethodInfo = typeof(SPInventoryVM)
      .GetMethod("AfterTransfer", NonPublic | Instance | DeclaredOnly);

    internal static readonly MethodInfo SetIsCivilianItemMethodInfo = typeof(SPItemVM)
      .GetMethod("set_IsCivilianItem", Public | Instance | DeclaredOnly);

    internal static readonly FieldInfo CurrentCharacterFieldInfo = typeof(SPInventoryVM)
      .GetField("_currentCharacter", NonPublic | Instance | DeclaredOnly);
    
    internal static readonly FieldInfo LeftItemListFieldInfo = typeof(SPInventoryVM)
      .GetField("_leftItemListVM", NonPublic | Instance | DeclaredOnly);
    
    internal static readonly FieldInfo RightItemListFieldInfo = typeof(SPInventoryVM)
      .GetField("_rightItemListVM", NonPublic | Instance | DeclaredOnly);
      
    internal static bool FoundAllFields = false;
    
    internal static readonly Action<SPItemVM, bool> SetIsCivilianItem;
    
    internal static readonly Func<SPInventoryVM, CharacterObject> CurrentCharacterGetter;
    
    internal static readonly Func<SPInventoryVM, MBBindingList<SPItemVM>> LeftItemListVmGetter;
    
    internal static readonly Func<SPInventoryVM, MBBindingList<SPItemVM>> RightItemListVmGetter;

    internal static void PrintCannotFindFieldError(String typeName)
      => CommunityPatchSubModule.Error
        ($"{nameof(EquipDraggedNonCivilianItemPatch)}: Couldn't find {typeName} by reflection.");
    
    static EquipDraggedNonCivilianItemPatch() {
      if (SetIsCivilianItemMethodInfo == null) {
        PrintCannotFindFieldError(nameof(SetIsCivilianItemMethodInfo));
        return;
      }
      
      if (CurrentCharacterFieldInfo == null) {
        PrintCannotFindFieldError(nameof(CurrentCharacterFieldInfo));
        return;
      }
      
      if (LeftItemListFieldInfo == null) {
        PrintCannotFindFieldError(nameof(LeftItemListFieldInfo));
        return;
      } 
      
      if (RightItemListFieldInfo == null) {
        PrintCannotFindFieldError(nameof(RightItemListFieldInfo));
        return;
      }
      
      SetIsCivilianItem = SetIsCivilianItemMethodInfo.BuildInvoker<Action<SPItemVM, bool>>(); 
      CurrentCharacterGetter = CurrentCharacterFieldInfo.BuildGetter<SPInventoryVM, CharacterObject>(); 
      LeftItemListVmGetter = LeftItemListFieldInfo.BuildGetter<SPInventoryVM, MBBindingList<SPItemVM>>(); 
      RightItemListVmGetter = RightItemListFieldInfo.BuildGetter<SPInventoryVM, MBBindingList<SPItemVM>>();
      FoundAllFields = true;
    }
    
    internal static readonly byte[][] UpdateCurrentCharacterIfPossibleHashes = {
      new byte[] {
        // e1.3.1.228624
        0x95, 0xA4, 0x67, 0xBA, 0x07, 0x7D, 0x7D, 0xCB,
        0x77, 0xD3, 0x3B, 0x9F, 0xFD, 0xDD, 0x20, 0xB3,
        0x83, 0xC2, 0xCD, 0xD0, 0xBA, 0x95, 0xCC, 0xF3,
        0x27, 0xB3, 0x3D, 0xC5, 0x90, 0x6B, 0x4D, 0x22
      },
      new byte[] {
        // e1.4.1.229965
        0x57, 0x69, 0x90, 0xAC, 0xC2, 0x34, 0x60, 0xFE,
        0xAA, 0x66, 0x9C, 0x70, 0xBE, 0xBA, 0x87, 0xCC,
        0xE1, 0x97, 0x73, 0x42, 0xDA, 0x8D, 0x76, 0xB9,
        0x32, 0xA8, 0x0A, 0xEB, 0x02, 0xC0, 0x2E, 0xB2
      }
    };
    
    internal static readonly byte[][] InitializeInventoryHashes = {
      new byte[] {
        // e1.3.1.228624
        0x64, 0x60, 0xEB, 0xE8, 0x33, 0x13, 0x5B, 0xB6,
        0x6F, 0xF2, 0x00, 0x4B, 0x86, 0x4A, 0x00, 0x36,
        0x8A, 0x5A, 0x4A, 0xB8, 0x0C, 0x63, 0x8E, 0x7C,
        0x2D, 0x20, 0xB1, 0x56, 0x65, 0x9F, 0x43, 0x38
      },
      new byte[] {
        // e1.4.1.229965
        0x09, 0x71, 0x08, 0xF9, 0x66, 0xA7, 0x3A, 0xBA,
        0x6F, 0x89, 0xAD, 0x79, 0xCD, 0x45, 0xFE, 0x93,
        0xA0, 0x1F, 0x38, 0x31, 0xB3, 0xEB, 0x22, 0x3E,
        0x2B, 0x3B, 0xA7, 0x5C, 0x5B, 0xF9, 0x27, 0xE0
      }
    };
    
    internal static readonly byte[][] AfterTransferHashes = {
      new byte[] {
        // e1.3.1.228624
        0x25, 0x15, 0x66, 0x66, 0xB9, 0x1A, 0x40, 0xFA,
        0x42, 0x17, 0x88, 0x63, 0x6D, 0x89, 0xC9, 0x05,
        0x6C, 0x2F, 0x66, 0x2E, 0x2E, 0x97, 0xBA, 0xF7,
        0x2E, 0xDA, 0x95, 0x93, 0x09, 0xE3, 0xCD, 0xC9
      },
      new byte[] {
        // e1.4.0.229335
        0x25, 0x15, 0x66, 0x66, 0xB9, 0x1A, 0x40, 0xFA,
        0x42, 0x17, 0x88, 0x63, 0x6D, 0x89, 0xC9, 0x05,
        0x6C, 0x2F, 0x66, 0x2E, 0x2E, 0x97, 0xBA, 0xF7,
        0x2E, 0xDA, 0x95, 0x93, 0x09, 0xE3, 0xCD, 0xC9
      },
      new byte[] {
        // e1.4.1.229965
        0xE2, 0x6A, 0x91, 0x63, 0xC2, 0xA0, 0x16, 0x8D,
        0x7B, 0xF3, 0x60, 0x38, 0x11, 0x23, 0x4E, 0x6F,
        0x41, 0x9D, 0xD0, 0x7F, 0x2F, 0xB8, 0xCB, 0xA8,
        0xDA, 0x57, 0x02, 0x03, 0xB9, 0xA9, 0x6C, 0x8C
      }
    };

  }

  public abstract class EquipDraggedNonCivilianItemPatch<TPatch> : PerkPatchBase<TPatch> where TPatch : EquipDraggedNonCivilianItemPatch<TPatch> {

    private static MethodInfo UpdateCurrentCharacterIfPossibleMethodInfo => EquipDraggedNonCivilianItemPatch.UpdateCurrentCharacterIfPossibleMethodInfo;
    
    private static MethodInfo InitializeInventoryMethodInfo => EquipDraggedNonCivilianItemPatch.InitializeInventoryMethodInfo;
    
    private static MethodInfo AfterTransferMethodInfo => EquipDraggedNonCivilianItemPatch.AfterTransferMethodInfo;
    
    private static Action<SPItemVM, bool> SetIsCivilianItem => EquipDraggedNonCivilianItemPatch.SetIsCivilianItem;

    private static Func<SPInventoryVM, MBBindingList<SPItemVM>> RightItemListVmGetter => EquipDraggedNonCivilianItemPatch.RightItemListVmGetter;
    
    private static Func<SPInventoryVM, MBBindingList<SPItemVM>> LeftItemListVmGetter => EquipDraggedNonCivilianItemPatch.LeftItemListVmGetter;
    
    private static Func<SPInventoryVM, CharacterObject> CurrentCharacterGetter => EquipDraggedNonCivilianItemPatch.CurrentCharacterGetter;

    private static bool FoundAllFields = EquipDraggedNonCivilianItemPatch.FoundAllFields;

    private static Action<String> PrintCannotFindFieldError = EquipDraggedNonCivilianItemPatch.PrintCannotFindFieldError; 
    
    private MethodInfo UpdateCurrentCharacterIfPossiblePrefixMethodInfo => GetType()
      .GetMethod("UpdateCurrentCharacterIfPossiblePrefix", NonPublic | Static | DeclaredOnly);
    
    private MethodInfo UpdateCurrentCharacterIfPossiblePostfixMethodInfo => GetType()
          .GetMethod("UpdateCurrentCharacterIfPossiblePostfix", NonPublic | Static | DeclaredOnly);
    
    private MethodInfo InitializeInventoryPostfixMethodInfo => GetType()
      .GetMethod("InitializeInventoryPostfix", NonPublic | Static | DeclaredOnly);
    
    private MethodInfo AfterTransferPostfixMethodInfo => GetType()
      .GetMethod("AfterTransferPostfix", NonPublic | Static | DeclaredOnly);

    private static byte[][] UpdateCurrentCharacterIfPossibleHashes => EquipDraggedNonCivilianItemPatch.UpdateCurrentCharacterIfPossibleHashes;
    
    private static byte[][] InitializeInventoryHashes => EquipDraggedNonCivilianItemPatch.InitializeInventoryHashes;
    
    private static byte[][] AfterTransferHashes => EquipDraggedNonCivilianItemPatch.AfterTransferHashes;

    protected EquipDraggedNonCivilianItemPatch(string perkId) : base(perkId) {
    }
    
    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return UpdateCurrentCharacterIfPossibleMethodInfo;
      yield return InitializeInventoryMethodInfo;
      yield return AfterTransferMethodInfo;
    }
    
    public override bool Applied { get; protected set; }

    public override void Apply(Game game) {
      if (Applied) return;
      
      CommunityPatchSubModule.Harmony.Patch(InitializeInventoryMethodInfo, postfix: new HarmonyMethod(InitializeInventoryPostfixMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(AfterTransferMethodInfo, postfix: new HarmonyMethod(AfterTransferPostfixMethodInfo));
      CommunityPatchSubModule.Harmony.Patch(UpdateCurrentCharacterIfPossibleMethodInfo,
        prefix: new HarmonyMethod(UpdateCurrentCharacterIfPossiblePrefixMethodInfo),
        postfix: new HarmonyMethod(UpdateCurrentCharacterIfPossiblePostfixMethodInfo));

      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      if (!FoundAllFields) {
        return false;
      }
      if (UpdateCurrentCharacterIfPossibleMethodInfo == null) {
        PrintCannotFindFieldError(nameof(UpdateCurrentCharacterIfPossibleMethodInfo));
        return false;
      }
      
      if (InitializeInventoryMethodInfo == null) {
        PrintCannotFindFieldError(nameof(InitializeInventoryMethodInfo));
        return false;
      }
      
      if (AfterTransferMethodInfo == null) {
        PrintCannotFindFieldError(nameof(AfterTransferMethodInfo));
        return false;
      }

      if (UpdateCurrentCharacterIfPossiblePrefixMethodInfo == null) {
        PrintCannotFindFieldError(nameof(UpdateCurrentCharacterIfPossiblePrefixMethodInfo));
        return false;
      }
        
      if (UpdateCurrentCharacterIfPossiblePostfixMethodInfo == null) {
        PrintCannotFindFieldError(nameof(UpdateCurrentCharacterIfPossiblePostfixMethodInfo));
        return false;
      } 
        
      if (InitializeInventoryPostfixMethodInfo == null) {
        PrintCannotFindFieldError(nameof(InitializeInventoryPostfixMethodInfo));
        return false;
      }
      
      if (AfterTransferPostfixMethodInfo == null) {
        PrintCannotFindFieldError(nameof(InitializeInventoryPostfixMethodInfo));
        return false;
      }

      if (!IsTargetPatchable(UpdateCurrentCharacterIfPossibleMethodInfo, UpdateCurrentCharacterIfPossibleHashes)) {
        return false;
      }
      
      if (!IsTargetPatchable(InitializeInventoryMethodInfo, InitializeInventoryHashes)) {
        return false;
      }
      
      if (!IsTargetPatchable(AfterTransferMethodInfo, AfterTransferHashes)) {
        return false;
      }

      return base.IsApplicable(game);
    }

    private static void MapLeftAndRightInventory(SPInventoryVM spInventoryVm, Action<SPItemVM> itemMapper) {
      var rightItemListVm = RightItemListVmGetter(spInventoryVm);
      var leftItemListVm =  LeftItemListVmGetter(spInventoryVm);

      if (rightItemListVm == null || leftItemListVm == null) {
        return;
      }
      
      leftItemListVm.ApplyActionOnAllItems(itemMapper);
      rightItemListVm.ApplyActionOnAllItems(itemMapper);
    }

    private static void NonCivilianPerkItemsEquipableState(SPInventoryVM spInventoryVm, Func<SPItemVM, bool> isItemApplicableToPerk, bool state) {

      Action<SPItemVM> makeCivilianItemUnequipable = item => {
        var isCivilian = item?.ItemRosterElement.EquipmentElement.Item?.IsCivilian ?? false;
        if (isItemApplicableToPerk(item) && !isCivilian) {
          SetIsCivilianItem(item, state);
        }
      };
      
      MapLeftAndRightInventory(spInventoryVm, makeCivilianItemUnequipable);
    }

    private static void MakeNonCivilianPerkItemsUnequipable(SPInventoryVM spInventoryVm, Func<SPItemVM, bool> isItemApplicableToPerk)
      => NonCivilianPerkItemsEquipableState(spInventoryVm, isItemApplicableToPerk, false);

    private static void MakeNonCivilianPerkItemsEquipable(SPInventoryVM spInventoryVm, Func<SPItemVM, bool> isItemApplicableToPerk)
      => NonCivilianPerkItemsEquipableState(spInventoryVm, isItemApplicableToPerk, true);
    
    private static bool CharacterHasPerk(CharacterObject characterObject) {
      return characterObject != null && characterObject.GetPerkValue(ActivePatch.Perk);
    }

    private static void FlipEquipableStateOfNonCivilianPerkItemsIfCharacterHasPerkOrNot(SPInventoryVM spInventoryVm, CharacterObject previousCharacter, Func<SPItemVM, bool> isItemApplicableToPerk) {
      if (CharacterHasPerk(previousCharacter)) {
        MakeNonCivilianPerkItemsEquipable(spInventoryVm, isItemApplicableToPerk);
      }
      else {
        MakeNonCivilianPerkItemsUnequipable(spInventoryVm, isItemApplicableToPerk);
      }
    }
    
    protected static void FlipEquipableStateOfNonCivilianPerkItemsIfDisplayedCharacterHasPerkOrNot(SPInventoryVM spInventoryVm, Func<SPItemVM, bool> isItemApplicableToPerk) {
      var currentCharacter = CurrentCharacterGetter(spInventoryVm);
      if (currentCharacter != null) {
        FlipEquipableStateOfNonCivilianPerkItemsIfCharacterHasPerkOrNot(spInventoryVm, currentCharacter, isItemApplicableToPerk);
      }
    }
    
    protected static void StorePreviousCharacter(SPInventoryVM spInventoryVm, out CharacterObject currentCharacter) =>
      currentCharacter = CurrentCharacterGetter(spInventoryVm);

    protected static void KeepEquipableStateOfNonCivilianPerkItemsIfPreviousAndDisplayedCharacterHavePerkOrNot(SPInventoryVM spInventoryVm, CharacterObject previousCharacter, Func<SPItemVM, bool> isItemApplicableToPerk) {
      var displayedCharacter = CurrentCharacterGetter(spInventoryVm);
      var displayedCharacterHasPerk = CharacterHasPerk(displayedCharacter);
      
      // If the previous character in the inventory has the perk, then the non civilian items should not be updated if the new shown character also has the perk.
      // Non civilian items should not be updated if both characters do not have the perk.
      // If the player has no other hero in its army, then the previous and displayed character will be the same character. This use case falls into the previous case.
      if (previousCharacter == null || displayedCharacter == null || !(CharacterHasPerk(previousCharacter) ^ displayedCharacterHasPerk)) {
        return;
      }
      FlipEquipableStateOfNonCivilianPerkItemsIfCharacterHasPerkOrNot(spInventoryVm, displayedCharacter, isItemApplicableToPerk);
    }

  }

}