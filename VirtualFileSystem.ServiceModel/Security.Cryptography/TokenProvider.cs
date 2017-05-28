using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;

namespace VirtualFileSystem.ServiceModel.Security.Cryptography
{

    internal sealed class TokenProvider
    {

        public const int TokenLength512 = 64;

        public IReadOnlyList<int> ValidTokenLengths = new ReadOnlyCollection<int>(
            new[]
            {
                TokenLength512
            }
        );

        private TokenProvider()
        {
        }

        private static readonly Lazy<TokenProvider> defaultInstance = new Lazy<TokenProvider>(() => new TokenProvider());

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

        public void ValidateToken(IReadOnlyList<byte> token)
        {
            if (token is null)
                throw new ArgumentNullException(paramName: nameof(token));

            if (token.Count == 0)
                throw new ArgumentException(paramName: nameof(token), message: "Token is empty.");

            if (!this.ValidTokenLengths.Contains(token.Count))
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

        public bool EqualTokens(IReadOnlyList<byte> token1, IReadOnlyList<byte> token2)
        {
            ValidateToken(token1);
            ValidateToken(token2);

            return
                token1.Count == token2.Count &&
                token1.SequenceEqual(token2);
        }

    }

}
