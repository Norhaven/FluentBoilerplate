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

using FluentBoilerplate.Messages.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Exceptions
{
    /// <summary>
    /// Represents a failure to locate a service provider
    /// </summary>
    [Serializable]
    public sealed class ServiceProviderNotFoundException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="ServiceProviderNotFoundException"/>
        /// </summary>
        /// <param name="serviceType">The service type</param>
        public ServiceProviderNotFoundException(Type serviceType)
            :base(CommonErrors.ServiceProviderNotFound.WithValues(serviceType.FullName))
        {

        }
    }
}
