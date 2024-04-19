namespace BinarySerializer.PS1
{
    public class TMD_Color : BinarySerializable
    {
        // Note: The color values have a range between 0-2 rather than 0-1
        public SerializableColor Color { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Color = s.SerializeInto<SerializableColor>(Color, BytewiseColor.RGB888, name: nameof(Color));
            s.SerializePadding(1);
        }
    }
}