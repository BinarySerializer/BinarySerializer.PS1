namespace BinarySerializer.PS1
{
    public class PS1_VRAMRegion : BinarySerializable
    {
        public PS1_VRAMRegion() { }

        public PS1_VRAMRegion(short xPos, short yPos, short width, short height)
        {
            XPos = xPos;
            YPos = yPos;
            Width = width;
            Height = height;
        }

        public short XPos { get; set; }
        public short YPos { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            XPos = s.Serialize<short>(XPos, name: nameof(XPos));
            YPos = s.Serialize<short>(YPos, name: nameof(YPos));
            Width = s.Serialize<short>(Width, name: nameof(Width));
            Height = s.Serialize<short>(Height, name: nameof(Height));
        }

        public override bool UseShortLog => true;
        public override string ToString() => $"(x: {XPos}, y:{YPos}, width:{Width}, height:{Height})";
    }
}