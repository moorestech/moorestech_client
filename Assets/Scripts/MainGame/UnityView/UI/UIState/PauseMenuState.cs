﻿using MainGame.UnityView.Control;
using MainGame.UnityView.UI.UIState.UIObject;

namespace MainGame.UnityView.UI.UIState
{
    public class PauseMenuState : IUIState
    {
        private readonly PauseMenuObject _pauseMenu;

        public PauseMenuState(PauseMenuObject pauseMenu)
        {
            _pauseMenu = pauseMenu;
            pauseMenu.gameObject.SetActive(false);
        }

        public bool IsNext()
        {
            return InputManager.UI.CloseUI.GetKeyDown;
        }

        public UIStateEnum GetNext()
        {
            if (InputManager.UI.CloseUI.GetKeyDown)
            {
                return UIStateEnum.GameScreen;
            }

            return UIStateEnum.PauseMenu;
        }

        public void OnEnter(UIStateEnum lastStateEnum) { _pauseMenu.gameObject.SetActive(true); }

        public void OnExit() { _pauseMenu.gameObject.SetActive(false); }
    }
}