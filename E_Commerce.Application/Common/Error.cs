using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace E_Commerce.Application.Common
{
    public sealed record Error(string Code, string Description, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "General Failure has Occurred")
            => new Error(code, description, ErrorType.Failure);

        public static Error Validation(string code = "General.Validation", string description = "General Validation Error  has Occurred")
           => new Error(code, description, ErrorType.Validation);
        public static Error Forbidden(string code = "General.Forbidden", string description = "This Operation Is Forbidden")
          => new Error(code, description, ErrorType.Forbidden);
        public static Error Conflict(string code = "General.Conflict", string description = "General Conflict  has Occurred")
          => new Error(code, description, ErrorType.Conflict);
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "Access Is Denied Due to Bad Authorization")
          => new Error(code, description, ErrorType.Unauthorized);
        public static Error InValidCreadntials(string code = "General.InValidCreadntials", string description = "Provide Credentials are Invalid")
          => new Error(code, description, ErrorType.InValidCreadntials);
        public static Error NotFound(string code = "General.NotFound", string description = "Resorce NotFound")
         => new Error(code, description, ErrorType.NotFound);

    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        InValidCreadntials = 6,
    }
}
