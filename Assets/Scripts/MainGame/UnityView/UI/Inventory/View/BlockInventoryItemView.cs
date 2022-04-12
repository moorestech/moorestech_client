using System.Collections.Generic;
using MainGame.Basic;
using MainGame.UnityView.Block;
using MainGame.UnityView.UI.Inventory.Element;
using TMPro;
using UnityEngine;
using VContainer;

namespace MainGame.UnityView.UI.Inventory.View
{
    //ブロックインベントリを開くシステム
    //共通UI基盤にしたら消す
    public class BlockInventoryItemView : MonoBehaviour
    {
        private const int SlotCount = 5;
        
        
        [SerializeField] private RectTransform inputItems;
        [SerializeField] private RectTransform outputItems;

        [SerializeField] private InventoryItemSlot inventoryItemSlotPrefab;
        [SerializeField] private TMP_Text machineName;
        
        private readonly List<InventoryItemSlot> _mainInventorySlots = new();
        private readonly List<InventoryItemSlot> _inputInventorySlots = new();
        private readonly List<InventoryItemSlot> _outputInventorySlots = new();
        private ItemImages _itemImages;
        private int _inputSlotCount;
        private BlockObjects _blockObjects;

        private int _equippedItemIndex = -1;
        
        [Inject]
        public void Construct(ItemImages itemImages,BlockObjects blockObjects)
        {
            _blockObjects = blockObjects;
            _itemImages = itemImages;
        }

        //ブロックのインベントリを開く
        public void SettingBlockInventory(string uiType,int blockId, params short[] param)
        {
            //ステータスとステUitypeを渡しているけど現在は使っていない
            //ここは共通インベントリ基盤を作成する
            var input = param[0];
            var output = param[1];
            
            _inputSlotCount = input;
            //全て非表示
            _inputInventorySlots.ForEach(i => i.gameObject.SetActive(false));
            _outputInventorySlots.ForEach(i => i.gameObject.SetActive(false));
            
            //必要な分だけ表示し、indexを設定する
            for (int i = 0; i < input; i++)
            {
                _inputInventorySlots[i].gameObject.SetActive(true);
                _inputInventorySlots[i].Construct(PlayerInventoryConstant.MainInventorySize + i);
            }

            for (int i = 0; i < output; i++)
            {
                _outputInventorySlots[i].gameObject.SetActive(true);
                _outputInventorySlots[i].Construct(PlayerInventoryConstant.MainInventorySize + input + i);
            }

            machineName.text = _blockObjects.GetName(blockId);
        }

        //スロットをアップデートする
        public void BlockInventoryUpdate(int slot, int itemId, int count)
        {
            if (_equippedItemIndex == slot)
            {
                return;
            }
            
            var sprite = _itemImages.GetItemView(itemId);
            if (slot < _inputSlotCount)
            {
                _inputInventorySlots[slot].SetItem(sprite,count);
            }
            else
            {
                _outputInventorySlots[slot - _inputSlotCount].SetItem(sprite,count);
            }
        }
        
        
        public void MainInventoryUpdate(int slot, int itemId, int count)
        {
            var sprite = _itemImages.GetItemView(itemId);
            _mainInventorySlots[slot].SetItem(sprite,count);
        }
        
        
        public void ItemEquipped(int slot)
        {
            _equippedItemIndex = slot;
            _mainInventorySlots[slot].SetItem(_itemImages.GetItemView(0),0);
        }
        
        public void ItemUnequipped()
        {
            _equippedItemIndex = -1;
        }
        
        public IReadOnlyList<InventoryItemSlot> GetAllInventoryItemSlots()
        {
            if (_mainInventorySlots.Count == 0)
            {
                SetInventorySlot();
            }
            
            var merged = new List<InventoryItemSlot>();
            merged.AddRange(_mainInventorySlots);
            merged.AddRange(_inputInventorySlots);
            merged.AddRange(_outputInventorySlots);
            return merged;
        }

        private void SetInventorySlot()
        {
            //メインインベントリの作成
            for (int i = 0; i < PlayerInventoryConstant.MainInventorySize; i++)
            {
                var s = Instantiate(inventoryItemSlotPrefab.gameObject, transform).GetComponent<InventoryItemSlot>();
                s.Construct(i);
                _mainInventorySlots.Add(s);
            }

            //ブロックインベントリの作成
            for (int i = 0; i < SlotCount; i++)
            {
                //inputの設定
                var s = Instantiate(inventoryItemSlotPrefab.gameObject, inputItems).GetComponent<InventoryItemSlot>();
                s.Construct(i);
                s.gameObject.SetActive(false);
                _inputInventorySlots.Add(s);
                
                //outputの設定
                s = Instantiate(inventoryItemSlotPrefab.gameObject, outputItems).GetComponent<InventoryItemSlot>();
                s.Construct(i);
                s.gameObject.SetActive(false);
                _outputInventorySlots.Add(s);
            }
        }
        
        public InventoryItemSlot GetOpenedInventoryItemSlot(int index)
        {
            if (index < PlayerInventoryConstant.MainInventorySize)
            {
                return _mainInventorySlots[index];
            }
            index -= PlayerInventoryConstant.MainInventorySize;
            if (index < _inputSlotCount)
            {
                return _inputInventorySlots[index];
            }

            return _outputInventorySlots[index - _inputSlotCount];
        }
    }
}