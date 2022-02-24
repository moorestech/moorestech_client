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
        [SerializeField] private HotBarItemView hotBarItemView;
        [SerializeField] private ItemImages itemImages;

        public MainInventoryDataCache MainInventoryDataCache => _mainInventoryDataCache;
        private MainInventoryDataCache _mainInventoryDataCache;
        
        //slot id count
        private List<(int, int, int)> _insertItems;

        private void Awake()
        {
            hotBarItemView.Construct(itemImages);
            mainInventoryItemView.Construct(itemImages);
            var updateEvent = new MainInventoryUpdateEvent();
            _mainInventoryDataCache = new MainInventoryDataCache(updateEvent,mainInventoryItemView,hotBarItemView);

            _insertItems = new List<(int,int,int)>();

            //アイテムの設定
            _insertItems.Add((0,1,5));
            _insertItems.Add((5,2,10));
            _insertItems.Add((10,2,1));
            _insertItems.Add((40,2,1));
            _insertItems.Add((44,2,1));

            //イベントを発火
            foreach (var item in _insertItems)
            {
                updateEvent.InvokeMainInventorySlotUpdate(
                    new MainInventorySlotUpdateProperties(
                        item.Item1,new ItemStack(item.Item2,item.Item3)));
            }

            StartCoroutine(Check());
        }

        private IEnumerator Check()
        {
            yield return new WaitForSeconds(0.1f);
            //アイテムのUIの取得
            var slots = mainInventoryItemView.GetInventoryItemSlots();

            //チェック
            foreach (var item in _insertItems)
            {
                var slot = item.Item1;
                var id = item.Item2;
                var count = item.Item3;

                var expectedCount = count.ToString();
                var actualCount = slots[slot].gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
                Assert.AreEqual(expectedCount,actualCount);

                //同じ画像かチェック
                var expectedImage = itemImages.GetItemImage(id).GetHashCode();
                //ButtonにもImageがついてあるため、そのままだとButtonのImageが取得される。
                //そのため、Last()を使ってアイテムのImageを取得する
                var actualImage = slots[slot].GetComponentsInChildren<Image>().Last().sprite.GetHashCode();
                Assert.AreEqual(expectedImage,actualImage);
            }
        }
    }
}