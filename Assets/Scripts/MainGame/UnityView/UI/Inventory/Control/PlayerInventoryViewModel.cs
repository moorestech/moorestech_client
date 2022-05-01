using System;
using System.Collections;
using System.Collections.Generic;
using Core.Item;
using MainGame.Basic;
using SinglePlay;

namespace MainGame.UnityView.UI.Inventory.Control
{
    /// <summary>
    /// プレイヤーの操作に応じてローカルでもっておくインベントリのアイテムリスト
    /// 実際のインベントリとのラグを軽減するためのキャッシュ機構で、実際のインベントリを更新するパケットが到着したら適宜入れ替える（ロールバックを発生させる）
    /// </summary>
    public class PlayerInventoryViewModel : IEnumerable<IItemStack>
    {
        private List<IItemStack> _mainInventory = new();
        private List<IItemStack> _subInventory = new();
        private readonly ItemStackFactory _itemStackFactory;
        public event Action OnInventoryUpdate;
        public int Count => _mainInventory.Count + _subInventory.Count;


        public PlayerInventoryViewModel(SinglePlayInterface single)
        {
            _itemStackFactory = single.ItemStackFactory;
        }

        public IItemStack this[int index]
        {
            get
            {
                if (index < _mainInventory.Count)
                {
                    return _mainInventory[index];
                }
                return _subInventory[index - _mainInventory.Count];
            }
            set
            {
                if (index < _mainInventory.Count)
                {
                    _mainInventory[index] = value;
                    return;
                }
                _subInventory[index - _mainInventory.Count] = value;
            }
        }

        public void SetMainInventory(List<ItemStack> mainInventory)
        {
            _mainInventory = mainInventory.ToIItemStackList(_itemStackFactory);
            OnInventoryUpdate?.Invoke();
        }
        
        public void SetSubInventory(List<ItemStack> subInventory)
        {
            _subInventory = subInventory.ToIItemStackList(_itemStackFactory);
            OnInventoryUpdate?.Invoke();
        }

        public IEnumerator<IItemStack> GetEnumerator()
        {
            var merged = new List<IItemStack>();
            merged.AddRange(_mainInventory);
            merged.AddRange(_subInventory);
            return merged.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class ListItemStackExtend
    {
        public static List<IItemStack> ToIItemStackList(this List<ItemStack> list,ItemStackFactory factory)
        {
            List<IItemStack> result = new List<IItemStack>();
            foreach (var itemStack in list)
            {
                result.Add(factory.Create(itemStack.ID,itemStack.Count));
            }
            return result;
        }
    }
    
}