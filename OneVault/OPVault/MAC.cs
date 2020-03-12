using System;
using System.Security.Cryptography;

namespace OneVault.OPVault
{
    public static class MAC
    {
        public const int MAC_SIZE = 32;

        static public bool CheckMAC(ArraySegment<byte> blob, ArraySegment<byte> mac, byte[] macKey)
        {
            byte[] computedMAC;

            using (HMACSHA256 hmac = new HMACSHA256(macKey))
                computedMAC = hmac.ComputeHash(blob.Array, blob.Offset, blob.Count);

            return ArraySegmentExt.Equals(computedMAC, mac);
        }
    }
}
