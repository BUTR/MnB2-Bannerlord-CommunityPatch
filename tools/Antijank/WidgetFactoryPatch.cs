using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.PrefabSystem;

namespace Antijank {

  public static class WidgetFactoryPatch {

    private static readonly MethodInfo PrefabExtensionRegisterAttributeTypesMethod = AccessTools.Method(typeof(PrefabExtension), "RegisterAttributeTypes");

    private static readonly MethodInfo GetPrefabNamesAndPathsFromCurrentPathMethod = AccessTools.Method(typeof(WidgetFactory), "GetPrefabNamesAndPathsFromCurrentPath");

    private static readonly Type CustomWidgetTypeType = Type.GetType("TaleWorlds.GauntletUI.PrefabSystem.CustomWidgetType, TaleWorlds.GauntletUI.PrefabSystem, Version=1.0.0.0, Culture=neutral");

    private static readonly ConstructorInfo CustomWidgetTypeCtor = AccessTools.Constructor(
      CustomWidgetTypeType,
      new[] {typeof(WidgetFactory), typeof(string), typeof(string)});

    private static readonly FieldInfo CustomWidgetTypeResourcesPathField = AccessTools.Field(CustomWidgetTypeType, "_resourcesPath");

    static WidgetFactoryPatch()
      => Context.Harmony.Patch(
        AccessTools.Method(typeof(WidgetFactory), "Initialize"),
        new HarmonyMethod(typeof(WidgetFactoryPatch), nameof(InitializeReplacement))
      );

    public static void Init() {
      // runs static initializer
    }

    public static bool InitializeReplacement(WidgetFactory __instance, Dictionary<string, Type> ____builtinTypes, object ____customTypes) {
      foreach (var prefabExtension in __instance.PrefabExtensionContext.PrefabExtensions)
        PrefabExtensionRegisterAttributeTypesMethod.Invoke(prefabExtension, new object[] {__instance.WidgetAttributeContext});

      foreach (var incoming in WidgetInfo.CollectWidgetTypes()) {
        if (____builtinTypes.ContainsKey(incoming.Name)) {
          var existing = ____builtinTypes[incoming.Name];
          var incomingAsmQualifiedName = incoming.AssemblyQualifiedName;
          var existingAsmQualifiedName = existing.AssemblyQualifiedName;
          var incomingIsMod = PathHelpers.IsModuleAssembly(incoming.Assembly, out var incomingMod);
          var existingIsMod = PathHelpers.IsModuleAssembly(existing.Assembly, out var existingMod);
          var result = MessageBox.Error(
            $"Two GUI widget components have the same symbol, {incoming.Name}.\n\n" +
            $"Incoming: {incomingAsmQualifiedName}\n"
            + $" - {(incomingIsMod ? incomingMod.Name : "Not from a mod")}\n\n"
            + $"Existing: {existingAsmQualifiedName}\n"
            + $" - {(existingIsMod ? existingMod.Name : "Not from a mod")}\n\n"
            + $"Would you like the incoming widget to overwrite the existing widget?",
            "Conflicting Widgets",
            MessageBoxType.YesNo
          );

          if (result == MessageBoxResult.Yes)
            ____builtinTypes[incoming.Name] = incoming;
        }
        else
          ____builtinTypes.Add(incoming.Name, incoming);
      }

      var cwtDict = (IDictionary) ____customTypes;
      var prefabs = (Dictionary<string, string>) GetPrefabNamesAndPathsFromCurrentPathMethod.Invoke(__instance, null);
      foreach (var keyValuePair in prefabs) {
        var incomingName = keyValuePair.Key;
        var incomingPath = keyValuePair.Value;
        var incomingCwt = CustomWidgetTypeCtor.Invoke(new object[] {__instance, incomingPath, incomingName});
        if (cwtDict.Contains(incomingName)) {
          var existingCwt = cwtDict[incomingName];
          var existingPath = (string) CustomWidgetTypeResourcesPathField.GetValue(existingCwt);
          var incomingIsMod = PathHelpers.IsModulePath(incomingPath, out var incomingMod);
          var existingIsMod = PathHelpers.IsModulePath(existingPath, out var existingMod);
          var result = MessageBox.Error(
            $"Two GUI widget components have the same symbol, {incomingName}.\n\n" +
            $"Incoming: {incomingPath}\n"
            + $" - {(incomingIsMod ? incomingMod.Name : "Not from a mod")}\n\n"
            + $"Existing: {existingPath}\n"
            + $" - {(existingIsMod ? existingMod.Name : "Not from a mod")}\n\n"
            + $"Would you like the incoming widget to overwrite the existing widget?",
            "Conflicting Widgets",
            MessageBoxType.YesNo
          );

          if (result == MessageBoxResult.Yes)
            cwtDict[incomingName] = incomingCwt;
        }
        else
          cwtDict.Add(incomingName, incomingCwt);
      }

      return false;
    }

  }

}