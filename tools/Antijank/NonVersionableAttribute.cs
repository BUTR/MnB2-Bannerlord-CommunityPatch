namespace System.Runtime.Versioning {

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
  public sealed class NonVersionableAttribute : Attribute {

  }

}