﻿using System.Collections.Generic;
using MainGame.Basic;
using MainGame.Network.Event;
using MainGame.Network.Util;
using UnityEngine;

namespace MainGame.Network.Receive.EventPacket
{
    public class BlockPlaceEventProtocol : IAnalysisEventPacket
    {
        readonly NetworkReceivedChunkDataEvent _networkReceivedChunkDataEvent;

        public BlockPlaceEventProtocol(NetworkReceivedChunkDataEvent networkReceivedChunkDataEvent)
        {
            _networkReceivedChunkDataEvent = networkReceivedChunkDataEvent;
        }

        public void Analysis(List<byte> packet)
        {
            var bytes = new ByteArrayEnumerator(packet);
            bytes.MoveNextToGetShort();
            bytes.MoveNextToGetShort();
            var x = bytes.MoveNextToGetInt();
            var y = bytes.MoveNextToGetInt();
            var blockId = bytes.MoveNextToGetInt();

            var direction = bytes.MoveNextToGetByte() switch
            {
                0 => BlockDirection.North,
                1 => BlockDirection.East,
                2 => BlockDirection.South,
                3 => BlockDirection.West,
                _ => BlockDirection.North
            };

            //ブロックをセットする
            _networkReceivedChunkDataEvent.InvokeBlockUpdateEvent(new BlockUpdateEventProperties(new Vector2Int(x,y), blockId,direction));
        }
    }
}