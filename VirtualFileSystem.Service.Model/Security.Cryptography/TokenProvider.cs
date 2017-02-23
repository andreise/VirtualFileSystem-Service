using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

namespace VirtualFileSystem.Service.Model.Security.Cryptography
{

    internal sealed class TokenProvider
    {
        public const int TokenLength512 = 64;

        public IReadOnlyList<int> ValidTokenLengths = new ReadOnlyCollection<int>(
            new int[]
            {
                TokenLength512
            }
        );

        private TokenProvider()
        {
        }

        private static Lazy<TokenProvider> defaultInstance = new Lazy<TokenProvider>(() => new TokenProvider());

        public static TokenProvider Default => defaultInstance.Value;

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <remarks>
        /// Needs for the guaranted static fields initialization in a multithreading work
        /// </remarks>
        static TokenProvider()
        {
        }

        public void ValidateToken(byte[] token)
        {
            if ((object)token == null)
                throw new ArgumentNullException(paramName: nameof(token));

            if (token.Length == 0)
                throw new ArgumentException(paramName: nameof(token), message: "Token is empty.");

            if (!this.ValidTokenLengths.Contains(token.Length))
                throw new ArgumentException(paramName: nameof(token), message: "Token has an invalid length.");
        }

        public byte[] GenerateToken()
        {
            byte[] token = new byte[this.ValidTokenLengths[0]];

            using (RandomNumberGenerator tokenGenerator = RandomNumberGenerator.Create())
            {
                tokenGenerator.GetBytes(token);
            }

            return token;
        }

        public bool IsEqualTokens(byte[] token1, byte[] token2)
        {
            ValidateToken(token1);
            ValidateToken(token2);

            if (token1.Length != token2.Length)
                return false;

            for (int i = 0; i < token1.Length; i++)
            {
                if (token1[i] != token2[i])
                    return false;
            }

            return true;
        }
    }

}
