using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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
      // Concept
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(Concept), nameof(Concept.Title)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ConceptTitlePostfix)));
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(Concept), nameof(Concept.Description)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(ConceptDescriptionPostfix)));
      // SkeletonScale
      Context.Harmony.Patch(AccessTools.PropertyGetter(typeof(SkeletonScale), nameof(SkeletonScale.BoneNames)),
        postfix: new HarmonyMethod(typeof(FixMissingPropertiesPatches), nameof(SkeletonScaleBoneNamesPostfix)));
    }

    public static void Init() {
      // run static initializer
    }

    private delegate string DescDlg<in TInstance>(TInstance inst);

    private delegate TResult FixDlg<in TInstance, out TResult>(TInstance inst);

    private static readonly Dictionary<MemberInfo, bool> DontShowError = new Dictionary<MemberInfo, bool>();

    private static readonly MethodInfo ItemObjDetermineItemCatMethod = AccessTools.Method(typeof(ItemObject), nameof(ItemObject.DetermineItemCategoryForItem));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ItemObjectGetItemCategoryPostfix(MethodBase __originalMethod, ItemObject __instance, ref ItemCategory __result) {
      if (__result == null && new StackFrame(2, false).GetMethod() == ItemObjDetermineItemCatMethod)
        return;

      GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId,
        item => Game.Current?.BasicModels?.ItemCategorySelector?.GetItemCategoryForItem(item)
          ?? DefaultItemCategories.Unassigned);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void CharacterObjectGetNamePostfix(MethodBase __originalMethod, ItemObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void BasicCultureObjectGetNamePostfix(MethodBase __originalMethod, BasicCultureObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void BuildingTypeGetNamePostfix(MethodBase __originalMethod, BuildingType __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ClanGetNamePostfix(MethodBase __originalMethod, Clan __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void IssueEffectGetNamePostfix(MethodBase __originalMethod, IssueEffect __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void KingdomGetNamePostfix(MethodBase __originalMethod, Kingdom __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SettlementGetNamePostfix(MethodBase __originalMethod, Settlement __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ItemObjectGetNamePostfix(MethodBase __originalMethod, ItemObject __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ItemModifierGetNamePostfix(MethodBase __originalMethod, ItemModifier __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ConceptTitlePostfix(MethodBase __originalMethod, Concept __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ConceptDescriptionPostfix(MethodBase __originalMethod, Concept __instance, ref TextObject __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => new TextObject($"{{={item.StringId}{item.StringId}}}"));

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void SkeletonScaleBoneNamesPostfix(MethodBase __originalMethod, SkeletonScale __instance, ref List<string> __result)
      => GenericPostfix((MethodInfo) __originalMethod, __instance, ref __result, item => item.StringId, item => Enumerable.Range(0, item.BoneIndices?.Length ?? 0).Select(i => $"{item.StringId}_{i}").ToList());

  }

}