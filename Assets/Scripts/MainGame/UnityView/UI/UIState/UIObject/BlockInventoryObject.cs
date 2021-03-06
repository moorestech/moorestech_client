using System.Collections.Generic;
using Game.PlayerInventory.Interface;
using MainGame.UnityView.UI.Inventory.View;
using MainGame.UnityView.UI.Inventory.View.SubInventory;
using MainGame.UnityView.UI.Inventory.View.SubInventory.Element;
using UnityEngine;

namespace MainGame.UnityView.UI.UIState.UIObject
{
    public class BlockInventoryObject : MonoBehaviour
    {
        [SerializeField] private PlayerInventorySlots playerInventorySlots;

        public void SetOneSlotInventory(string blockName,int slot)
        {
            var arraySlot = new List<ArraySlot>();
            arraySlot.Add(CreateArraySlot(0,272,10,slot,PlayerInventoryConst.MainInventoryColumns));
            
            var text = new List<TextElement>(){new(0,436,0,blockName,30)};
            
            var blockInventory = new SubInventoryViewBluePrint(){ArraySlots = arraySlot,TextElements = text};
            
            playerInventorySlots.SetSubSlots(blockInventory,new SubInventoryOptions());
        }
        
        public void SetIOSlotInventory(string blockName,int input,int output)
        {
            const int maxSlotColumns = 5;
            var arraySlot = new List<ArraySlot>();
            arraySlot.Add(CreateArraySlot(330,272,10,input,maxSlotColumns));
            arraySlot.Add(CreateArraySlot(-330,272,10,output,maxSlotColumns));
            
            var text = new List<TextElement>(){new(0,436,0,blockName,30)};
            
            var blockInventory = new SubInventoryViewBluePrint(){ArraySlots = arraySlot,TextElements = text};
            
            playerInventorySlots.SetSubSlots(blockInventory,new SubInventoryOptions());
        }


        private ArraySlot CreateArraySlot(int x,int y,int priority,int slot,int maxSlotColumns)
        {
            if (slot < maxSlotColumns)
            {
                return new ArraySlot(x, y, priority, 1, slot);
            }

            var height = 1 + slot / maxSlotColumns;
            var width = maxSlotColumns;
            var blank = maxSlotColumns - slot % maxSlotColumns;
                
            return new ArraySlot(x, y, priority, height, maxSlotColumns,blank);

        }
        
        
    }
}