using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace CommunityPatch.Options {

  public abstract class ViewModelBase : ViewModel {

    protected void UpdateWithNotify<TField>(ref TField field, TField value, [CallerMemberName] string name = null) where TField : IEquatable<TField> {
      if (field == null ? value == null : field.Equals(value))
        return;

      field = value;
      OnPropertyChanged(name);
    }

  }

}