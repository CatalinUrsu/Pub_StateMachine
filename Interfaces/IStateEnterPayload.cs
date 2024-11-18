using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IStateEnterPayload<in TPayload> : IState
{
    UniTaskVoid Enter(TPayload payload);
}
}