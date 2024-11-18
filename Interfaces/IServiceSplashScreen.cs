using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IServiceSplashScreen
{
    void RegisterSplashScreen(ISplashScreen panel);
    public UniTask ShowPage(bool skipAnimation = false);
    public UniTask HidePage();
}
}