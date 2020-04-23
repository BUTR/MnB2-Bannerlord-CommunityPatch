using System;
using JetBrains.Annotations;

namespace CommunityPatch.Options {

  public interface IOption : IComparable, IComparable<Option> {

    [NotNull]
    public OptionsStore Store { get; }

    [CanBeNull]
    public string Namespace { get; }

    [NotNull]
    public string Name { get; }

    bool IsEnum { get; }

    public abstract void Set(object value);

    object GetMetadata(string metadataType);

  }

  public interface IOption<TOption> : IComparable<IOption<TOption>>, IEquatable<IOption<TOption>>, IEquatable<TOption> {

    void Set(TOption value);

  }

}