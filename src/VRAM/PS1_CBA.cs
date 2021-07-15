namespace BinarySerializer.PS1
{
    public class PS1_CBA : BinarySerializable
    {
        public int ClutX { get; set; }
        public int ClutY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<ushort>(bitFunc =>
            {
                ClutX = bitFunc(ClutX, 6, name: nameof(ClutX));
                ClutY = bitFunc(ClutY, 9, name: nameof(ClutY));
                bitFunc(default, 1, name: "Padding");
            });
        }
    }
}