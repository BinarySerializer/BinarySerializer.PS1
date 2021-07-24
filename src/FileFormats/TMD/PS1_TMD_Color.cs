namespace BinarySerializer.PS1
{
    public class PS1_TMD_Color : BinarySerializable
    {
        public RGB777Color Color { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Color = s.SerializeObject<RGB777Color>(Color, name: nameof(Color));
            s.SerializePadding(1);
        }
    }
}