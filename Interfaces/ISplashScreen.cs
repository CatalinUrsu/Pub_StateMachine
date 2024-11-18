using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface ISplashScreen
{
    public UniTask ShowPanel(bool skipAnimation = false);
    public UniTask HidePanel();
}
}