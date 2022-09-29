namespace BinarySerializer.PS1
{
    public class PS1_TMD_UV : BinarySerializable, ISerializerShortLog
    {
        public byte U { get; set; }
        public byte V { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            U = s.Serialize<byte>(U, name: nameof(U));
            V = s.Serialize<byte>(V, name: nameof(V));
        }

        public string ShortLog => ToString();
        public override string ToString() => $"UV({U}, {V})";
    }
}