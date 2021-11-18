﻿using System;
using System.Linq;

namespace BinarySerializer.PS1
{
    public class PS1_TMD : BinarySerializable
    {
        /// <summary>
        /// Indicates if the objects have bones defined in place of the scale
        /// </summary>
        public bool Pre_HasBones { get; set; }

        /// <summary>
        /// Indicates if the objects have a color table defined in the header
        /// </summary>
        public bool Pre_HasColorTable { get; set; }

        public bool Pre_HasBonePositions { get; set; }

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

            Pointer anchor = s.CurrentPointer;
            Objects = s.SerializeObjectArray<PS1_TMD_Object>(Objects, ObjectsCount, x =>
            {
                x.Pre_PointerAnchor = Flags.HasFlag(TMDFlags.FIXP) ? null : anchor;
                x.Pre_HasBones = Pre_HasBones;
                x.Pre_HasColorTable = Pre_HasColorTable;
                x.Pre_HasBonePositions = Pre_HasBonePositions;
            }, name: nameof(Objects));

            // Go to the end of the file
            PS1_TMD_Object lastObj = Objects.LastOrDefault();
            if (lastObj != null)
            {
                // The normals are usually at the end, but not always
                Pointer endPointer = lastObj.Primitives.Last().Offset + lastObj.Primitives.Last().Size;

                if (lastObj.Vertices.Length > 0 && lastObj.Vertices.Last().Offset.FileOffset > endPointer.FileOffset)
                    endPointer = lastObj.Vertices.Last().Offset + lastObj.Vertices.Last().Size;

                if (lastObj.Normals.Length > 0 && lastObj.Normals.Last().Offset.FileOffset > endPointer.FileOffset)
                    endPointer = lastObj.Normals.Last().Offset + lastObj.Normals.Last().Size;

                s.Goto(endPointer);
            }
        }

        [Flags]
        public enum TMDFlags : uint
        {
            None = 0,
            FIXP = 1 << 0,
        }
    }
}