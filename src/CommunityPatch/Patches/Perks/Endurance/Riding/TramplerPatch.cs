using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static System.Reflection.BindingFlags;
using static System.Reflection.Emit.OpCodes;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  [PatchObsolete(ApplicationVersionType.EarlyAccess, 1, 4)]
  public sealed class TramplerPatch : PerkPatchBase<TramplerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Mission).GetMethod("GetAttackCollisionResults", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo OverloadedTargetMethodInfo = typeof(Mission).GetMethod("GetAttackCollisionResults", Public | Static | DeclaredOnly);

    private static readonly MethodInfo TranspilerPatchMethodInfo = typeof(TramplerPatch).GetMethod(nameof(Transpiler), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PostfixPatchMethodInfo = typeof(TramplerPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return OverloadedTargetMethodInfo;
    }

    public static readonly byte[][] TargetHashes = {
      new byte[] {
        // e1.2.1.227410
        0xC0, 0xBC, 0x9E, 0xDE, 0xAF, 0xAB, 0xFE, 0xE0,
        0x21, 0x89, 0xAE, 0x21, 0xB5, 0xA4, 0x97, 0x58,
        0xE8, 0x48, 0xE4, 0x75, 0xF0, 0x58, 0xE8, 0xC5,
        0x86, 0x06, 0xC1, 0xE8, 0x6D, 0x42, 0x9A, 0xAA
      },
      new byte[] {
        // e1.3.0.227640
        0x70, 0x1B, 0x59, 0xFB, 0xB5, 0x40, 0x50, 0x56,
        0xA3, 0x07, 0x20, 0x1C, 0x7F, 0xCB, 0x8F, 0xA4,
        0x3A, 0xC5, 0x8A, 0x34, 0xD2, 0x4E, 0x5F, 0x6B,
        0xFF, 0x69, 0x3B, 0x64, 0x5C, 0xEC, 0xEB, 0x9D
      }
    };

    public static readonly byte[][] OverloadedTargetHashes = {
      new byte[] {
        // e1.2.1.227410
        0x3C, 0x34, 0x56, 0x68, 0xCE, 0xD5, 0x86, 0x1B,
        0x38, 0x81, 0xCA, 0x3C, 0x8B, 0xB3, 0xF9, 0xE9,
        0xDF, 0x22, 0x86, 0xB0, 0xCA, 0xBF, 0xCB, 0xAF,
        0xAA, 0xCF, 0x99, 0xCD, 0x4A, 0x4D, 0xB6, 0x2C
      },
      new byte[] {
        // e1.3.0.227640
        0x11, 0xB1, 0x3A, 0x8B, 0x32, 0xB9, 0x1D, 0xDA,
        0xF0, 0x27, 0x7B, 0xAB, 0xB0, 0x59, 0x4E, 0x44,
        0xF7, 0x3C, 0x00, 0xDB, 0x37, 0x65, 0x02, 0xCB,
        0x83, 0xC7, 0x03, 0x7B, 0xDB, 0x17, 0x1A, 0xCF
      }
    };

    public TramplerPatch() : base("GKlmIYik") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) {
        return false;
      }
      
      if (TargetMethodInfo == null)
        return false;

      if (OverloadedTargetMethodInfo == null)
        return false;

      if (AlreadyPatchedByOthers(Harmony.GetPatchInfo(TargetMethodInfo)))
        return false;

      if (!TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(TargetHashes))
        return false;

      if (AlreadyPatchedByOthers(Harmony.GetPatchInfo(OverloadedTargetMethodInfo)))
        return false;

      if (!OverloadedTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(OverloadedTargetHashes))
        return false;

      if (CharacterLocalVariableInfos.Count != 2) {
        CommunityPatchSubModule.Error
          ($"{nameof(TramplerPatch)}: Expected two BasicCharacterObject local variable instances in the original method.");
        return false;
      }

      return true;
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        transpiler: new HarmonyMethod(TranspilerPatchMethodInfo));

      CommunityPatchSubModule.Harmony.Patch(OverloadedTargetMethodInfo,
        postfix: new HarmonyMethod(PostfixPatchMethodInfo));
      Applied = true;
    }

    private static readonly MethodInfo GetCharacterMethod = typeof(Agent)
      .GetMethod("get_Character", Public | Instance | DeclaredOnly);

    private static bool IsAttackerAgentAssignedAndAgentRiderNotNull(List<CodeInstruction> instructions, int start)
      => start + 4 < instructions.Count
        && instructions[start].opcode == Ldarg_1
        && instructions[start + 1].opcode == Callvirt
        && instructions[start + 1].operand is MethodInfo mi && mi == GetCharacterMethod
        && instructions[start + 2].opcode == Stloc_S
        && instructions[start + 3].opcode == Ldloc_S
        && instructions[start + 4].opcode == Brfalse_S;

    private static BasicCharacterObject UpdateCorrectCharacterForHorseChargeDamage(Agent agent, ref AttackCollisionData acd, BasicCharacterObject character)
      => acd.IsHorseCharge && agent.RiderAgent != null ? agent.RiderAgent.Character : character;

    private static readonly MethodInfo CorrectCharacterForHorseChargeDamageMethod = typeof(TramplerPatch)
      .GetMethod("UpdateCorrectCharacterForHorseChargeDamage", NonPublic | Static | DeclaredOnly);

    private static readonly List<LocalVariableInfo> CharacterLocalVariableInfos = TargetMethodInfo
      .GetMethodBody()!.LocalVariables.Where(var => var.LocalType == typeof(BasicCharacterObject)).ToList();

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
      var codes = instructions.ToList();
      for (var index = 0; index < codes.Count - 5; index++) {
        if (!IsAttackerAgentAssignedAndAgentRiderNotNull(codes, index))
          continue;

        var attackerCharacterLocalVariableIndex = CharacterLocalVariableInfos[1].LocalIndex;
        codes.InsertRange(index + 5, new List<CodeInstruction> {
          new CodeInstruction(Ldarg_1), // Load attackerAgent argument
          new CodeInstruction(Ldarg_S, 5), // Load attackCollisionData argument
          new CodeInstruction(Ldloc_S, attackerCharacterLocalVariableIndex), // Load attackerCharacter local variable
          new CodeInstruction(Call, CorrectCharacterForHorseChargeDamageMethod), // Update attackerCharacter if the collision is a horse charge
          new CodeInstruction(Stloc_S, attackerCharacterLocalVariableIndex) // Store value in attackerCharacter
        });
        return codes.AsEnumerable();
      }

      CommunityPatchSubModule.Error($"{nameof(TramplerPatch)}: Could not find the starting point to add new instructions in {TargetMethodInfo}." + Environment.NewLine);
      return codes.AsEnumerable();
    }

    private static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk)
      => (character as CharacterObject)?.GetPerkValue(perk) ?? false;

    private int TramplerDamageModifier(int baseDamage) {
      var tramplerDamage = baseDamage * (1f + Perk.PrimaryBonus);
      return (int) Math.Round(tramplerDamage, MidpointRounding.AwayFromZero);
    }

    private static void Postfix(BasicCharacterObject attackerAgentCharacter, ref AttackCollisionData attackCollisionData, ref CombatLogData combatLog) {
      if (!(attackCollisionData.IsHorseCharge && HeroHasPerk(attackerAgentCharacter, ActivePatch.Perk))) {
        return;
      }

      combatLog.InflictedDamage = ActivePatch.TramplerDamageModifier(combatLog.InflictedDamage);
      attackCollisionData.InflictedDamage = ActivePatch.TramplerDamageModifier(attackCollisionData.InflictedDamage);
    }

  }

}