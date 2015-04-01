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
        bool HandleException<TException>(TException exception, int currentExecutionAttempt) where TException : Exception;        
    }
}
