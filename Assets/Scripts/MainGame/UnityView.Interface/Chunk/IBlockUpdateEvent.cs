﻿using UnityEngine;

namespace MainGame.UnityView.Interface.Chunk
{
    public interface IBlockUpdateEvent
    {
        public delegate void OnBlockPlaceEvent(Vector2Int blockPosition,int blockId);
        public delegate void OnBlockRemoveEvent(Vector2Int blockPosition);
        
        public void Subscribe(OnBlockPlaceEvent onBlockPlaceEvent,OnBlockRemoveEvent onBlockRemoveEvent);
        public void UnSubscribe(OnBlockPlaceEvent onBlockPlaceEvent,OnBlockRemoveEvent onBlockRemoveEvent);
    }
}