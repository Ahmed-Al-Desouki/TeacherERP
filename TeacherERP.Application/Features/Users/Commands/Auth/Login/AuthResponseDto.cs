namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiryDate);
}