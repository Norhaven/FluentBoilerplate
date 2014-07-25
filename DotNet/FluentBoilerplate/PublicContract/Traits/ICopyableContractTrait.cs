using FluentBoilerplate.Contexts;
using FluentBoilerplate.Runtime.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBoilerplate.Traits
{
    public interface ICopyableContractTrait<TContext>
    {
        TContext Copy(ContextBundle bundle = null,
                      IContractBundle contractBundle = null);
    }
}
