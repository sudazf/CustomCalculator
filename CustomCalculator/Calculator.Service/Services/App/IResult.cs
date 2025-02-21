using System;

namespace Calculator.Service.Services.App
{
    public interface IResult
    {
        event EventHandler<object> OnResultChanged;
        object Result { get; }
    }
}
