using TaleWorlds.Library;

namespace CommunityPatch.Options {

  public abstract class LabelledViewModelBase : ViewModelBase {

    private string _name;

    [DataSourceProperty]
    public string Name {
      get => _name;
      set => UpdateWithNotify(ref _name, value);
    }

    private string _description;

    [DataSourceProperty]
    public string Description {
      get => _description;
      set => UpdateWithNotify(ref _description, value);
    }

  }

}