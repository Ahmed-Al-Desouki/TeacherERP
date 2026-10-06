using System;
using System.Collections.Generic;
using System.Text;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        void Add(RefreshToken refreshToken);
    }
}
