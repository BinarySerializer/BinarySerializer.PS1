namespace BinarySerializer.PS1
{
    public class PS1_TMD_Color : BinarySerializable
    {
        public RGB888Color Color { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Color = s.SerializeObject<RGB888Color>(Color, name: nameof(Color));
            s.SerializePadding(1);
        }
    }
}