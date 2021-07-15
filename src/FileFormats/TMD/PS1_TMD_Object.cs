namespace BinarySerializer.PS1
{
    public class PS1_TMD_Object : BinarySerializable
    {
        public Pointer Pre_PointerAnchor { get; set; }

        public Pointer VerticesPointer { get; set; }
        public uint VerticesCount { get; set; }
        public Pointer NormalsPointer { get; set; }
        public uint NormalsCount { get; set; }
        public Pointer PrimitivesPointer { get; set; }
        public uint PrimitivesCount { get; set; }
        public int Scale { get; set; } // Raised to the second power

        // Serialized from pointers
        public PS1_TMD_Vertex[] Vertices { get; set; }
        public PS1_TMD_Normal[] Normals { get; set; }
        public PS1_TMD_Packet[] Primitives { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            VerticesPointer = s.SerializePointer(VerticesPointer, anchor: Pre_PointerAnchor, name: nameof(VerticesPointer));
            VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
            NormalsPointer = s.SerializePointer(NormalsPointer, anchor: Pre_PointerAnchor, name: nameof(NormalsPointer));
            NormalsCount = s.Serialize<uint>(NormalsCount, name: nameof(NormalsCount));
            PrimitivesPointer = s.SerializePointer(PrimitivesPointer, anchor: Pre_PointerAnchor, name: nameof(PrimitivesPointer));
            PrimitivesCount = s.Serialize<uint>(PrimitivesCount, name: nameof(PrimitivesCount));
            Scale = s.Serialize<int>(Scale, name: nameof(Scale));

            s.DoAt(VerticesPointer, () => Vertices = s.SerializeObjectArray<PS1_TMD_Vertex>(Vertices, VerticesCount, name: nameof(Vertices)));
            s.DoAt(NormalsPointer, () => Normals = s.SerializeObjectArray<PS1_TMD_Normal>(Normals, NormalsCount, name: nameof(Normals)));
            s.DoAt(PrimitivesPointer, () => Primitives = s.SerializeObjectArray<PS1_TMD_Packet>(Primitives, PrimitivesCount, name: nameof(Primitives)));
        }
    }
}