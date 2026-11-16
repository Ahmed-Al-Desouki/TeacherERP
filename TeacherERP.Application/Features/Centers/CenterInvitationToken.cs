using System.Security.Cryptography;
using System.Text;
using System.Buffers.Text;

namespace TeacherERP.Application.Features.Centers
{
    public static class CenterInvitationToken
    {
        public static string Generate() => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));

        public static string Hash(string token)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim())));
    }
}
