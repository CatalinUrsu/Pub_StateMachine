using Cysharp.Threading.Tasks;

namespace StateMachine
{
public class SplashScreenService : IServiceSplashScreen
{
    ISplashScreen _splashScreen;
    IServiceLoadingProgress _loadingProgressService;

    public SplashScreenService(IServiceLoadingProgress loadingProgressService) => _loadingProgressService = loadingProgressService;

    public void RegisterSplashScreen(ISplashScreen panel) => _splashScreen = panel;

    public async UniTask ShowPage(bool skipAnimation = false) => await _splashScreen.ShowPanel(skipAnimation);

    public async UniTask HidePage()
    {
        await _splashScreen.HidePanel();
        _loadingProgressService.ResetProgress();
    }
}
}