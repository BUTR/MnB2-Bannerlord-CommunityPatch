
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Endurance.Riding {

  public sealed class TramplerPatch : PatchBase<TramplerPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(Mission).GetMethod("GetAttackCollisionResults", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo OverloadedTargetMethodInfo = typeof(Mission).GetMethod("GetAttackCollisionResults", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo TranspilerPatchMethodInfo = typeof(TramplerPatch).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private static readonly MethodInfo PostfixPatchMethodInfo = typeof(TramplerPatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
      yield return OverloadedTargetMethodInfo;
    }

    private static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.225664
        0xC0, 0xBC, 0x9E, 0xDE, 0xAF, 0xAB, 0xFE, 0xE0,
        0x21, 0x89, 0xAE, 0x21, 0xB5, 0xA4, 0x97, 0x58,
        0xE8, 0x48, 0xE4, 0x75, 0xF0, 0x58, 0xE8, 0xC5,
        0x86, 0x06, 0xC1, 0xE8, 0x6D, 0x42, 0x9A, 0xAA
      },
      new byte[] {
        // e1.1.0.225664
        0xE6, 0xC2, 0x12, 0xD5, 0x61, 0x66, 0xA5, 0x94,
        0x66, 0x84, 0x8E, 0x49, 0xF2, 0x5A, 0x01, 0x82,
        0xC8, 0x0E, 0x76, 0x49, 0x49, 0xE6, 0xC6, 0xBF,
        0x1C, 0xEE, 0xDF, 0x0B, 0x6C, 0x69, 0x7B, 0x7E
      },
    };

    public override void Reset() {}

    public override bool? IsApplicable(Game game)
      => TargetMethodInfo != null
        && OverloadedTargetMethodInfo != null
        && !AlreadyPatchedByOthers(Harmony.GetPatchInfo(TargetMethodInfo))
        && TargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes[0])
        && !AlreadyPatchedByOthers(Harmony.GetPatchInfo(OverloadedTargetMethodInfo))
        && OverloadedTargetMethodInfo.MakeCilSignatureSha256().MatchesAnySha256(Hashes[1]);

    public override void Apply(Game game) {
      if (Applied) return;
      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        transpiler: new HarmonyMethod(TranspilerPatchMethodInfo));

      CommunityPatchSubModule.Harmony.Patch(OverloadedTargetMethodInfo,
        postfix: new HarmonyMethod(PostfixPatchMethodInfo));
      Applied = true;
    }
    
    private static readonly MethodInfo GetCharacterMethod = typeof(Agent)
      .GetMethod("get_Character", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    
    private static bool IsAttackerAgentAssignedAndAgentRiderNotNull(List<CodeInstruction> instructions, int start)
      => start + 4 < instructions.Count
        && instructions[start].opcode == OpCodes.Ldarg_1
        && instructions[start + 1].opcode == OpCodes.Callvirt
        && instructions[start + 1].operand is MethodInfo mi && mi == GetCharacterMethod
        && instructions[start + 2].opcode == OpCodes.Stloc_S
        && instructions[start + 3].opcode == OpCodes.Ldloc_S
        && instructions[start + 4].opcode == OpCodes.Brfalse_S;

    private static BasicCharacterObject UpdateCorrectCharacterForHorseChargeDamage(Agent agent, ref AttackCollisionData acd, BasicCharacterObject character)
      => acd.IsHorseCharge && agent.RiderAgent != null ? agent.RiderAgent.Character : character;
    
    private static readonly MethodInfo CorrectCharacterForHorseChargeDamageMethod = typeof(TramplerPatch)
      .GetMethod("UpdateCorrectCharacterForHorseChargeDamage", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    
    private static readonly List<LocalVariableInfo> characterLocalVariableInfos = TargetMethodInfo
      .GetMethodBody().LocalVariables.Where(var => var.LocalType == typeof(BasicCharacterObject)).ToList();
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

      if (characterLocalVariableInfos.Count != 2) {
        CommunityPatchSubModule.Error($"{nameof(TramplerPatch)}: Expected two BasicCharacterObject  local variable instances in the original method."
          + $" The original code has been changed requiring an update of this patch." + Environment.NewLine);
        return instructions;
      }
      
      var codes = instructions.ToList();
      for (var index = 0; index<codes.Count - 5; index++)
      {
        if (IsAttackerAgentAssignedAndAgentRiderNotNull(codes, index)) {
          var attackerCharacterLocalVariableIndex = characterLocalVariableInfos[1].LocalIndex;
          codes.InsertRange(index+5, new List<CodeInstruction> {
            new CodeInstruction(OpCodes.Ldarg_1), // Load attackerAgent argument
            new CodeInstruction(OpCodes.Ldarg_S, 5), // Load attackCollisionData argument
            new CodeInstruction(OpCodes.Ldloc_S, attackerCharacterLocalVariableIndex), // Load attackerCharacter local variable
            new CodeInstruction(OpCodes.Call, CorrectCharacterForHorseChargeDamageMethod), // Update attackerCharacter if the collision is a horse charge
            new CodeInstruction(OpCodes.Stloc_S, attackerCharacterLocalVariableIndex) // Store value in attackerCharacter
          });
          return codes.AsEnumerable();
        }
      }
      
      CommunityPatchSubModule.Error($"{nameof(TramplerPatch)}: Could not find the starting point to add new instructions in {TargetMethodInfo}." + Environment.NewLine);
      return codes.AsEnumerable();
    }
    
    private static bool HeroHasPerk(BasicCharacterObject character, PerkObject perk)
      => (character as CharacterObject)?.GetPerkValue(perk) ?? false;

    private static int TramplerDamageModifier(int baseDamage) => baseDamage + baseDamage/2; 
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Postfix(BasicCharacterObject attackerAgentCharacter, ref AttackCollisionData attackCollisionData, ref CombatLogData combatLog) {
      if (!(attackCollisionData.IsHorseCharge && HeroHasPerk(attackerAgentCharacter, DefaultPerks.Riding.Trampler))) {
        return;
      }
      combatLog.InflictedDamage = TramplerDamageModifier(combatLog.InflictedDamage);
      attackCollisionData.InflictedDamage = TramplerDamageModifier(attackCollisionData.InflictedDamage);
    }
  }
}