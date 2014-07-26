using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{
#if DEBUG
    public
#else
    internal 
#endif
        interface IExceptionAwareAction
    {
        void Do();
        void HandleException<TException>(TException exception) where TException : Exception;
    }
}
