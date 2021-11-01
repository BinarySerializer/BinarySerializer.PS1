namespace BinarySerializer.PS1
{
    public class PS1_TMD_Part : BinarySerializable
    {
        public int VertexIndex { get; set; }
        public int VerticesCount { get; set; }
        public int NormalIndex { get; set; }
        public int NormalsCount { get; set; }
        public int ParentPartIndex { get; set; } // Index - 1, 0 is no parent

        public override void SerializeImpl(SerializerObject s)
        {
            VertexIndex = s.Serialize<int>(VertexIndex, name: nameof(VertexIndex));
            VerticesCount = s.Serialize<int>(VerticesCount, name: nameof(VerticesCount));
            NormalIndex = s.Serialize<int>(NormalIndex, name: nameof(NormalIndex));
            NormalsCount = s.Serialize<int>(NormalsCount, name: nameof(NormalsCount));
            ParentPartIndex = s.Serialize<int>(ParentPartIndex, name: nameof(ParentPartIndex));
        }
    }
}