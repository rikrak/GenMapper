namespace GenMapper
{
    public readonly struct IntIdentifier : IEquatable<IntIdentifier>, IConvertible
    {
        private readonly int _id;
        
        public IntIdentifier(int id)
        {
            _id = id;
        }

        public static explicit operator int(IntIdentifier value) => value._id;
        public static explicit operator int?(IntIdentifier? value) => value?._id;
        public static explicit operator IntIdentifier(int value) => new IntIdentifier(value);
        public static explicit operator IntIdentifier?(int? value) => value.HasValue ? new IntIdentifier(value.Value) : null;

        public bool Equals(IntIdentifier other)
        {
            return _id == other._id;
        }

        public override bool Equals(object? obj)
        {
            return obj is IntIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(IntIdentifier left, IntIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IntIdentifier left, IntIdentifier right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return this._id.ToString("d");
        }

        #region IConvertable
        TypeCode IConvertible.GetTypeCode() => Convert.GetTypeCode(this._id);

        bool IConvertible.ToBoolean(IFormatProvider? provider) => this._id != 0;
        byte IConvertible.ToByte(IFormatProvider? provider) => Convert.ToByte(this._id, provider);
        char IConvertible.ToChar(IFormatProvider? provider) => Convert.ToChar(this._id, provider);
        DateTime IConvertible.ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(this._id, provider);
        decimal IConvertible.ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(this._id, provider);
        double IConvertible.ToDouble(IFormatProvider? provider) => Convert.ToDouble(this._id, provider);
        short IConvertible.ToInt16(IFormatProvider? provider) => Convert.ToInt16(this._id, provider);
        int IConvertible.ToInt32(IFormatProvider? provider) => Convert.ToInt32(this._id, provider);
        long IConvertible.ToInt64(IFormatProvider? provider) => Convert.ToInt64(this._id, provider);
        sbyte IConvertible.ToSByte(IFormatProvider? provider) => Convert.ToSByte(this._id, provider);
        float IConvertible.ToSingle(IFormatProvider? provider) => Convert.ToSingle(this._id, provider);
        string IConvertible.ToString(IFormatProvider? provider) => Convert.ToString(this._id, provider);
        object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(this._id, conversionType, provider);
        ushort IConvertible.ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(this._id, provider);
        uint IConvertible.ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(this._id, provider);
        ulong IConvertible.ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(this._id, provider);

        #endregion
    }
}
