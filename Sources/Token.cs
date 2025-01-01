
using System.Collections.Generic;

namespace WG2
{
    public class Token
    {
        public string Value;
        /// <summary>
        /// Tokens that follow this token. First dimension defines how far we seek for next tokens.
        /// For example 1 dimension is just next tokens. But 2 is next tokens and next tokens for the first
        /// next tokens. The same with 3, 4, 5...
        /// </summary>
        public List<Token>[] SubsequentTokens;

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
