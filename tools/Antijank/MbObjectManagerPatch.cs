using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using HarmonyLib;
using TaleWorlds.ObjectSystem;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public static class MbObjectManagerPatch {

    private static int InitCount = 0;
    
    static MbObjectManagerPatch() {
      if (InitCount > 0) {
        if (Debugger.IsAttached)
          Debugger.Break();
        throw new InvalidOperationException("Multiple static initializer runs!");
      }

      ++InitCount;
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "CreateDocumentFromXmlFile"),
        postfix: new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(CreateDocumentFromXmlFilePostfix)));
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "MergeTwoXmls"),
        new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(MergeTwoXmlsReplacement)));
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "LoadXml"),
        new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(LoadXmlReplacement)));
    }

    public static void Init() {
      // run static initializer
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateDocumentFromXmlFilePostfix(ref XmlDocument __result, string xmlPath, string xsdPath, bool forceSkipValidation) {
      var root = __result.DocumentElement;
      if (root == null)
        return;

      __result.PreserveWhitespace = false;

      var fullPath = xmlPath;
      if (fullPath.StartsWith("./") || fullPath.StartsWith("../") || fullPath.StartsWith(".\\") || fullPath.StartsWith("..\\"))
        fullPath = new Uri(Path.Combine(Environment.CurrentDirectory, xmlPath)).LocalPath;
      PathHelpers.Normalize(fullPath);

      root.SetAttribute("xmlns:xaj", AntijankXmlNamespace);
      root.SetAttribute("source", AntijankXmlNamespace, fullPath);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool MergeTwoXmlsReplacement(ref XmlDocument __result, XmlDocument xmlDocument1, XmlDocument xmlDocument2, string id) {
      var root = xmlDocument1?.DocumentElement;
      var root2 = xmlDocument2?.DocumentElement;

      if (root == null && root2 == null) {
        var newDoc = new XmlDocument {PreserveWhitespace = false};
        newDoc.AppendChild(newDoc.CreateElement(id + "s"));
        __result = newDoc;
        return false;
      }

      if (root == null) {
        __result = xmlDocument2;
        return false;
      }

      if (root2 == null) {
        __result = xmlDocument1;
        return false;
      }

      var source1 = string.Intern(root!.GetAttribute("source", AntijankXmlNamespace));
      var source2 = string.Intern(root2!.GetAttribute("source", AntijankXmlNamespace));

      foreach (XmlNode node in root.ChildNodes) {
        if (!(node is XmlElement element))
          continue;

        if (!element.HasAttribute("source", AntijankXmlNamespace))
          element.SetAttribute("source", AntijankXmlNamespace, source1);
      }

      foreach (XmlNode node in root2.ChildNodes) {
        if (node is XmlElement element)
          if (!element.HasAttribute("source", AntijankXmlNamespace))
            element.SetAttribute("source", AntijankXmlNamespace, source2);

        root.AppendChild(xmlDocument1.ImportNode(node, true));
      }

      __result = xmlDocument1;
      return false;
    }

    private static readonly Type ObjTypeRecType = typeof(MBObjectManager).GetNestedType("IObjectTypeRecord", NonPublic | Instance | Static);

    private static readonly MethodInfo ObjTypeRecElementListNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementListName");

    private static readonly MethodInfo ObjTypeRecElementNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementName");

    private static readonly MethodInfo MbObjectManagerGetPresumedObjectMethod = AccessTools.Method(typeof(MBObjectManager), "GetPresumedObject");

    private static string ObjTypeRecElementListName(object otr) => (string) ObjTypeRecElementListNameMethod.Invoke(otr, null);

    private static string ObjTypeRecElementName(object otr) => (string) ObjTypeRecElementNameMethod.Invoke(otr, null);

    private static readonly Dictionary<string, bool> ContinueWithErrorRememberChoice = new Dictionary<string, bool>();

    private static readonly string AntijankXmlNamespace = "https://antijank/";

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool LoadXmlReplacement(MBObjectManager __instance, IList ___ObjectTypeRecords, IList ___NonSerializedObjectTypeRecords, XmlDocument doc, Type typeOfGameMenusCallbacks) {
      var nodeIndex = 0;
      var found = false;
      string typeName = null;
      for (; nodeIndex < doc.ChildNodes.Count; ++nodeIndex) {
        var tmpNodeIndex = nodeIndex;
        var node = doc.ChildNodes[tmpNodeIndex];
        foreach (var otr in ___ObjectTypeRecords) {
          if (ObjTypeRecElementListName(otr) != node.Name)
            continue;

          typeName = ObjTypeRecElementName(otr);
          found = true;
          break;
        }

        if (!found) {
          foreach (var otr in ___NonSerializedObjectTypeRecords) {
            if (ObjTypeRecElementListName(otr) != node.Name)
              continue;

            typeName = ObjTypeRecElementName(otr);
            found = true;
            break;
          }
        }

        if (found)
          break;
      }

      if (!found)
        return false;

      for (var node = doc.ChildNodes[nodeIndex].ChildNodes[0]; node != null; node = node.NextSibling) {
        if (node.NodeType == XmlNodeType.Comment)
          continue;
        if (!(node is XmlElement elem))
          continue;

        var id = elem.GetAttribute("id");
        try {
          var obj = (MBObjectBase) MbObjectManagerGetPresumedObjectMethod.Invoke(__instance, new object[] {typeName, id, true});
          if (typeOfGameMenusCallbacks != null)
            obj.Deserialize(__instance, node, typeOfGameMenusCallbacks);
          else
            obj.Deserialize(__instance, node);
          obj.AfterInitialized();
        }
        catch (Exception ex) {
          var source = elem.GetAttribute("source", AntijankXmlNamespace);
          if (string.IsNullOrEmpty(source))
            source = doc.DocumentElement!.GetAttribute("source", AntijankXmlNamespace);
          var isMod = PathHelpers.IsModulePath(new Uri(source).LocalPath, out var mod);
          var rememberedChoice = ContinueWithErrorRememberChoice.TryGetValue(mod.Alias, out var rememberedContinue);

          Console.WriteLine("Failed to deserialize xml element and instantiate object.");
          Console.WriteLine($"Element ID: {id}");
          Console.WriteLine($"Source: {source}");
          Console.WriteLine($"Module: {(isMod ? $"{mod.Name} ({mod.Id})" : "Not from a mod.")}");
          Console.WriteLine();

          if (isMod && rememberedChoice && rememberedContinue)
            continue;

          if (MessageBox.Error("Failed to deserialize xml element and instantiate object.\n"
            + $"Element ID: {id}\n"
            + $"Source: {source}\n"
            + $"Module: {(isMod ? $"{mod.Name} ({mod.Id})" : "Not from a mod.")}\n\n"
            + "Would you like to continue loading without this component?",
            "Xml Loading Error",
            MessageBoxType.YesNo,
            () => {
              MessageBox.Info(ex.ToString(), "Exception Details");
            }) == MessageBoxResult.No)
            throw;

          if (!isMod || rememberedChoice)
            continue;

          if (MessageBox.Info("Continue showing deserialization failures from this mod?", "Remember Decision", MessageBoxType.YesNo) == MessageBoxResult.No)
            ContinueWithErrorRememberChoice[mod.Alias] = true;
        }
      }

      return false;
    }

  }

}