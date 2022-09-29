﻿using System;

namespace BinarySerializer.PS1
{
    public class PS1_TMD_Packet : BinarySerializable
    {
        public bool Pre_HasColorTable { get; set; }

        /// <summary>
        /// Indicates the word length of the 2D drawing primitives that are generated by intermediate processing.
        /// </summary>
        public byte OLen { get; set; }

        /// <summary>
        /// Indicates the length, in words, of the packet data section.
        /// </summary>
        public byte ILen { get; set; }

        public PacketFlags Flags { get; set; }

        public PS1_TMD_PacketMode Mode { get; set; }

        // Data

        public ushort[] RGBIndices { get; set; }
        public PS1_TMD_Color[] RGB { get; set; }
        public ushort[] Vertices { get; set; }
        public ushort[] Normals { get; set; }
        public PS1_TMD_UV[] UV { get; set; }

        public PS1_CBA CBA { get; set; }
        public PS1_TSB TSB { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            OLen = s.Serialize<byte>(OLen, name: nameof(OLen));
            ILen = s.Serialize<byte>(ILen, name: nameof(ILen));
            Flags = s.Serialize<PacketFlags>(Flags, name: nameof(Flags));
            Mode = s.SerializeObject<PS1_TMD_PacketMode>(Mode, name: nameof(Mode));

            // Don't parse if the packet is empty
            if (ILen == 0)
                return;

            var endPointer = s.CurrentPointer + ILen * 4;

            if (Mode.Code == PS1_TMD_PacketMode.PacketModeCODE.Polygon)
            {
                var verticesCount = Mode.IsQuad ? 4 : 3;
                var hasLightSource = !Flags.HasFlag(PacketFlags.LGT);

                var normalsCount = 0;

                if (hasLightSource)
                    normalsCount = Mode.IIP ? verticesCount : 1;

                int rgbCount;

                if (hasLightSource)
                {
                    if (Flags.HasFlag(PacketFlags.GRD))
                        rgbCount = verticesCount;
                    else if (Mode.TME)
                        rgbCount = 0;
                    else
                        rgbCount = 1;
                }
                else
                {
                    rgbCount = Mode.IIP ? verticesCount : 1;
                }

                if (!Pre_HasColorTable)
                {
                    // Check if it has a texture
                    if (Mode.TME)
                    {
                        UV ??= new PS1_TMD_UV[Mode.IsQuad ? 4 : 3];

                        UV[0] = s.SerializeObject<PS1_TMD_UV>(UV[0], name: $"{nameof(UV)}[0]");
                        CBA = s.SerializeObject<PS1_CBA>(CBA, name: nameof(CBA));
                        UV[1] = s.SerializeObject<PS1_TMD_UV>(UV[1], name: $"{nameof(UV)}[1]");
                        TSB = s.SerializeObject<PS1_TSB>(TSB, name: nameof(TSB));
                        UV[2] = s.SerializeObject<PS1_TMD_UV>(UV[2], name: $"{nameof(UV)}[2]");
                        s.Align();

                        if (Mode.IsQuad)
                        {
                            UV[3] = s.SerializeObject<PS1_TMD_UV>(UV[3], name: $"{nameof(UV)}[3]");
                            s.Align();
                        }
                    }

                    RGB = s.SerializeObjectArray<PS1_TMD_Color>(RGB, rgbCount, name: nameof(RGB));

                    Normals ??= new ushort[normalsCount];
                    Vertices ??= new ushort[verticesCount];

                    for (int i = 0; i < Vertices.Length; i++)
                    {
                        if (i < Normals.Length)
                            Normals[i] = s.Serialize<ushort>(Normals[i], name: $"{nameof(Normals)}[{i}]");

                        Vertices[i] = s.Serialize<ushort>(Vertices[i], name: $"{nameof(Vertices)}[{i}]");
                    }

                    s.Align();
                }
                else
                {
                    // TODO: Improve this code. There is no documentation which seems to match this, so it's quite messy and might not always work.

                    // Check if it has a texture
                    if (Mode.TME)
                    {
                        UV ??= new PS1_TMD_UV[Mode.IsQuad ? 4 : 3];

                        UV[0] = s.SerializeObject<PS1_TMD_UV>(UV[0], name: $"{nameof(UV)}[0]");
                        CBA = s.SerializeObject<PS1_CBA>(CBA, name: nameof(CBA));
                        UV[1] = s.SerializeObject<PS1_TMD_UV>(UV[1], name: $"{nameof(UV)}[1]");
                        TSB = s.SerializeObject<PS1_TSB>(TSB, name: nameof(TSB));
                        UV[2] = s.SerializeObject<PS1_TMD_UV>(UV[2], name: $"{nameof(UV)}[2]");

                        if (!Mode.IsQuad && rgbCount != 1)
                            s.Align();

                        if (Mode.IsQuad)
                        {
                            UV[3] = s.SerializeObject<PS1_TMD_UV>(UV[3], name: $"{nameof(UV)}[3]");

                            RGBIndices = s.SerializeArray<ushort>(RGBIndices, rgbCount, name: nameof(RGBIndices));

                            s.Align();
                        }
                        else if (rgbCount == 1)
                        {
                            RGBIndices = s.SerializeArray<ushort>(RGBIndices, rgbCount, name: nameof(RGBIndices));
                            s.Align();
                        }
                    }

                    RGBIndices ??= new ushort[rgbCount];

                    Normals ??= new ushort[normalsCount];
                    Vertices ??= new ushort[verticesCount];

                    if (rgbCount == 0 && normalsCount == verticesCount)
                    {
                        Normals = s.SerializeArray<ushort>(Normals, normalsCount, name: nameof(Normals));
                        Vertices = s.SerializeArray<ushort>(Vertices, verticesCount, name: nameof(Vertices));
                    }
                    else
                    {
                        for (int i = 0; i < Vertices.Length; i++)
                        {
                            if (i < Normals.Length)
                                Normals[i] = s.Serialize<ushort>(Normals[i], name: $"{nameof(Normals)}[{i}]");

                            if ((!Mode.TME || (!Mode.IsQuad && rgbCount != 1)) && i < RGBIndices.Length)
                                RGBIndices[i] = s.Serialize<ushort>(RGBIndices[i], name: $"{nameof(RGBIndices)}[{i}]");

                            Vertices[i] = s.Serialize<ushort>(Vertices[i], name: $"{nameof(Vertices)}[{i}]");
                        }
                    }

                    s.Align();
                }
            }
            else if (Mode.Code == PS1_TMD_PacketMode.PacketModeCODE.StraightLine)
            {
                throw new NotImplementedException("Serialize straight line");
            }
            else if (Mode.Code == PS1_TMD_PacketMode.PacketModeCODE.Sprite)
            {
                throw new NotImplementedException("Serialize 3D sprite");
            }
            else
            {
                throw new BinarySerializableException(this, $"Invalid packet mode code {Mode.Code}");
            }

            // Check to make sure the entire pack was serialized, otherwise correct it. Ignore this if the object has
            // a color table since the length doesn't match the actual packet length then.
            if (s.CurrentPointer != endPointer && !Pre_HasColorTable)
            {
                s.Context.SystemLogger?.LogWarning($"Packet was not fully serialized. Expected end was {endPointer}, end is {s.CurrentPointer}.");
                s.Goto(endPointer);
            }
        }

        [Flags]
        public enum PacketFlags : byte
        {
            None = 0,

            /// <summary>
            /// 1: Light source calculation not carried out
            /// 0: Light source calculation carried out
            /// </summary>
            LGT = 1 << 0,

            /// <summary>
            /// 1: Double-faced polygon
            /// 0: Single-faced polygon
            /// (Valid, only when the CODE value refers to a polygon.)
            /// </summary>
            FCE = 1 << 1,

            /// <summary>
            /// Valid only for the polygon not textured, subjected to light source calculation.
            /// 1: Gradation polygon
            /// 0: Single-color polygon
            /// </summary>
            GRD = 1 << 2,
        }
    }
}