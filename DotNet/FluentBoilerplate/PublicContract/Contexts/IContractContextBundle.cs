using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Contexts
{
    public interface IContractContextBundle
    {
        IImmutableQueue<IContractCondition> Preconditions { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnReturn { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnThrow { get; }
        IImmutableQueue<Action> InstanceValidations { get; }

        IContractContextBundle Merge(IImmutableQueue<IContractCondition> preconditions = null,
                                     IImmutableQueue<IContractCondition> postconditionsOnReturn = null,
                                     IImmutableQueue<IContractCondition> postconditionsOnThrow = null,
                                     IImmutableQueue<Action> instanceValidations = null);
    }
}
