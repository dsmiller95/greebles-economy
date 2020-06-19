using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface GenericStateHandler<T, ParamType> where T : Enum
{
    T stateHandle { get; }
    Task<T> HandleState(ParamType data);
    T validPreviousStates { get; }
    void TransitionIntoState(ParamType data);
    T validNextStates { get; }
    void TransitionOutOfState(ParamType data);
}
