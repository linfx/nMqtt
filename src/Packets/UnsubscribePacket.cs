﻿using System.Collections.Generic;
using DotNetty.Buffers;
using nMqtt.Protocol;

namespace nMqtt.Packets
{
    /// <summary>
    /// 取消订阅
    /// </summary>
    [PacketType(PacketType.UNSUBSCRIBE)]
    public sealed class UnsubscribePacket : PacketWithId
    {
        List<string> _topics = new List<string>();

        public override void Encode(IByteBuffer buffer)
        {
            var buf = Unpooled.Buffer();
            try
            {
                buf.WriteShort(PacketId);

                foreach (var item in _topics)
                {
                    buf.WriteString(item);
                }

                FixedHeader.RemaingLength = buf.ReadableBytes;
                FixedHeader.WriteTo(buffer);
                buffer.WriteBytes(buf);
                buf = null;
            }
            finally
            {
                buf?.Release();
            }
        }

        public void AddRange(params string[] topics)
        {
            _topics.AddRange(topics);
        }
    }

    /// <summary>
    /// 取消订阅回执
    /// </summary>
    [PacketType(PacketType.UNSUBACK)]
    public sealed class UnsubscribeAckMessage : Packet
    {
    }
}