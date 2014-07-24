using System;
namespace FluentBoilerplate.Contexts
{
    public interface IContractCondition
    {
        void Fail();
        bool IsConditionMet();
    }
}
