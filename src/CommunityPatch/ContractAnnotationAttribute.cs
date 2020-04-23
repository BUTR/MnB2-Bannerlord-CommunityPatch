using System;
using System.Diagnostics;

namespace JetBrains.Annotations {

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  [Conditional("JETBRAINS_ANNOTATIONS")]
  public sealed class ContractAnnotationAttribute : Attribute {

    public ContractAnnotationAttribute([NotNull] string contract) : this(contract, false) {
    }

    public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates) {
      Contract = contract;
      ForceFullStates = forceFullStates;
    }

    [NotNull]
    public string Contract { get; }

    public bool ForceFullStates { get; }

  }

}