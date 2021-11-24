namespace BinarySerializer.PS1
{
    public class PS1_CBA : BinarySerializable
    {
        public int ClutX { get; set; }
        public int ClutY { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<ushort>(b =>
            {
                ClutX = b.SerializeBits<int>(ClutX, 6, name: nameof(ClutX));
                ClutY = b.SerializeBits<int>(ClutY, 9, name: nameof(ClutY));
                b.SerializePadding(1, logIfNotNull: true);
            });
        }
    }
}