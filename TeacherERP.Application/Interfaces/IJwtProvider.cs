using System;
using System.Collections.Generic;
using System.Text;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IJwtProvider
    {
        Task<(string Token, string JwtId)> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
