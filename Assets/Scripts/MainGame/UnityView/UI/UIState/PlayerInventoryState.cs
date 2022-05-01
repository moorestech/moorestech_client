﻿using System;
using MainGame.Control.UI.UIState;
using MainGame.Control.UI.UIState.UIObject;
using MainGame.UnityView.UI.CraftRecipe;
using MainGame.UnityView.UI.UIState.UIObject;

namespace MainGame.UnityView.UI.UIState
{
    public class PlayerInventoryState : IUIState
    {
        private readonly PlayerInventoryObject _playerInventory;
        
        private readonly MoorestechInputSettings _inputSettings;

        private readonly ItemListViewer _itemListViewer;
        private readonly ItemRecipePresenter _itemRecipePresenter;


        public event Action OnOpenInventory;

        public PlayerInventoryState( MoorestechInputSettings inputSettings, PlayerInventoryObject playerInventory,
            ItemListViewer itemListViewer,ItemRecipePresenter itemRecipePresenter)
        {
            _inputSettings = inputSettings;
            _playerInventory = playerInventory;
            _itemListViewer = itemListViewer;
            _itemRecipePresenter = itemRecipePresenter;

            playerInventory.gameObject.SetActive(false);
        }

        public bool IsNext()
        {
            return _inputSettings.UI.CloseUI.triggered || _inputSettings.UI.OpenInventory.triggered || _itemRecipePresenter.IsClicked;
        }

        public UIStateEnum GetNext()
        {
            if (_inputSettings.UI.CloseUI.triggered || _inputSettings.UI.OpenInventory.triggered)
            {
                return UIStateEnum.GameScreen;
            }

            if (_itemRecipePresenter.IsClicked)
            {
                return UIStateEnum.RecipeViewer;
            }

            return UIStateEnum.PlayerInventory;
        }

        public void OnEnter(UIStateEnum lastStateEnum)
        {
            _playerInventory.gameObject.SetActive(true);
            _playerInventory.SetCraftInventory();
            _itemListViewer.gameObject.SetActive(true);
            
            OnOpenInventory?.Invoke();
        }

        public void OnExit()
        {
            _playerInventory.gameObject.SetActive(false);
            _itemListViewer.gameObject.SetActive(false);
        }
    }
}