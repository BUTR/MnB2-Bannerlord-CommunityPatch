using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.Core;

using static CommunityPatch.PatchApplicabilityHelper;
using static CommunityPatch.HarmonyHelpers;
using static HarmonyLib.AccessTools;

namespace CommunityPatch {

  public abstract class PatchBase<TPatch> : IPatch where TPatch : IPatch {

    private static readonly Regex TargetMethodNameRegex
      = new Regex(@"(?'targetMethodName'.*)(?'patchType'Prefix|Postfix|Transpiler|Finalizer)", RegexOptions.Compiled);

    protected readonly Dictionary<MethodInfo, List<(string patchType, MethodInfo patchMethod)>> PatchedMethodsInfo
      = new Dictionary<MethodInfo, List<(string, MethodInfo)>>();

    [PublicAPI]
    public static TPatch ActivePatch => (TPatch) CommunityPatchSubModule.ActivePatches[typeof(TPatch)];

    protected PatchBase() {
      var patchMethods = GetType().GetMethods(all).Where(m => Attribute.IsDefined(m, typeof(PatchClassAttribute)));
      foreach (var patchMethod in patchMethods) {
        var match = TargetMethodNameRegex.Match(patchMethod.Name);
        var targetMethodName = match.Groups["targetMethodName"].Value;
        var patchType = match.Groups["patchType"].Value;
        var info = patchMethod.GetCustomAttribute<PatchClassAttribute>().Info;
        var targetMethod = info.declaringType.GetMethod(targetMethodName, all);
        if (!PatchedMethodsInfo.ContainsKey(targetMethod)) {
          PatchedMethodsInfo[targetMethod] = new List<(string, MethodInfo)>();
        }
        PatchedMethodsInfo[targetMethod].Add((patchType, patchMethod));
      }
    }

    public virtual bool? IsApplicable(Game game) {
      if (!this.IsCompatibleWithGameVersion()) {
        return false;
      }
      
      foreach (var targetMethod in PatchedMethodsInfo.Keys) {
        var patchInfo = Harmony.GetPatchInfo(targetMethod);
        if (AlreadyPatchedByOthers(patchInfo)) {
          return false;
        }
        var hash = targetMethod.MakeCilSignatureSha256();
        if (!hash.MatchesAnySha256((byte[][]) GetType().GetField("Hashes", all).GetValue(null))) {
          return false;
        }
      }
      return true;
    }

    public virtual void Apply(Game game) {
      if (!Applied) {
        foreach (var kv in PatchedMethodsInfo) {
          var targetMethod = kv.Key;
          var patchMethods = kv.Value;
          var patchProcessor = CommunityPatchSubModule.Harmony.CreateProcessor(targetMethod);
          foreach (var (patchType, patchMethod) in patchMethods) {
            patchProcessor.GetType().GetMethod($"Add{patchType}", new[] { typeof(MethodInfo) })
              .Invoke(patchProcessor, new object[] { patchMethod });
          }
          patchProcessor.Patch();
        }
        Applied = true;
      }
    }

    public virtual bool Applied { get; protected set; }

    public virtual void Reset() { }

    public virtual IEnumerable<MethodBase> GetMethodsChecked()
      => PatchedMethodsInfo.Keys;

  }

}