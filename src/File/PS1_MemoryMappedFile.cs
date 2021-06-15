namespace BinarySerializer.PS1
{
    /// <summary>
    /// A memory mapped file with the option to allow unmapped developer pointers
    /// </summary>
    public class PS1_MemoryMappedFile : MemoryMappedFile 
    {
        public PS1_MemoryMappedFile(Context context, string filePath, uint baseAddress, InvalidPointerMode currentInvalidPointerMode, Endian endianness = Endian.Little, long fileLength = 0) : base(context, filePath, baseAddress, endianness, fileLength)
        {
            CurrentInvalidPointerMode = currentInvalidPointerMode;
        }

        public InvalidPointerMode CurrentInvalidPointerMode { get; }

		private static bool CheckIfDevPointer(uint serializedValue, Pointer anchor = null) 
        {
			var anchorOffset = anchor?.AbsoluteOffset ?? 0;
			var offset = serializedValue + anchorOffset;
			offset ^= 0xFFFFFFFF;
			return offset >= 0x80000000 && offset < 0x807FFFFF;
		}

		public override bool AllowInvalidPointer(long serializedValue, Pointer anchor = null) => CurrentInvalidPointerMode switch
        {
            InvalidPointerMode.DevPointerXOR => CheckIfDevPointer((uint)serializedValue, anchor: anchor),
            InvalidPointerMode.Allow => true,
            _ => true
        };

        public enum InvalidPointerMode 
        {
			DevPointerXOR,
			Allow
		}
    }
}