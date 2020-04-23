using System;

namespace CommunityPatch.Options {

  public class DiscreteOptionViewModelBase<T> : OptionViewModelBase<T> where T : IEquatable<T>, IComparable<T> {

    protected DiscreteOptionViewModelBase(Option option) : base(option) {
    }

  }

}