﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameConst;
using MainGame.Mod;
using SinglePlay;
using UnityEngine;

namespace Test
{
    public class TexturesFromModeTest : MonoBehaviour
    {
        [SerializeField] private List<Texture2D> textures;

        private void Start()
        {
            textures = ItemTextureLoader.GetItemTexture(ServerConst.ServerModsDirectory,new SinglePlayInterface(ServerConst.ServerModsDirectory))
                .Select(i => i.texture2D).ToList();
        }
    }
}