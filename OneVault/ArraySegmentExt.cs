using System;

static internal class ArraySegmentExt
{
    public static byte[] GetByteArray(ArraySegment<byte> a)
    {
        byte[] result = new byte[a.Count];

        Array.Copy(a.Array, a.Offset, result, 0, a.Count);

        return result;
    }

    public static bool Equals(ArraySegment<byte> a1, ArraySegment<byte> a2)
    {
        if (a1 == a2)
            return true;

        if ((a1 != null) && (a2 != null))
        {
            if (a1.Count != a2.Count)
                return false;

            for (int i = 0; i < a1.Count; i++)
            {
                if (a1.Array[a1.Offset + i] != a2.Array[a2.Offset + i])
                    return false;
            }

            return true;
        }

        return false;
    }

    public static bool Equals(ArraySegment<byte> a1, byte[] a2)
    {
        return Equals(a1, new ArraySegment<byte>(a2, 0, a2.Length));
    }

    public static bool Equals(byte[] a1, ArraySegment<byte> a2)
    {
        return Equals(new ArraySegment<byte>(a1, 0, a1.Length), a2);
    }

    public static bool Equals(byte[] a1, byte[] a2)
    {
        return Equals(new ArraySegment<byte>(a1, 0, a1.Length), new ArraySegment<byte>(a2, 0, a2.Length));
    }

}

