using Cysharp.Threading.Tasks;

namespace StateMachine
{
public class SplashScreenService : IServiceSplashScreen
{
    ISplashScreen<ISplashScreenInfo> _splashScreen;
    IServiceLoadingProgress _loadingProgressService;

    public SplashScreenService(IServiceLoadingProgress loadingProgressService) => _loadingProgressService = loadingProgressService;

    public void RegisterSplashScreen<T>(ISplashScreen<T> panel) where T : ISplashScreenInfo => _splashScreen = (ISplashScreen<ISplashScreenInfo>)panel;

    public async UniTask ShowPage<T>(T config) where T : ISplashScreenInfo => await _splashScreen.ShowPanel(config);

    public async UniTask HidePage()
    {
        await _splashScreen.HidePanel();
        _loadingProgressService.ResetProgress();
    }
}
}