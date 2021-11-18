namespace Matrix.Sdk.Sample.Console
{
    using System;
    using Sodium;

    public class CryptographyService
    {
        public string ToHexString(byte[] input)
        {
            var hexString = BitConverter.ToString(input);

            string result = hexString.Replace("-", "");

            return result.ToLower();
        }

        public byte[] GenerateLoginDigest()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
            var message = $"login:{now / 1000 / (5 * 60)}";

            return GenericHash.Hash(message, (byte[]?) null, 32);
        }

        public KeyPair GenerateEd25519KeyPair(string seed)
        {
            byte[] hash = GenericHash.Hash(seed, (byte[]?) null, 32);

            return PublicKeyAuth.GenerateKeyPair(hash);
        }

        public string GenerateHexSignature(byte[] loginDigest, byte[] secretKey)
        {
            byte[] signature = PublicKeyAuth.SignDetached(loginDigest, secretKey);

            return ToHexString(signature);
        }

        public string GenerateHexId(byte[] publicKey)
        {
            byte[] hash = GenericHash.Hash(publicKey, null, publicKey.Length);

            return ToHexString(hash);
        }
    }
}

// public async Task<LoginResponse> LoginAsync(Uri baseAddress, KeyPair keyPair,
//     CancellationToken cancellationToken)
// {
//     byte[] loginDigest = _cryptographyService.GenerateLoginDigest();
//     string hexSignature = _cryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
//     string publicKeyHex = _cryptographyService.ToHexString(keyPair.PublicKey);
//     string hexId = _cryptographyService.GenerateHexId(keyPair.PublicKey);
//
//     var password = $"ed:{hexSignature}:{publicKeyHex}";
//     string deviceId = publicKeyHex;
//
//     var model = new LoginRequest
//     (
//         new Identifier
//         (
//             "m.id.user",
//             hexId
//         ),
//         password,
//         deviceId,
//         "m.login.password"
//     );
//
//     HttpClient httpClient = CreateHttpClient(baseAddress);
//
//     var path = $"{ResourcePath}/login";
//
//     return await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
// }