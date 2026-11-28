using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Shared
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public Error Error { get; }
        public object? Details { get; }

        protected Result(bool isSuccess, Error error, T? value = default, object? details = null)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException();
            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
            Value = value;
            Details = details;
        }

        public static Result<T> Success(T value) => new(true, Error.None, value);
        public static Result<T> Failure(Error error) => new(false, error);
        public static Result<T> Failure(Error error, object details) => new(false, error, details: details);
    }
}
