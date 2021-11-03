namespace BinarySerializer.PS1
{
    public class PS1_TMD_Bone : BinarySerializable
    {
        public int VerticesIndex { get; set; }
        public int VerticesCount { get; set; }
        public int NormalsIndex { get; set; }
        public int NormalsCount { get; set; }
        public int ParentIndex { get; set; } // Index - 1, 0 is no parent

        public override void SerializeImpl(SerializerObject s)
        {
            VerticesIndex = s.Serialize<int>(VerticesIndex, name: nameof(VerticesIndex));
            VerticesCount = s.Serialize<int>(VerticesCount, name: nameof(VerticesCount));
            NormalsIndex = s.Serialize<int>(NormalsIndex, name: nameof(NormalsIndex));
            NormalsCount = s.Serialize<int>(NormalsCount, name: nameof(NormalsCount));
            ParentIndex = s.Serialize<int>(ParentIndex, name: nameof(ParentIndex));
        }
    }
}