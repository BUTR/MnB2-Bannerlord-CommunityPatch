using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch.Options {

  public class ByteOptionViewModel : DiscreteOptionViewModelBase<byte> {

    public ByteOptionViewModel(Option<byte> option)
      : base(option) {
    }

  }

}