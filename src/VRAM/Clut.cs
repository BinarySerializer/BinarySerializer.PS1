namespace BinarySerializer.PS1
{
    public class Clut : BinarySerializable
    {
        public SerializableColor[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeIntoArray<SerializableColor>(Palette, 256, BitwiseColor.RGBA5551, name: nameof(Palette));
        }
    }
}