using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Shared
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
    }
}
