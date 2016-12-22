using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Shared
{
    public class Result
    {
        public static Result<T, E> CreateSuccess<T, E>(T result) => new Result<T, E>(result);
        public static Result<T, E> CreateFailure<T, E>(E error) => new Result<T, E>(error);
    }

    public class Result<T, E>
    {
        private T result;
        private E error;

        public bool IsOk { get; private set; }
        public bool IsError => !IsOk;

        internal Result(T value)
        {
            this.result = value;
            this.error = default(E);
            IsOk = true;
        }

        internal Result(E error)
        {
            this.result = default(T);
            this.error = error;
            IsOk = false;
        }

        /// <exception cref="System.InvalidOperationException">Thrown when there is no result but an error instead</exception>
        public T GetResult()
        {
            if (IsOk) return result;
            throw new InvalidOperationException($"No result exists because there is error {error}");
        }

        /// <exception cref="System.InvalidOperationException">Thrown when there is no error but a result instead</exception>
        public E GetError()
        {
            if (IsError) return error;
            throw new InvalidOperationException($"No error exists because there is a result {result}");
        }
    }
}
