using System.Collections.Generic;
using MainGame.Constant;
using MainGame.UnityView.Interface;
using MainGame.UnityView.UI.Inventory.Element;
using UnityEngine;
using VContainer;

namespace MainGame.UnityView.UI.Inventory.View
{
    //TODO ブロックインベントリを開くシステム
    public class BlockInventoryItemView : MonoBehaviour
    {
        private const int SlotCount = 5;
        
        
        [SerializeField] private RectTransform inputItems;
        [SerializeField] private RectTransform outputItems;
        
        [SerializeField] private InventoryItemSlot inventoryItemSlot;
        private readonly List<InventoryItemSlot> _playerInventorySlots = new();
        private readonly List<InventoryItemSlot> _inputInventorySlots = new();
        private readonly List<InventoryItemSlot> _outputInventorySlots = new();
        private ItemImages _itemImages;
        
        
        [Inject]
        public void Construct(IInventoryUpdateEvent inventoryUpdateEvent,ItemImages itemImages)
        {
            inventoryUpdateEvent.Subscribe(OnInventoryUpdate);
            _itemImages = itemImages;
            
            //プレイヤーインベントリの表示
            for (int i = 0; i < PlayerInventoryConstant.MainInventorySize; i++)
            {
                var s = Instantiate(inventoryItemSlot.gameObject, transform).GetComponent<InventoryItemSlot>();
                s.Construct(i);
                _playerInventorySlots.Add(s);
            }

            //ブロックインベントリの表示
            for (int i = 0; i < SlotCount; i++)
            {
                //inputの設定
                var s = Instantiate(inventoryItemSlot.gameObject, inputItems).GetComponent<InventoryItemSlot>();
                _inputInventorySlots.Add(s);
                
                //outputの設定
                s = Instantiate(inventoryItemSlot.gameObject, outputItems).GetComponent<InventoryItemSlot>();
                _outputInventorySlots.Add(s);
            }
        }

        private void OnInventoryUpdate(int slot, int itemId, int count)
        {
            var sprite = _itemImages.GetItemImage(itemId);
            _playerInventorySlots[slot].SetItem(sprite,count);
        }
        
        public IReadOnlyList<InventoryItemSlot> GetInventoryItemSlots()
        {
            return _playerInventorySlots;
        }
    }
}