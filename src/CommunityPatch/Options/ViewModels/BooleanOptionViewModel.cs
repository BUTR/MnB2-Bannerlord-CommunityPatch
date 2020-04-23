using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch.Options {

  public class BooleanOptionViewModel : DiscreteOptionViewModelBase<bool> {

    public BooleanOptionViewModel(Option<bool> option) : base(option) {
    }

  }

}