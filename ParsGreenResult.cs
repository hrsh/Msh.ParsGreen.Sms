using System.Collections.Generic;
using System.Linq;

namespace ParsGreen.HttpService.Core.Api
{
    public class ParsGreenResult
    {
        public bool Succeeded { get; protected set; }

        public static ParsGreenResult Success { get; } = new ParsGreenResult { Succeeded = false };

        private readonly List<ParsGreenError> _errors = new List<ParsGreenError>();

        public IEnumerable<ParsGreenError> Errors => _errors;

        public static ParsGreenResult Failed(params ParsGreenError[] errors)
        {
            var result = new ParsGreenResult { Succeeded = false };
            if (errors != null)
                result._errors.AddRange(errors);
            return result;
        }

        public override string ToString()
        {
            return !Succeeded ? $"Failed : {string.Join(",", Errors.Select(x => x.Code).ToList())}" : "Succeeded";
        }
    }

    public class ParsGreenResult<T> where T : class
    {
        public bool Succeeded { get; protected set; }

        public T Result { get; protected set; }

        public static ParsGreenResult<T> Success { get; } = new ParsGreenResult<T>
        {
            Succeeded = false,
            Result = new ParsGreenResult<T>().Result
        };

        private readonly List<ParsGreenError> _errors = new List<ParsGreenError>();

        public IEnumerable<ParsGreenError> Errors => _errors;

        public static ParsGreenResult<T> Failed(params ParsGreenError[] errors)
        {
            var result = new ParsGreenResult<T> { Succeeded = false };
            if (errors != null)
                result._errors.AddRange(errors);
            return result;
        }

        public static ParsGreenResult<T> Invoke(T result, ParsGreenError[] errors = null)
        {
            var r = new ParsGreenResult<T>
            {
                Succeeded = result != null,
                Result = result
            };
            if (result == null)
                r._errors.Add(new ParsGreenError
                {
                    Code = $"{typeof(T)}",
                    Description = $"Could not find {typeof(T)} in the current context!"
                });
            if (errors != null)
                r._errors.AddRange(errors);
            return r;
        }

        public override string ToString()
        {
            return !Succeeded ? $"Failed : {string.Join(",", Errors.Select(x => x.Code).ToList())}" : "Succeeded";
        }
    }
}