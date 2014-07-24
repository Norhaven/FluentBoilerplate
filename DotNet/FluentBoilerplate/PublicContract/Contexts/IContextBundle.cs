using FluentBoilerplate.Providers;
using System;

namespace FluentBoilerplate.Contexts
{
    public interface IContextBundle
    {
        IContextBundle Copy(IPermissionsProvider permissionsProvider = null, 
                            IImmutableErrorContext errorContext = null, 
                            IConnectionAccessProvider serviceProvider = null, 
                            IConnectionAccessProvider dataProvider = null, 
                            ITranslationProvider translationProvider = null, 
                            IValidationProvider validationProvider = null);

        IConnectionAccessProvider Data { get; }
        IImmutableErrorContext Errors { get; }
        IPermissionsProvider Permissions { get; }
        IConnectionAccessProvider Services { get; }
        ITranslationProvider Translation { get; }
        IValidationProvider Validation { get; }
    }
}
