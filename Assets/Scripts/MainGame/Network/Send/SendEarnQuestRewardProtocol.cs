﻿using System.Linq;
using MainGame.Network.Settings;
using MessagePack;
using Server.Protocol.PacketResponse;

namespace MainGame.Network.Send
{
    public class SendEarnQuestRewardProtocol
    {
        private readonly ISocketSender _socketSender;
        private readonly int _playerId;

        
        public SendEarnQuestRewardProtocol(PlayerConnectionSetting playerConnectionSetting,ISocketSender socketSender)
        {
            _socketSender = socketSender;
            _playerId = playerConnectionSetting.PlayerId;
        }

        public void Send(string questId)
        {
            _socketSender.Send(MessagePackSerializer.Serialize(new EarnQuestRewardMessagePack(
                _playerId,questId)).ToList());
        }
    }
}