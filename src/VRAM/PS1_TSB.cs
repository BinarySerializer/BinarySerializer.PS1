namespace BinarySerializer.PS1
{
    public class PS1_TSB : BinarySerializable
    {
        // See http://hitmen.c02.at/files/docs/psx/psx.pdf page 37
        public byte TX { get; set; } // value * 64
        public byte TY { get; set; } // value * 256
        public byte ABR { get; set; } // Semi transparency mode
        public TexturePageTP TP { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                TX = (byte)bitFunc(TX, 4, name: nameof(TX));
                TY = (byte)bitFunc(TY, 1, name: nameof(TY));
                ABR = (byte)bitFunc(ABR, 2, name: nameof(ABR));
                TP = (TexturePageTP)bitFunc((byte)TP, 2, name: nameof(TP));
            });
        }

        public enum TexturePageTP : byte
        {
            CLUT_4Bit = 0,
            CLUT_8Bit = 1,
            Direct_15Bit = 2,
        }
    }
}