using System;
using JetBrains.Annotations;

namespace BinarySerializer.PS1
{
    public class PS1_TMD : BinarySerializable
    {
        // Header
        public uint ID { get; set; }
        public TMDFlags Flags { get; set; }
        public uint ObjectsCount { get; set; }

        // Obj table
        public PS1_TMD_Object[] Objects { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // HEADER

            ID = s.Serialize<uint>(ID, name: nameof(ID));

            if (ID != 0x41)
                throw new BinarySerializableException(this, $"Unsupported TMD version {ID}");

            Flags = s.Serialize<TMDFlags>(Flags, name: nameof(Flags));
            ObjectsCount = s.Serialize<uint>(ObjectsCount, name: nameof(ObjectsCount));

            // OBJ TABLE

            var anchor = s.CurrentPointer;
            Objects = s.SerializeObjectArray<PS1_TMD_Object>(Objects, ObjectsCount, x => x.Pre_PointerAnchor = Flags.HasFlag(TMDFlags.FIXP) ? null : anchor, name: nameof(Objects));
        }

        [Flags]
        public enum TMDFlags : uint
        {
            None = 0,
            FIXP = 1 << 0,
        }
    }
}