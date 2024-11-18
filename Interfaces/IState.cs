using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IState
{
    StatesMachine StatesMachine { get; set; }
    UniTask Exit();
}
}