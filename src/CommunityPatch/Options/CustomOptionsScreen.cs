using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.TwoDimension;

namespace CommunityPatch {

  [OverrideView(typeof(OptionsScreen))]
  public class CustomOptionsGauntletScreen : ScreenBase {

    private GauntletLayer _gauntletLayer;

    private CustomOptionsViewModel _dataSource;

    private GauntletMovie _gauntletMovie;

    private KeybindingPopup _keyBindingPopup;

    private GameKeyOptionVM _currentGameKey;

    private SpriteCategory _spriteCategory;

    private OptionsVM _optionsVm;

    private IEnumerable<GenericOptionDataVM> GenerateOptionDataViewModels(IEnumerable<Option> options) {
      foreach (var option in options) {
        if (option.IsEnum)
          throw new NotImplementedException("Enum options are not yet implemented.");

        var metaVm = option.GetMetadata("ViewModel");
        if (metaVm != null) {
          if (metaVm is Func<OptionsVM, StringOptionViewModel> s)
            yield return s(_optionsVm);
          if (metaVm is Func<OptionsVM, GenericOptionDataVM> g)
            yield return g(_optionsVm);
        }
        else
          switch (option) {
            // @formatter:off
            case StringOption v: yield return new StringOptionViewModel(v, _optionsVm, (bool?) v.GetMetadata("IsObfuscated") ?? false); break;
            case Option<bool> v: yield return new BooleanOptionViewModel(v, _optionsVm); break;
            case Option<sbyte> v: yield return new SByteOptionViewModel(v, _optionsVm); break;
            case Option<byte> v: yield return new ByteOptionViewModel(v, _optionsVm); break;
            case Option<short> v: yield return new ShortOptionViewModel(v, _optionsVm); break;
            case Option<ushort> v: yield return new UShortOptionViewModel(v, _optionsVm); break;
            case Option<int> v: yield return new IntOptionViewModel(v, _optionsVm); break;
            case Option<uint> v: yield return new UIntOptionViewModel(v, _optionsVm); break;
            case Option<long> v: yield return new LongOptionViewModel(v, _optionsVm); break;
            case Option<ulong> v: yield return new ULongOptionViewModel(v, _optionsVm); break;
            case Option<float> v: yield return new FloatOptionViewModel(v, _optionsVm); break;
            case Option<double> v: yield return new DoubleOptionViewModel(v, _optionsVm); break;
            // @formatter:on
            default:
              throw new NotImplementedException(option.GetType().Name + " is not implemented.");
            //throw new NotSupportedException(option.GetType().Name + " is not supported."); // <- future
          }
      }
    }

    protected override void OnInitialize() {
      base.OnInitialize();
      _spriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_options"];
      _spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
      _optionsVm = new OptionsVM(true, false, OnKeyBindRequest);
      _dataSource = new CustomOptionsViewModel(_optionsVm) {
        ModOptions = new CustomOptionCategoryViewModel(_optionsVm, "ModOptions".Localized(),
          GenerateOptionDataViewModels(CommunityPatchSubModule.Options.GetKnownOptions()))
      };
      _gauntletLayer = new GauntletLayer(4000);
      _gauntletMovie = _gauntletLayer.LoadMovie("CustomOptions", _dataSource);
      _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
      _gauntletLayer.InputRestrictions.SetInputRestrictions();
      _gauntletLayer.IsFocusLayer = true;
      _keyBindingPopup = new KeybindingPopup(SetHotKey, this);
      AddLayer(_gauntletLayer);
      ScreenManager.TrySetFocus(_gauntletLayer);
    }

    protected override void OnFinalize() {
      base.OnFinalize();
      _spriteCategory.Unload();
    }

    protected override void OnDeactivate()
      => LoadingWindow.EnableGlobalLoadingWindow();

    protected override void OnFrameTick(float dt) {
      base.OnFrameTick(dt);
      if (!_keyBindingPopup.IsActive && _gauntletLayer.Input.IsHotKeyReleased("Exit")) {
        _dataSource.ExecuteCloseOptions();
        ScreenManager.TrySetFocus(_gauntletLayer);
        if (Game.Current != null) {
          Game.Current.GameStateManager.ActiveStateDisabledByUser = _dataSource.OldGameStateManagerDisabledStatus;
        }

        ScreenManager.PopScreen();
      }

      _keyBindingPopup.Tick();
    }

    private void OnKeyBindRequest(GameKeyOptionVM requestedHotKeyToChange) {
      _currentGameKey = requestedHotKeyToChange;
      _keyBindingPopup.OnToggle(true);
    }

    private void SetHotKey(Key key) {
      if (_dataSource.GameKeyOptionGroups.Groups
        .First(g => g.GameKeys.Contains(_currentGameKey))
        .GameKeys.Any((GameKeyOptionVM k) => k.CurrentKey.InputKey == key.InputKey)
      ) {
        InformationManager.AddQuickInformation(new TextObject("{=n4UUrd1p}Already in use"));
        return;
      }

      if (_gauntletLayer.Input.IsHotKeyReleased("Exit")) {
        _keyBindingPopup.OnToggle(false);
        return;
      }

      var currentGameKey = _currentGameKey;
      currentGameKey?.Set(key.InputKey);

      _currentGameKey = null;
      _keyBindingPopup.OnToggle(false);
    }

  }

}