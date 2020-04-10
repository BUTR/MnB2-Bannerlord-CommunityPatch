using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace CommunityPatch {

  public class CustomOptionCategoryViewModel : OptionCategoryVM {

    public CustomOptionCategoryViewModel(OptionsVM optionsVm, TextObject tabName, IEnumerable<GenericOptionDataVM> options)
      : base(optionsVm, tabName, new IOptionData[0], false) {
      var optionsField = typeof(OptionCategoryVM).GetField("_options",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      // ReSharper disable once PossibleNullReferenceException
      var optionsBindingList = (MBBindingList<GenericOptionDataVM>) optionsField.GetValue(this);

      foreach (var option in options)
        optionsBindingList.Add(option);

      // ReSharper disable once VirtualMemberCallInConstructor // because of course we do...
      RefreshValues();
    }

  }

}