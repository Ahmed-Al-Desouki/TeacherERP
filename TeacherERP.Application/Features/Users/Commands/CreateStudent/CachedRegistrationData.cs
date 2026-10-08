using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    public class CachedRegistrationData
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
