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
      },
      new byte[] {
        // e1.4.1.229326
        0xFC, 0xF9, 0xD0, 0x71, 0xB7, 0x05, 0xB5, 0xE6,
        0x7D, 0x1F, 0x92, 0x7B, 0x26, 0xBA, 0x19, 0xFF,
        0xEA, 0x4D, 0xD7, 0x8F, 0xD8, 0x90, 0x6F, 0xD3,
        0x13, 0x76, 0x89, 0x42, 0xA2, 0xBE, 0x60, 0x30
      },
      new byte[] {
        // e1.4.2.231233
        0x9B, 0x61, 0xE9, 0x19, 0x98, 0x3B, 0x19, 0xB1,
        0x8C, 0x54, 0xC4, 0x34, 0xBC, 0x32, 0x2B, 0x0D,
        0xAD, 0x4B, 0x4E, 0x06, 0x2E, 0x72, 0x1B, 0x1C,
        0x34, 0xD9, 0xF9, 0x37, 0xE4, 0xF5, 0xBF, 0xB7
      }
    };

    public static readonly byte[][] ConstructorHashes = {
      new byte[] {
        // e1.4.0.228616
        0x87, 0x52, 0xC1, 0x34, 0xEF, 0x61, 0xAD, 0x8F,
        0x2B, 0x80, 0x23, 0x1A, 0xCC, 0x60, 0xE3, 0x14,
        0xCA, 0xB0, 0xC8, 0xEE, 0x49, 0x74, 0xD9, 0x3F,
        0x50, 0x3A, 0xAD, 0x88, 0x24, 0x13, 0xB8, 0xFE
      },
      new byte[] {
        // e1.4.1.229326
        0xB6, 0x45, 0x26, 0x0E, 0xC2, 0xB9, 0x82, 0xB0,
        0xCD, 0xB8, 0xEF, 0xBB, 0xD5, 0x4D, 0xEE, 0x9A,
        0xE9, 0xDE, 0x12, 0xAC, 0xE4, 0xC2, 0xE5, 0x66,
        0x43, 0x7F, 0x34, 0xF1, 0x13, 0xE0, 0xC9, 0xB0
      },
      new byte[] {
        // e1.4.2.231233
        0xBC, 0xD3, 0x28, 0xA9, 0x8A, 0x2B, 0x7F, 0x09,
        0xD3, 0x9D, 0xDB, 0xBA, 0x0E, 0x4E, 0xA1, 0x8D,
        0xE6, 0xFC, 0xB4, 0x53, 0xD2, 0xD4, 0x59, 0x0F,
        0x37, 0x0B, 0x1C, 0x63, 0x81, 0xD5, 0xE8, 0xB4
      }
    };

    public TramplerPatch2() : base("GKlmIYik") {
    }

    public override bool? IsApplicable(Game game) {
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

      try {
        if (Perk != null)
          return true;
      }
      catch {
        return false;
      }

      return base.IsApplicable(game);
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