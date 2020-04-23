using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommunityPatch.Options;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using Module = TaleWorlds.MountAndBlade.Module;
using Path = System.IO.Path;

namespace CommunityPatch.Patches {

  [UsedImplicitly]
  [HarmonyPatch(typeof(EscapeMenuVM), MethodType.Constructor, typeof(IEnumerable<EscapeMenuItemVM>), typeof(TextObject))]
  public static class GroupEscapeMenuOptionsPatch {

    public static FieldInfo EscapeMenuItemVmOnExecute = typeof(EscapeMenuItemVM)
      .GetField("_onExecute", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public static FieldInfo EscapeMenuItemVmIdentifier = typeof(EscapeMenuItemVM)
      .GetField("_identifier", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static readonly object _groupEscMenuOptsKey = new object();

    public static void Postfix(EscapeMenuVM __instance, ref MBBindingList<EscapeMenuItemVM> ____menuItems, IEnumerable<EscapeMenuItemVM> items, TextObject title = null) {
      if (CommunityPatchSubModule.Options.DontGroupThirdPartyMenuOptions) {
        ____menuItems.Add(new EscapeMenuItemVM(new TextObject("{=CommunityPatchOptions}Community Patch Options"),
          _ => CommunityPatchSubModule.Current.ShowOptions(), _groupEscMenuOptsKey, false));
        return;
      }

      var list = ____menuItems.ToList();

      var customOptions = new List<EscapeMenuItemVM>();
      for (var i = 0; i < list.Count; i++) {
        var item = list[i];

        try {
          var act = (Action<object>) EscapeMenuItemVmOnExecute.GetValue(item);
          var actAsm = act.Method.DeclaringType?.Assembly;
          var optAsmName = actAsm?.GetName().Name;

          if (optAsmName == null
            || optAsmName.StartsWith("TaleWorlds.")
            || optAsmName.StartsWith("SandBox.")
            || optAsmName.StartsWith("SandBoxCore.")
            || optAsmName.StartsWith("StoryMode.")) {
            if (PathHelpers.IsOfficialAssembly(actAsm))
              continue;
          }
        }
        catch {
          // yeah, it's 3rd party.
        }

        customOptions.Add(item);
        list[i] = null;
      }

      var newList = new MBBindingList<EscapeMenuItemVM>();

      foreach (var item in list) {
        if (item != null)
          newList.Add(item);
      }

      if (customOptions.Count <= 0) {
        newList.Insert(newList.Count - 2, new EscapeMenuItemVM(new TextObject("{=CommunityPatchOptions}Community Patch Options"),
          _ => CommunityPatchSubModule.Current.ShowOptions(), _groupEscMenuOptsKey, false));

        ____menuItems = newList;
        return;
      }

      newList.Insert(newList.Count - 2, new EscapeMenuItemVM(new TextObject("{=MoreOptions}More Options"), _ => {
        var options = new List<InquiryElement>();

        foreach (var item in customOptions) {
          options.Add(new InquiryElement(item, item.ActionText, null, !item.IsDisabled, null));
        }

        options.Add(new InquiryElement(_groupEscMenuOptsKey, new TextObject("{=CommunityPatchOptions}Community Patch Options").ToString(), null));

        InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
          new TextObject("{=MoreOptions}More Options").ToString(),
          null,
          options,
          true,
          true,
          new TextObject("{=Open}Open").ToString(),
          null,
          selection => {
            var picked = selection.FirstOrDefault()?.Identifier;
            if (picked == _groupEscMenuOptsKey) {
              CommunityPatchSubModule.Current.ShowOptions();
              return;
            }

            if (picked is EscapeMenuItemVM vm)
              SynchronizationContext.Current.Post(_ => {
                vm.ExecuteAction();
              }, null);
          },
          null
        ), true);
      }, "MoreOptions", false));

      ____menuItems = newList;
    }

  }

}