using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Contexts
{
    public interface IBundledContractContext<TContext>
    {
        IContractContextBundle Bundle { get; }
        TContext Copy(IContractContextBundle bundle);
    }
}
