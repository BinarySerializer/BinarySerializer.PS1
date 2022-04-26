using System;

namespace BinarySerializer.PS1
{
    public class PS1_CBA : BinarySerializable, IEquatable<PS1_CBA>
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

        public override bool UseShortLog => true;
        public override string ToString() => $"CBA({ClutX}, {ClutY})";

        #region Equality
        public override bool Equals(object other) {
            if (other is PS1_CBA)
                return Equals((PS1_CBA)other);
            else
                return false;
        }

        public bool Equals(PS1_CBA other) {
            if (other == null) return false;
            if (other.ClutX != ClutX || other.ClutY != ClutY)
                return false;
            return true;
        }

        public override int GetHashCode() {
            return (ClutX, ClutY).GetHashCode();
        }

        public static bool operator ==(PS1_CBA term1, PS1_CBA term2) {
            if ((object)term1 == null) {
                return ((object)term2 == null);
            }
            return term1.Equals(term2);
        }

        public static bool operator !=(PS1_CBA term1, PS1_CBA term2) {
            return !(term1 == term2);
        }
        #endregion
    }
}