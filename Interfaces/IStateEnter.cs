using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IStateEnter : IState
{
    UniTaskVoid Enter();
}
}