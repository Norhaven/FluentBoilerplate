using FluentBoilerplate.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBoilerplate.Runtime.Contexts
{
    public interface IBundledContractContext
    {
        IContractBundle Bundle { get; }
    }
}
