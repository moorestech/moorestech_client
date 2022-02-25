using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MainGame.Basic;
using MainGame.GameLogic.Inventory;
using MainGame.Network.Event;
using MainGame.UnityView;
using MainGame.UnityView.UI.Inventory.Element;
using MainGame.UnityView.UI.Inventory.View;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Test.TestModule.UI
{
    public class InventoryViewTestModule : MonoBehaviour
    {
        [SerializeField] private MainInventoryItemView mainInventoryItemView;
        [SerializeField] private CraftingInventoryItemView craftingInventoryItemView;
        [SerializeField] private HotBarItemView hotBarItemView;
        [SerializeField] private ItemImages itemImages;

        public MainInventoryDataCache MainInventoryDataCache => _mainInventoryDataCache;
        private MainInventoryDataCache _mainInventoryDataCache;
        
        public CraftingInventoryDataCache CraftingInventoryDataCache => _craftingInventoryDataCache;
        private CraftingInventoryDataCache _craftingInventoryDataCache;
        
        //slot id count
        private List<(int, int, int)> _insertItems;

        private void Awake()
        {
            hotBarItemView.Construct(itemImages);
            mainInventoryItemView.Construct(itemImages);
            
            

            //メインインベントリに挿入するアイテムの設定
            _insertItems = new List<(int,int,int)>();
            _insertItems.Add((0,1,5));
            _insertItems.Add((5,2,10));
            _insertItems.Add((10,2,1));
            _insertItems.Add((40,2,1));
            _insertItems.Add((44,2,1));
            
            
            //メインインベントリの設定とイベントの発火
            var mainUpdateEvent = new MainInventoryUpdateEvent();
            _mainInventoryDataCache = new MainInventoryDataCache(mainUpdateEvent,mainInventoryItemView,hotBarItemView);

            //イベントを発火
            foreach (var item in _insertItems)
            {
                mainUpdateEvent.InvokeMainInventorySlotUpdate(
                    new MainInventorySlotUpdateProperties(
                        item.Item1,new ItemStack(item.Item2,item.Item3)));
            }
            
            
            //クラフトインベントリに挿入するアイテムの設定
            _insertItems = new List<(int,int,int)>();
            _insertItems.Add((0,1,5));
            _insertItems.Add((5,2,10));
            _insertItems.Add((10,2,1));
            _insertItems.Add((40,2,1));
            _insertItems.Add((44,2,1));
            
            
            //メインインベントリの設定とイベントの発火
            var craftingUpdateEvent = new CraftingInventoryUpdateEvent();
            _craftingInventoryDataCache = new CraftingInventoryDataCache(craftingUpdateEvent,craftingInventoryItemView);

            //イベントを発火
            foreach (var item in _insertItems)
            {
                mainUpdateEvent.InvokeMainInventorySlotUpdate(
                    new MainInventorySlotUpdateProperties(
                        item.Item1,new ItemStack(item.Item2,item.Item3)));
            }
        }
    }
}