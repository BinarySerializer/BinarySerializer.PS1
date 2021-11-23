using System;

namespace BinarySerializer.PS1
{
    public class PS1_CEL : BinarySerializable
    {
        public byte ID { get; set; }
        public byte Version { get; set; }
        public bool HasATTR { get; set; }
        public bool IsATTR16Bit { get; set; }
        public ushort CellsCount { get; set; }
        public byte CellsWidth { get; set; }
        public byte CellsHeight { get; set; }
        public Cell[] Cells { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<byte>(ID, name: nameof(ID));

            if (ID != 0x22)
                throw new Exception($"Invalid ID {ID}");

            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 0x03)
                throw new Exception($"Invalid Version {Version}");

            s.DoBits<ushort>(b =>
            {
                b.SerializeBits<int>(0, 14, name: "Reserved");
                HasATTR = b.SerializeBits<int>(HasATTR ? 1 : 0, 1, name: nameof(HasATTR)) == 1;
                IsATTR16Bit = b.SerializeBits<int>(IsATTR16Bit ? 1 : 0, 1, name: nameof(IsATTR16Bit)) == 1;
            });

            CellsCount = s.Serialize<ushort>(CellsCount, name: nameof(CellsCount));
            CellsWidth = s.Serialize<byte>(CellsWidth, name: nameof(CellsWidth));
            CellsHeight = s.Serialize<byte>(CellsHeight, name: nameof(CellsHeight));

            Cells = s.SerializeObjectArray<Cell>(Cells, CellsCount, name: nameof(Cells));
        }

        public class Cell : BinarySerializable
        {
            public byte XOffset { get; set; }
            public byte YOffset { get; set; }
            public int ClutX { get; set; }
            public int ClutY { get; set; }
            public bool ABE { get; set; } // Semi-transparency processing, not available prior to version 3

            public override void SerializeImpl(SerializerObject s)
            {
                XOffset = s.Serialize<byte>(XOffset, name: nameof(XOffset));
                YOffset = s.Serialize<byte>(YOffset, name: nameof(YOffset));
                
                s.DoBits<ushort>(b =>
                {
                    ClutX = b.SerializeBits<int>(ClutX, 6, name: nameof(ClutX));
                    ClutY = b.SerializeBits<int>(ClutY, 9, name: nameof(ClutY));
                    ABE = b.SerializeBits<int>(ABE ? 1 : 0, 1, name: nameof(ABE)) == 1;
                });
            }
        }
    }
}