using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public class Security
{
    public static string SHA256Hash(string InputText)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(InputText)));
    }

    public static byte[] SHA256HashToBytes(string InputText)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(InputText));
    }

    public static string SHA1Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return default;

        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(input));
        return string.Concat(hash.Select(b => b.ToString("x2")));
    }
}
