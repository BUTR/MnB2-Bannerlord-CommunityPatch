using System;

namespace Antijank.Interop {

  [Flags]
  public enum FileDialogOptions : uint {

    OverwritePrompt = 1u << 1,

    StrictFileTypes = 1u << 2,

    NoChangeDir = 1u << 3,

    PickFolders = 1u << 5,

    ForceFileSystem = 1u << 6,

    AllNonStorageItems = 1u << 7,

    NoValidate = 1u << 8,

    AllowMultiselect = 1u << 9,

    PathMustExist = 1u << 11,

    FileMustExist = 1u << 12,

    CreatePrompt = 1u << 13,

    ShareAware = 1u << 14,

    NoReadOnlyReturn = 1u << 15,

    NoTestFileCreate = 1u << 16,

    HideMruPlaces = 1u << 17,

    HidePinnedPlaces = 1u << 18,

    NoDereferenceLinks = 1u << 20,

    OkButtonNeedsInteraction = 1u << 21,

    DontAddToRecent = 1u << 25,

    ForceShowHidden = 1u << 28,

    DefaultNoMiniMode = 1u << 29,

    ForcePreviewPaneOn = 1u << 30,

    SupportStreamableItems = 1u << 31

  }

}