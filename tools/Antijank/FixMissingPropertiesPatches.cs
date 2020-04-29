using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Antijank {

  public class FixMissingPropertiesPatches {

    static FixMissingPropertiesPatches() {
      // ItemObject
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(ItemObject), nameof(ItemObject.ItemCategory)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ItemObjectGetItemCategoryPostfix)));
      // CharacterObject
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(CharacterObject), nameof(CharacterObject.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(CharacterObjectGetNamePostfix)));
      // BasicCultureObject
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(BasicCultureObject), nameof(BasicCultureObject.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(BasicCultureObjectGetNamePostfix)));
      // BuildingType
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(BuildingType), nameof(BuildingType.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(BuildingTypeGetNamePostfix)));
      // Clan
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(Clan), nameof(Clan.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ClanGetNamePostfix)));
      // IssueEffect
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(IssueEffect), nameof(IssueEffect.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(IssueEffectGetNamePostfix)));
      // Kingdom
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(Kingdom), nameof(Kingdom.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(KingdomGetNamePostfix)));
      // Settlement
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(Settlement), nameof(Settlement.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(SettlementGetNamePostfix)));
      // ItemObject
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(ItemObject), nameof(ItemObject.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ItemObjectGetNamePostfix)));
      // ItemModifier
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(ItemModifier), nameof(ItemModifier.Name)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ItemModifierGetNamePostfix)));
    }

    public static void Init() {
      // run static initializer
    }

    public static void MissingPropertyNotice(string msg, string caption, string continueMsg, ref bool? dontShow) {
      Console.WriteLine(msg);

      if (dontShow == true)
        return;

      MessageBox.Error(msg, caption);

      if (dontShow != null)
        return;

      if (MessageBox.Info(continueMsg, "Remember Decision", MessageBoxType.YesNo) == MessageBoxResult.No)
        dontShow = true;
    }

    private delegate string DescDlg<in TInstance>(TInstance inst);

    private delegate TResult FixDlg<in TInstance, out TResult>(TInstance inst);

    private static readonly Dictionary<MemberInfo, bool> DontShowError = new Dictionary<MemberInfo, bool>();

    private static void GenericPostfix<TInstance, TResult>(MemberInfo member, TInstance inst, ref TResult result, DescDlg<TInstance> desc, FixDlg<TInstance, TResult> fix) {
      if (result != null)
        return;

      result = fix(inst);

      var fieldDesc = $"{typeof(TInstance).Name}.{typeof(TResult).Name}";

      var msg = $"Missing {fieldDesc}: {desc(inst)}";
      Console.WriteLine(msg);

      var askedDontShow = DontShowError.TryGetValue(member, out var dontShowError);

      if (dontShowError)
        return;

      MessageBox.Error(msg, $"Missing {fieldDesc}");

      if (askedDontShow)
        return;

      if (MessageBox.Info($"Continue showing missing {fieldDesc} errors?", "Remember Decision", MessageBoxType.YesNo) == MessageBoxResult.No)
        DontShowError[member] = true;
      else
        DontShowError[member] = false;
    }

    public static void ItemObjectGetItemCategoryPostfix(MethodBase __originalMethod, ItemObject __instance, ref ItemCategory __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, _ => DefaultItemCategories.Unassigned);

    public static void CharacterObjectGetNamePostfix(MethodBase __originalMethod, ItemObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void BasicCultureObjectGetNamePostfix(MethodBase __originalMethod, BasicCultureObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void BuildingTypeGetNamePostfix(MethodBase __originalMethod, BuildingType __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void ClanGetNamePostfix(MethodBase __originalMethod, Clan __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void IssueEffectGetNamePostfix(MethodBase __originalMethod, IssueEffect __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void KingdomGetNamePostfix(MethodBase __originalMethod, Kingdom __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void SettlementGetNamePostfix(MethodBase __originalMethod, Settlement __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void ItemObjectGetNamePostfix(MethodBase __originalMethod, ItemObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    public static void ItemModifierGetNamePostfix(MethodBase __originalMethod, ItemModifier __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

  }

}