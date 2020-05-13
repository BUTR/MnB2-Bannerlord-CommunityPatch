using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchNotBefore(ApplicationVersionType.EarlyAccess, 1, 4)]
  public sealed class TramplerPatch2 : PerkPatchBase<TramplerPatch2> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo OverloadedTargetMethodInfo = typeof(Mission).GetMethod("GetAttackCollisionResults", Public | Static | DeclaredOnly);

    [CanBeNull]
    private static readonly Type AttackInformationType = Type.GetType("TaleWorlds.MountAndBlade.AttackInformation, TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral", false);

    private static readonly ConstructorInfo TargetConstructorInfo = AttackInformationType?.GetConstructor(new[] {
      typeof(Agent), typeof(Agent), typeof(GameEntity), typeof(AttackCollisionData).MakeByRefType()
    });

    private static readonly MethodInfo UpdateCharacterPostfixPatchMethodInfo = typeof(TramplerPatch2).GetMethod(nameof(UpdateCorrectCharacterForHorseChargeDamagePostfix), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo UpdateHorseDamagePostfixPatchMethodInfo = typeof(TramplerPatch2).GetMethod(nameof(UpdateHorseDamagePostfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetConstructorInfo;
      yield return OverloadedTargetMethodInfo;
    }

    public static readonly byte[][] OverloadedTargetHashes = {
      new byte[] {
        // e1.4.0.228531
        0x7F, 0xB8, 0x39, 0x7E, 0x72, 0x45, 0x53, 0x96,
        0x11, 0x95, 0x24, 0x60, 0x34, 0xDF, 0x07, 0x95,
        0x6F, 0xFB, 0xF6, 0xD5, 0x97, 0x30, 0x33, 0x4F,
        0x90, 0x4E, 0x8B, 0x8C, 0x14, 0xB1, 0xE6, 0xC7
      }
    };

    public static readonly byte[][] ConstructorHashes = {
      new byte[] {
        // e1.4.0.228616
        0x87, 0x52, 0xC1, 0x34, 0xEF, 0x61, 0xAD, 0x8F,
        0x2B, 0x80, 0x23, 0x1A, 0xCC, 0x60, 0xE3, 0x14,
        0xCA, 0xB0, 0xC8, 0xEE, 0x49, 0x74, 0xD9, 0x3F,
        0x50, 0x3A, 0xAD, 0x88, 0x24, 0x13, 0xB8, 0xFE
      }
    };

    public TramplerPatch2() : base("GKlmIYik") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) {
        return false;
      }
      
      if (OverloadedTargetMethodInfo == null)
        return false;

      if (AlreadyPatchedByOthers(Harmony.GetPatchInfo(OverloadedTargetMethodInfo)))
        return false;

      if (!OverloadedTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(OverloadedTargetHashes))
        return false;

      if (TargetConstructorInfo == null)
        return false;

      if (AlreadyPatchedByOthers(Harmony.GetPatchInfo(TargetConstructorInfo)))
        return false;

      if (!TargetConstructorInfo.MakeCilSignatureSha256().MatchesAnySha256(ConstructorHashes))
        return false;

      return true;
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetConstructorInfo,
        postfix: new HarmonyMethod(UpdateCharacterPostfixPatchMethodInfo));

      CommunityPatchSubModule.Harmony.Patch(OverloadedTargetMethodInfo,
        postfix: new HarmonyMethod(UpdateHorseDamagePostfixPatchMethodInfo));

      Applied = true;
    }

    private static readonly FieldInfo AttackerAgentCharacterFieldInfo = AttackInformationType?.GetField("AttackerAgentCharacter", Public | Instance | DeclaredOnly);

    [CanBeNull]
    private static readonly FieldRef<IntPtr, BasicCharacterObject> AttackerAgentCharacter
      = AttackerAgentCharacterFieldInfo == null ? null : AttackerAgentCharacterFieldInfo.BuildRef<IntPtr, BasicCharacterObject>();

    private static unsafe void UpdateCorrectCharacterForHorseChargeDamagePostfix(ref byte __instance, Agent attackerAgent, ref AttackCollisionData attackCollisionData) {
      var p = (IntPtr) Unsafe.AsPointer(ref __instance);
      if (attackCollisionData.IsHorseCharge && attackerAgent.RiderAgent?.Character != null && p != default)
        AttackerAgentCharacter!(p) = attackerAgent.RiderAgent.Character;
    }

    private static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk)
      => (character as CharacterObject)?.GetPerkValue(perk) ?? false;

    private int TramplerDamageModifier(int baseDamage) {
      var tramplerDamage = baseDamage * (1f + Perk.PrimaryBonus);
      return (int) Math.Round(tramplerDamage, MidpointRounding.AwayFromZero);
    }

    private static unsafe void UpdateHorseDamagePostfix(ref byte attackInformation, ref AttackCollisionData attackCollisionData, ref CombatLogData combatLog) {
      var p = (IntPtr) Unsafe.AsPointer(ref attackInformation);
      if (!(attackCollisionData.IsHorseCharge && p != default && HeroHasPerk(AttackerAgentCharacter!(p), ActivePatch.Perk)))
        return;

      combatLog.InflictedDamage = ActivePatch.TramplerDamageModifier(combatLog.InflictedDamage);
      attackCollisionData.InflictedDamage = ActivePatch.TramplerDamageModifier(attackCollisionData.InflictedDamage);
    }

  }

}