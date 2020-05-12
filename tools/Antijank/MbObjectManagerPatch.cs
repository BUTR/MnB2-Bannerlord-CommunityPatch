using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using HarmonyLib;
using static System.Reflection.BindingFlags;

namespace Antijank {

  public static class MbObjectManagerPatch {

    private static readonly Type MbObjectMgrType
      = Type.GetType("TaleWorlds.ObjectSystem.MBObjectManager, TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral", false)
      ?? Type.GetType("TaleWorlds.Core.MBObjectManager, TaleWorlds.Core, Version=1.0.0.0, Culture=neutral", true);

    private static readonly Type MbObjectBaseType
      = Type.GetType("TaleWorlds.ObjectSystem.MBObjectBase, TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral", false)
      ?? Type.GetType("TaleWorlds.Core.MBObjectBase, TaleWorlds.Core, Version=1.0.0.0, Culture=neutral", true);


    private static readonly Type ObjTypeRecType = MbObjectMgrType.GetNestedType("IObjectTypeRecord", NonPublic | Instance | Static);

    private static readonly MethodInfo ObjTypeRecElementListNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementListName");

    private static readonly MethodInfo ObjTypeRecElementNameMethod = AccessTools.PropertyGetter(ObjTypeRecType, "ElementName");

    //private static string ObjTypeRecElementListName(object otr) => (string) ObjTypeRecElementListNameMethod.Invoke(otr, null);
    private static readonly Func<object, string> ObjTypeRecElementListName
      = ObjTypeRecElementListNameMethod.BuildInvoker<Func<object, string>>();

    //private static string ObjTypeRecElementName(object otr) => (string) ObjTypeRecElementNameMethod.Invoke(otr, null);
    private static readonly Func<object, string> ObjTypeRecElementName
      = ObjTypeRecElementNameMethod.BuildInvoker<Func<object, string>>();

    private static readonly MethodInfo MbObjectManagerGetPresumedObjectMethod = AccessTools.Method(MbObjectMgrType, "GetPresumedObject");
    
    private static readonly MethodInfo MbObjectBaseDeserializeMethod1
      = MbObjectBaseType.GetMethod("Deserialize", Public | NonPublic | Instance | DeclaredOnly, null, new[] {MbObjectMgrType, typeof(XmlNode)}, null);

    private static readonly MethodInfo MbObjectBaseDeserializeMethod2
      = MbObjectBaseType.GetMethod("Deserialize", Public | NonPublic | Instance | DeclaredOnly, null, new[] {MbObjectMgrType, typeof(XmlNode), typeof(Type)}, null);

    private static readonly MethodInfo MbObjectBaseAfterInitializedMethod
      = MbObjectBaseType.GetMethod("AfterInitialized", Public | NonPublic | Instance | DeclaredOnly);

    private static readonly Action<object, object, XmlNode> MbObjectBaseDeserialize1
      = MbObjectBaseDeserializeMethod1.BuildInvoker<Action<object, object, XmlNode>>();

    private static readonly Action<object, object, XmlNode, Type> MbObjectBaseDeserialize2
      = MbObjectBaseDeserializeMethod2.BuildInvoker<Action<object, object, XmlNode, Type>>();

    private static readonly Action<object> MbObjectBaseAfterInitialized
      = MbObjectBaseAfterInitializedMethod.BuildInvoker<Action<object>>();

    private static readonly Func<object, string, string, bool, object> MbObjectManagerGetPresumedObject
      = MbObjectManagerGetPresumedObjectMethod.BuildInvoker<Func<object, string, string, bool, object>>();

    private static int InitCount = 0;

    static MbObjectManagerPatch() {
      if (InitCount > 0) {
        if (Debugger.IsAttached)
          Debugger.Break();
        throw new InvalidOperationException("Multiple static initializer runs!");
      }

      ++InitCount;

      Context.Harmony.Patch(AccessTools.Method(MbObjectMgrType, "CreateDocumentFromXmlFile"),
        postfix: new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(CreateDocumentFromXmlFilePostfix)));
      Context.Harmony.Patch(AccessTools.Method(MbObjectMgrType, "MergeTwoXmls"),
        new HarmonyMethod(typeof(MbObjectManagerPatch), nameof(MergeTwoXmlsReplacement)));
      Context.Harmony.Patch(AccessTools.Method(MbObjectMgrType, "LoadXml"),
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

    private static readonly Dictionary<string, bool> ContinueWithErrorRememberChoice = new Dictionary<string, bool>();

    private static readonly string AntijankXmlNamespace = "https://antijank/";

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool LoadXmlReplacement(object __instance, IList ___ObjectTypeRecords, IList ___NonSerializedObjectTypeRecords, XmlDocument doc, Type typeOfGameMenusCallbacks) {
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
          // (MBObjectBase)
          var obj = MbObjectManagerGetPresumedObject(__instance, typeName, id, true);
          if (typeOfGameMenusCallbacks != null)
            MbObjectBaseDeserialize2(obj, __instance, node, typeOfGameMenusCallbacks);
          else
            MbObjectBaseDeserialize1(obj, __instance, node);
          MbObjectBaseAfterInitialized(obj);
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