namespace TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp
{
    public record ResetTokenDto(string ResetToken, int ExpiresInSeconds);
}