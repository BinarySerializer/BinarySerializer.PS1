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
        public PS1_VRAMRegion Region { get; set; }
        public byte[] ImgData { get; set; }
        public RGBA5551Color[] ImgData_16 { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.Serialize<byte>(Header, name: nameof(Header));

            if (Header != 16)
                throw new Exception($"Invalid header {Header} - should be 0x10");

            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 0)
                throw new Exception($"Invalid version: {Header} - should be 0x00");

            s.SerializePadding(2, logIfNotNull: true);

            s.DoBits<uint>(b =>
            {
                ColorFormat = b.SerializeBits<TIM_ColorFormat>(ColorFormat, 3, name: nameof(ColorFormat));
                HasClut = b.SerializeBits<bool>(HasClut, 1, name: nameof(HasClut));
                b.SerializePadding(28, logIfNotNull: true);
            });

            if (HasClut)
                Clut = s.SerializeObject<TIM_Clut>(Clut, name: nameof(Clut));

            Length = s.Serialize<int>(Length, name: nameof(Length));
            Region = s.SerializeObject<PS1_VRAMRegion>(Region, name: nameof(Region));

            var imgDataLength = Length - 12; // width * height * 2

            if (Region.Width == 0 || Region.Height == 0)
                imgDataLength = 4;

            if (ColorFormat == TIM_ColorFormat.BPP_4 || ColorFormat == TIM_ColorFormat.BPP_8)
                ImgData = s.SerializeArray<byte>(ImgData, imgDataLength, name: nameof(ImgData));
            else if (ColorFormat == TIM_ColorFormat.BPP_16)
                ImgData_16 = s.SerializeObjectArray<RGBA5551Color>(ImgData_16, Region.Width * Region.Height, name: nameof(ImgData_16));
            else
                throw new NotImplementedException("Raw 24-bit image data is currently not supported");
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
            public PS1_VRAMRegion Region { get; set; }
            public RGBA5551Color[] Palette { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Length = s.Serialize<int>(Length, name: nameof(Length));
                Region = s.SerializeObject<PS1_VRAMRegion>(Region, name: nameof(Region));
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, Region.Width * Region.Height, name: nameof(Palette));
            }
        }
    }
}