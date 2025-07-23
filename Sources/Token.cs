
using System.Collections.Generic;

namespace WG2
{
    public class Token
    {
        public string Value;

        public Token(string value)
        {
            Value = value;
        }

        public static bool operator ==(Token left, Token right)
        {
            if(left is null && right is null)
            {
                return true;
            }
            if(left is null || right is null)
            {
                return false;
            }
            return left.Value == right.Value;
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
