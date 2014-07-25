using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Contexts
{
    public interface IContractBundle
    {
        IImmutableQueue<IContractCondition> Preconditions { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnReturn { get; }
        IImmutableQueue<IContractCondition> PostconditionsOnThrow { get; }
        IImmutableQueue<Action> InstanceValidations { get; }

        IContractBundle AddPrecondition(IContractCondition condition);
        IContractBundle AddPostconditionOnReturn(IContractCondition condition);
        IContractBundle AddPostconditionOnThrow(IContractCondition condition);
        IContractBundle AddInstanceValidation(Action action);
    }
}
