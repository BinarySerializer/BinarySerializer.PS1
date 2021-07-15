namespace BinarySerializer.PS1
{
    public class PS1_TMD_PacketMode : BinarySerializable
    {
        /// <summary>
        /// Brightness calculation at time of texture mapping
        /// 0: On
        /// 1: Off (Draws texture as is)
        /// </summary>
        public bool TGE { get; set; }

        /// <summary>
        /// Translucency processing
        /// 0: On
        /// 1: Off (Draws texture as is)
        /// </summary>
        public bool ABE { get; set; }

        /// <summary>
        /// Texture specification
        /// 0: Off
        /// 1: On
        /// </summary>
        public bool TME { get; set; }

        /// <summary>
        /// Indicates if it's quad, otherwise tri
        /// </summary>
        public bool IsQuad { get; set; }

        /// <summary>
        /// Shading mode
        /// 0: Flat shading
        /// 1: Gouraud shading
        /// </summary>
        public bool IIP { get; set; }

        public PacketModeCODE Code { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            s.SerializeBitValues<byte>(bitFunc =>
            {
                TGE = bitFunc(TGE ? 1 : 0, 1, name: nameof(TGE)) == 1;
                ABE = bitFunc(ABE ? 1 : 0, 1, name: nameof(ABE)) == 1;
                TME = bitFunc(TME ? 1 : 0, 1, name: nameof(TME)) == 1;
                IsQuad = bitFunc(IsQuad ? 1 : 0, 1, name: nameof(IsQuad)) == 1;
                IIP = bitFunc(IIP ? 1 : 0, 1, name: nameof(IIP)) == 1;
                Code = (PacketModeCODE)bitFunc((int)Code, 3, name: nameof(Code));
            });
        }

        public enum PacketModeCODE : byte
        {
            Polygon = 1,
            StraightLine = 2,
            Sprite = 3,
        }
    }
}