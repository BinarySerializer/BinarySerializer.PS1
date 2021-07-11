using System;

namespace BinarySerializer.PS1
{
    public class PS1_TIM : BinarySerializable
    {
        public byte Header { get; set; }
        public byte Version { get; set; }
        public TIM_ColorFormat ColorFormat { get; set; }
        public bool HasClut { get; set; }
        public TIM_Clut Clut { get; set; }
        public int Length { get; set; }
        public ushort XPos { get; set; }
        public ushort YPos { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte[] ImgData { get; set; }
        public RGBA5551Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.Serialize<byte>(Header, name: nameof(Header));

            if (Header != 16)
                throw new Exception($"Invalid header {Header} - should be 0x10");

            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 0)
                throw new Exception($"Invalid version: {Header} - should be 0x00");

            s.SerializePadding(2, logIfNotNull: true);

            s.SerializeBitValues<uint>(bitFunc =>
            {
                ColorFormat = (TIM_ColorFormat)bitFunc((int)ColorFormat, 3, name: nameof(ColorFormat));
                HasClut = bitFunc(HasClut ? 1 : 0, 1, name: nameof(HasClut)) == 1;
            });

            if (HasClut)
                Clut = s.SerializeObject<TIM_Clut>(Clut, name: nameof(Clut));

            Length = s.Serialize<int>(Length, name: nameof(Length));
            XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
            YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            if (ColorFormat == TIM_ColorFormat.BPP_4 || ColorFormat == TIM_ColorFormat.BPP_8)
                ImgData = s.SerializeArray<byte>(ImgData, Length - 12, name: nameof(ImgData)); // Width * Height * 2
            else
                throw new NotImplementedException("Raw image data is currently not supported");
        }

        public enum TIM_ColorFormat : byte
        {
            BPP_4 = 0,
            BPP_8 = 1,
            BPP_16 = 2,
            BPP_24 = 3,
        }

        public class TIM_Clut : BinarySerializable
        {
            public int Length { get; set; }
            public ushort XPos { get; set; }
            public ushort YPos { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public RGBA5551Color[] Palette { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Length = s.Serialize<int>(Length, name: nameof(Length));
                XPos = s.Serialize<ushort>(XPos, name: nameof(XPos));
                YPos = s.Serialize<ushort>(YPos, name: nameof(YPos));
                Width = s.Serialize<ushort>(Width, name: nameof(Width));
                Height = s.Serialize<ushort>(Height, name: nameof(Height));
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, Width * Height, name: nameof(Palette));
            }
        }
    }
}