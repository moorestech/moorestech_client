using System;
using System.Management.Instrumentation;
using UnityEngine.InputSystem;

namespace MainGame.UnityView.Control
{
    public static class InputManager
    {
        public static PayerInputManager Player => player ??= new PayerInputManager(Instance);
        private static PayerInputManager player;
            
        public static PlayableInputManager Playable => playable ??= new PlayableInputManager(Instance);
        private static PlayableInputManager playable;

        public static UIInputManager UI => ui ??= new UIInputManager(Instance);
        private static UIInputManager ui;
        
        
        private static MoorestechInputSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MoorestechInputSettings();
                    _instance.Enable();
                }
                
                return _instance;
            }
        }
        private static MoorestechInputSettings _instance;
    }

    public class PayerInputManager
    {
        public readonly InputKey Move;
        public readonly InputKey Look;
        public readonly InputKey Jump;
        public readonly InputKey Sprint;

        public PayerInputManager(MoorestechInputSettings settings)
        {
            Move = new InputKey(settings.Player.Move);
            Look = new InputKey(settings.Player.Look);
            Jump = new InputKey(settings.Player.Jump);
            Sprint = new InputKey(settings.Player.Sprint);
        }
    }

    public class PlayableInputManager
    {
        public readonly InputKey ScreenLeftClick;
        public readonly InputKey ScreenRightClick;
        public readonly InputKey ClickPosition;
        public readonly InputKey BlockPlaceRotation;

        public PlayableInputManager(MoorestechInputSettings settings)
        {
            ScreenLeftClick = new InputKey(settings.Playable.ScreenLeftClick);
            ScreenRightClick = new InputKey(settings.Playable.ScreenRightClick);
            ClickPosition = new InputKey(settings.Playable.ClickPosition);
            BlockPlaceRotation = new InputKey(settings.Playable.BlockPlaceRotation);
        }
    }

    public class UIInputManager
    {
        public readonly InputKey OpenMenu;
        public readonly InputKey CloseUI;
        public readonly InputKey OpenInventory;
        public readonly InputKey InventoryItemOnePut;
        public readonly InputKey InventoryItemHalve;
        public readonly InputKey HotBar;
        public readonly InputKey SwitchHotBar;
        public readonly InputKey BlockDelete;
        public readonly InputKey AllCraft;
        public readonly InputKey OneStackCraft;
        public readonly InputKey QuestUI;
        public readonly InputKey ItemDirectMove;

        public UIInputManager(MoorestechInputSettings settings)
        {
            OpenMenu = new InputKey(settings.UI.OpenMenu);
            CloseUI = new InputKey(settings.UI.CloseUI);
            OpenInventory = new InputKey(settings.UI.OpenInventory);
            InventoryItemOnePut = new InputKey(settings.UI.InventoryItemOnePut);
            InventoryItemHalve = new InputKey(settings.UI.InventoryItemHalve);
            HotBar = new InputKey(settings.UI.HotBar);
            SwitchHotBar = new InputKey(settings.UI.SwitchHotBar);
            BlockDelete = new InputKey(settings.UI.BlockDelete);
            AllCraft = new InputKey(settings.UI.AllCraft);
            OneStackCraft = new InputKey(settings.UI.OneStackCraft);
            QuestUI = new InputKey(settings.UI.QuestUI);
            ItemDirectMove = new InputKey(settings.UI.ItemDirectMove);
        }
    } 
    public class InputKey
    {
        private readonly InputAction _inputAction;
        
        public event Action OnGetKeyDown;
        public event Action OnGetKey;
        public event Action OnGetKeyUp;
        
        public bool GetKeyDown => _inputAction.WasPressedThisFrame();
        public bool GetKey => _inputAction.IsPressed();
        public bool GetKeyUp => _inputAction.WasReleasedThisFrame();
        
        public TValue ReadValue<TValue>() where TValue : struct => _inputAction.ReadValue<TValue>();
        
        
        public InputKey(InputAction key)
        {
            _inputAction = key;
            key.started += _ => { OnGetKeyDown?.Invoke(); };
            key.performed += _ => { OnGetKey?.Invoke(); };
            key.canceled += _ => { OnGetKeyUp?.Invoke(); };
        }
    }
}