﻿using MainGame.UnityView.ControllerInput;
using MainGame.UnityView.Interface;
using MainGame.UnityView.Interface.PlayerInput;
using MainGame.UnityView.UI.Inventory.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainGame.Control.UI.Inventory
{
    public class MouseInventoryInput : MonoBehaviour,IControllerInput,IPostStartable
    {
        [SerializeField] private InventoryItemSlot equippedItem;
        
        private int _equippedItemIndex = -1;
        private MainInventoryItemView _mainInventoryItemView;
        private IPlayerInventoryItemMove _playerInventoryItemMove;
        
        private MoorestechInputSettings _inputSettings;

        [Inject]
        public void Construct(
            MainInventoryItemView mainInventoryItemView,
            IPlayerInventoryItemMove playerInventoryItemMove,
            IInventoryUpdateEvent inventoryUpdateEvent)
        {
            _mainInventoryItemView = mainInventoryItemView;
            _playerInventoryItemMove = playerInventoryItemMove;
            
            equippedItem.gameObject.SetActive(false);
            _inputSettings = new();
            _inputSettings.Enable();
            inventoryUpdateEvent.Subscribe(InventoryUpdate);
        }
        
        //イベントをボタンに登録する
        public void PostStart()
        {
            foreach (var slot in _mainInventoryItemView.GetInventoryItemSlots())
            {
                slot.SubscribeOnItemSlotClick(OnSlotClick);
            }
        }
        //ボタンがクリックされた時に呼び出される
        private void OnSlotClick(int slot)
        {
            if (_equippedItemIndex == -1)
            {
                var fromItem = _mainInventoryItemView.GetInventoryItemSlots()[slot];
                equippedItem.CopyItem(fromItem);
                
                _equippedItemIndex = slot;
                equippedItem.gameObject.SetActive(true);
                return;
            }

            //アイテムを半分だけおく
            if (_inputSettings.UI.InventoryItemHalve.inProgress)
            {
                _playerInventoryItemMove.MoveHalfItemStack(_equippedItemIndex,slot);
                return;
            }
            
            //アイテムを一個だけおく
            if (_inputSettings.UI.InventoryItemOnePut.inProgress)
            {
                _playerInventoryItemMove.MoveOneItemStack(_equippedItemIndex,slot);
                return;
            }
            
            //アイテムを全部おく
            _playerInventoryItemMove.MoveAllItemStack(_equippedItemIndex,slot);
            _equippedItemIndex = -1;
            equippedItem.gameObject.SetActive(false);
            
            
        }

        //equippedItemの更新を行う
        private void InventoryUpdate(int slot, int itemId, int count)
        {
            if (slot != _equippedItemIndex) return;
            var fromItem = _mainInventoryItemView.GetInventoryItemSlots()[slot];
            equippedItem.CopyItem(fromItem);
        }
        
        public void OnInput()
        {
            throw new System.NotImplementedException();
        }

        public void OffInput()
        {
            throw new System.NotImplementedException();
        }

    }
}