﻿using DotNetty.Buffers;
using nMqtt.Protocol;

namespace nMqtt.Packets
{
    /// <summary>
    /// 发布消息
    /// </summary>
    [PacketType(PacketType.PUBLISH)]
    internal sealed class PublishPacket : PacketWithId
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string TopicName { get; set; }
        /// <summary>
        /// 有效载荷
        /// </summary>
        public byte[] Payload { get; set; }

        internal PublishPacket() { }

        public PublishPacket(MqttQos qos, bool dup = false, bool retain = false)
        {
            FixedHeader.Qos = qos;
            FixedHeader.Dup = dup;
            FixedHeader.Retain = retain;
        }

        public override void Encode(IByteBuffer buffer)
        {
            var buf = Unpooled.Buffer();
            try
            {
                buf.WriteString(TopicName);
                if (Qos > MqttQos.AtMostOnce)
                {
                    buf.WriteUnsignedShort(PacketId);
                }
                buf.WriteBytes(Payload, 0, Payload.Length);

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

        public override void Decode(IByteBuffer buffer)
        {
            //variable header
            TopicName = buffer.ReadString();
            if (Qos == MqttQos.AtLeastOnce || Qos == MqttQos.ExactlyOnce)
                PacketId = buffer.ReadUnsignedShort();

            //playload
            var len = RemaingLength - (TopicName.Length + 2);
            Payload = new byte[len];
            buffer.ReadBytes(Payload, 0, len);
        }
    }

    /// <summary>
    /// 发布回执
    /// QoS level = 1
    /// </summary>
    [PacketType(PacketType.PUBACK)]
    internal sealed class PubAckPacket : PacketWithId
    {
        public PubAckPacket(ushort packetId = default)
        {
            PacketId = packetId;
        }
    }

    /// <summary>
    /// QoS2消息回执
    /// QoS 2 publish received, part 1
    /// </summary>
    [PacketType(PacketType.PUBREC)]
    internal sealed class PubRecPacket : PacketWithId
    {
        public PubRecPacket(ushort packetId = default)
        {
            PacketId = packetId;
        }
    }

    /// <summary>
    /// QoS2消息释放
    /// QoS 2 publish received, part 2
    /// </summary>
    [PacketType(PacketType.PUBREL)]
    internal sealed class PubRelPacket : PacketWithId
    {
        public PubRelPacket(ushort packetId = default)
        {
            PacketId = packetId;
        }
    }

    /// <summary>
    /// QoS2消息完成
    /// QoS 2 publish received, part 3
    /// </summary>
    [PacketType(PacketType.PUBCOMP)]
    internal sealed class PubCompPacket : PacketWithId
    {
        public PubCompPacket(ushort packetId = default)
        {
            PacketId = packetId;
        }
    }
}