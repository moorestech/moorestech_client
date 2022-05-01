﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Network.Event;
using MainGame.Network.Util;
using UnityEngine;

namespace MainGame.Network.Receive.EventPacket
{
    public class BlockRemoveEventProtocol : IAnalysisEventPacket 
    {
        private readonly NetworkReceivedChunkDataEvent _networkReceivedChunkDataEvents;

        public BlockRemoveEventProtocol(NetworkReceivedChunkDataEvent networkReceivedChunkDataEvents)
        {
            _networkReceivedChunkDataEvents = networkReceivedChunkDataEvents;
        }

        public void Analysis(List<byte> packet)
        {
            var bytes = new ByteArrayEnumerator(packet);
            bytes.MoveNextToGetShort();
            bytes.MoveNextToGetShort();
            var x = bytes.MoveNextToGetInt();
            var y = bytes.MoveNextToGetInt();
            
            _networkReceivedChunkDataEvents.InvokeBlockUpdateEvent(
                new BlockUpdateEventProperties(new Vector2Int(x,y),BlockConstant.NullBlockId,BlockDirection.North));
        }
    }
}