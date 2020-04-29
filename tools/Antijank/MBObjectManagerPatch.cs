using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public static class MbObjectManagerPatch {

    static MbObjectManagerPatch() {
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "CreateDocumentFromXmlFile"),
        postfix: new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(CreateDocumentFromXmlFilePostfix)));
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "MergeTwoXmls"),
        new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(MergeTwoXmlsReplacement)));
      Context.Harmony.Patch(AccessTools.Method(typeof(MBObjectManager), "LoadXml"),
        prefix: new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(LoadXmlReplacement)));
    }

    public static void Init() {
      // run static initializer
    }

    private static void CreateDocumentFromXmlFilePostfix(ref XmlDocument __result, string xmlPath, string xsdPath, bool forceSkipValidation) {
      var root = __result.DocumentElement;
      if (root == null)
        return;

      var fullPath = xmlPath;
      if (fullPath.StartsWith("./") || fullPath.StartsWith("../") || fullPath.StartsWith(".\\") || fullPath.StartsWith("..\\"))
        fullPath = new Uri(Path.Combine(Environment.CurrentDirectory, xmlPath)).LocalPath;
      PathHelpers.Normalize(fullPath);

      root.SetAttribute("xmlns:xaj", "https://antijank/");
      root.SetAttribute("source", "https://antijank/", fullPath);
    }

    private static bool MergeTwoXmlsReplacement(ref XmlDocument __result, XmlDocument xmlDocument1, XmlDocument xmlDocument2, string id) {
      var newDoc = new XmlDocument();

      var root = (XmlElement) newDoc.ImportNode(xmlDocument1.DocumentElement!, false);
      newDoc.AppendChild(root);

      root.SetAttribute("xmlns:xaj", "https://antijank/");

      root.RemoveAttribute("source", "https://antijank/");
      root.SetAttribute("id", "https://antijank/", id);

      newDoc.AppendChild(root);

      if (xmlDocument1.GetNamespaceOfPrefix("xaj") == null)
        throw new NotImplementedException();
      if (xmlDocument2.GetNamespaceOfPrefix("xaj") == null)
        throw new NotImplementedException();

      var src1 = xmlDocument1.DocumentElement!.GetAttribute("source", "https://antijank/");
      var src2 = xmlDocument2.DocumentElement!.GetAttribute("source", "https://antijank/");

      foreach (XmlNode node in xmlDocument1.DocumentElement.ChildNodes) {
        var copy = newDoc.ImportNode(node, true);
        if (copy is XmlElement copyElement) {
          if (string.IsNullOrEmpty(copyElement.GetAttribute("source", "https://antijank/")))
            copyElement.SetAttribute("source", "https://antijank/", src1);
        }

        newDoc.DocumentElement!.AppendChild(copy);
      }

      foreach (XmlNode node in xmlDocument2.DocumentElement.ChildNodes) {
        var copy = newDoc.ImportNode(node, true);
        if (copy is XmlElement copyElement) {
          if (string.IsNullOrEmpty(copyElement.GetAttribute("source", "https://antijank/")))
            copyElement.SetAttribute("source", "https://antijank/", src2);
        }

        newDoc.DocumentElement!.AppendChild(copy);
      }

      __result = newDoc;
      return false;
    }

    private static readonly Type ObjTypeRecType = typeof(MBObjectManager).GetNestedType("IObjectTypeRecord", NonPublic | Instance | Static);

    private static readonly MethodInfo ObjTypeRecElementListNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementListName");

    private static readonly MethodInfo ObjTypeRecElementNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementName");

    private static readonly MethodInfo MBObjectManagerGetPresumedObjectMethod = AccessTools.Method(typeof(MBObjectManager), "GetPresumedObject");

    private static string ObjTypeRecElementListName(object otr) => (string) ObjTypeRecElementListNameMethod.Invoke(otr, null);

    private static string ObjTypeRecElementName(object otr) => (string) ObjTypeRecElementNameMethod.Invoke(otr, null);

    private static readonly Dictionary<string, bool> ContinueWithErrorRememberChoice = new Dictionary<string, bool>();

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
          var obj = (MBObjectBase) MBObjectManagerGetPresumedObjectMethod.Invoke(__instance, new object[] {typeName, id, true});
          if (typeOfGameMenusCallbacks != null)
            obj.Deserialize(__instance, node, typeOfGameMenusCallbacks);
          else
            obj.Deserialize(__instance, node);
          obj.AfterInitialized();
        }
        catch (Exception ex) {
          var source = elem.GetAttribute("source", "https://antijank/");
          if (string.IsNullOrEmpty(source))
            source = doc.DocumentElement!.GetAttribute("source", "https://antijank/");
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