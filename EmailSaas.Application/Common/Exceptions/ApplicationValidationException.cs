using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Common.Exceptions
{
    public class ApplicationValidationException : Exception
    {
        public ApplicationValidationException(IEnumerable<ValidationFailure> failures)
            : base("One or more validation failures have occurred.")
        {
            Errors = failures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
