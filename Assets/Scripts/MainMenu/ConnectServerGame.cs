﻿using System;
using System.Net;
using System.Net.Sockets;
using MainMenu.PopUp;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class ConnectServerGame : MonoBehaviour
    {
        [SerializeField] private InputField serverIp;
        [SerializeField] private InputField serverPort;

        [SerializeField] private ServerConnectPopup serverConnectPopup;
        
        [SerializeField] private Button connectButton;

        private void Start()
        {
            connectButton.onClick.AddListener(Connect);
        }

        private void Connect()
        {
            if (!IPAddress.TryParse(serverIp.text, out var address))
            {
                serverConnectPopup.SetText("IPアドレスが正しくありません");
                return;
            }

            var port = int.Parse(serverPort.text);
            if (65535 < port)
            {
                serverConnectPopup.SetText("ポート番号は65535以下である必要があります");
                return;
            }
            if (port <= 1024)
            {
                serverConnectPopup.SetText("ポート番号は1025異常である必要があります");
                return;
            }

            try
            {
                var remoteEndPoint = new IPEndPoint(address, port);
                var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                socket.Connect(remoteEndPoint);

                if (socket.Connected)
                {
                    //接続が確認出来たので実際にゲームに移行
                }
            }
            catch (Exception e)
            {
                serverConnectPopup.SetText("サーバーへの接続に失敗しました\n"+e);
                return;
            }
        }
    }
}