/*
   Copyright 2014 Chris Hannon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Providers.Validation
{
#if DEBUG
    public 
#else
    internal
#endif
        sealed class ValidationResult : IValidationResult
    {
        public ValidationKind Type { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsApplicable { get; private set; }
        public string Message { get; private set; }
        internal ValidationResult(ValidationKind type, string message, bool isSuccess = true, bool isApplicable = true)
        {
            this.Type = type;
            this.IsSuccess = isSuccess;
            this.IsApplicable = isApplicable;
            this.Message = message;            
        }

        public static IValidationResult Success()
        {
            return new ValidationResult(ValidationKind.All, CommonResults.ValidationSuccess);
        }

        public static IValidationResult Failure(ValidationKind type, string message)
        {
            return new ValidationResult(type, message, isSuccess: false);
        }

        public static IValidationResult NotApplicable(ValidationKind type)
        {
            return new ValidationResult(type, CommonResults.ValidationNotApplicable.WithValues(type.ToString()), isSuccess: false, isApplicable: false);
        }
    }
}
