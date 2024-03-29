﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MainGame.Network.Event;

using MessagePack;
using Server.Event.EventReceive;
using UnityEngine;

namespace MainGame.Network.Receive.EventPacket
{
    public class BlockInventorySlotUpdateEventProtocol : IAnalysisEventPacket
    {
        private readonly ReceiveBlockInventoryEvent receiveBlockInventoryEvent;

        public BlockInventorySlotUpdateEventProtocol(ReceiveBlockInventoryEvent receiveBlockInventoryEvent)
        {
            this.receiveBlockInventoryEvent = receiveBlockInventoryEvent;
        }

        public void Analysis(List<byte> packet)
        {

            var data = MessagePackSerializer
                .Deserialize<OpenableBlockInventoryUpdateEventMessagePack>(packet.ToArray());
            
            receiveBlockInventoryEvent.InvokeBlockInventorySlotUpdate(
                new BlockInventorySlotUpdateProperties(new Vector2Int(data.X,data.Y), data.Slot, data.Item.Id, data.Item.Count)).Forget();
        }
    }
}