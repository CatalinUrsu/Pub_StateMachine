<h1 align="center">
Helpers
</h1>
<div align="center">
Basic application control by states driven by StateMachine. It helps to maintain project logic in a clean and organized way. There are two
types of states - default and with preload. For more control during state changing, the state changing is async.

</div>

# Contents:
- [State Machine 🕹️](#state-machine-)
- [Services 🧰](#services-)
  - [Camera Service 🎥](#camera-service-)
  - [Scene Loader Service ⏱️](#scene-loader-service-)
  - [Loading Progress Tracking Service 📼](#loading-progress-tracking-service-)
  - [Splash Screen Service 🧩](#splash-screen-service-)

# <br>State Machine 🕹️
StateMachine script is used to manage the states of an object, controlling its behavior based on its current state.
It helps in organizing code for different states (init, menu, gameplay, loading, etc). This implementation is based on async changing state.
Firstly it wait till the current state is finished, then it changes to the new state.
```csharp
using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace StateMachine
{
[Serializable]
public class StatesMachine
{
    readonly Dictionary<Type, IState> _states;
    protected IState currentState { get; set; }

    public StatesMachine(params IState[] states)
    {
        _states = new Dictionary<Type, IState>();

        foreach (var state in states)
        {
            if (_states.ContainsKey(state.GetType()))
                continue;

            state.StatesMachine = this;
            _states.Add(state.GetType(), state);
        }
    }
    
    public virtual async UniTask Enter<TState>(Action onLoaded = null) where TState : class, IStateEnter
    {
        var state = await ChangeState<TState>();
        state.Enter().Forget();
    }

    public virtual async UniTask Enter<TState, TPayload>(TPayload payload) where TState : class, IStateEnterPayload<TPayload>
    {
        var state = await ChangeState<TState>();
        state.Enter(payload).Forget();
    }

    protected virtual async UniTask<TState> ChangeState<TState>() where TState : class, IState
    {
        if (currentState != null)
            await currentState.Exit();

        var state = GetState<TState>();
        currentState = state;
        return state;
    }

    protected virtual TState GetState<TState>() where TState : class, IState => _states[typeof(TState)] as TState;
}
}
```


# Services 🧰
Beside the State Machine logic, it also contains some services that can be used in any project, like:
<i>
- [Camera Service](Services/CameraService.cs)
- [Scene Loader](Services/SceneLoaderService.cs)
- [Loading Progress Service](Services/LoadingProgressService.cs)
- [Splash Screen Service](Services/SplashScreenService.cs)
</i>

## Camera Service 🎥
Is used to get access to main camera of to any camera by tag or name. Useful for getting camera for some canvas with render mode set to
screen space camera.

## Scene Loader Service ⏱️
Service used to load and unload any scene using SceneManager or Addressable system. For tracking loading progress, it uses 
<b>LoadingProgressService</b> and <i><b><a href="https://github.com/Cysharp/UniTask?tab=readme-ov-file#progress">IProgress</a></b></i> 
interface implementation from <i><b><a href="https://github.com/Cysharp/UniTask">UniTask</a></b></i> repo.
<br>As a parameter for loading scene it used [Scene Load Params](SceneLoadInfo/SceneLoadParams.cs), that is just a struct with some options for loading 
scene and that can be created using vert friendly linq-style Builder pattern.
<br>As a result of loading scene, it returns [Scene Load Result](SceneLoadInfo/SceneLoadResult.cs) that contains some info about loading scene.

## Loading Progress Tracking Service 📼
This service is used to track loading and unloading of scenes and content on them. For this, it uses [Scene Load Progress](SceneLoadInfo/SceneLoadProgress.cs)
from [Scene Load Result](SceneLoadInfo/SceneLoadResult.cs), this class contains float values for scene loading progress and content loading progress.
<br><b>NOTE:</b> For correct working of loading tracking, need to set total number of scenes and content that will be loaded in the beginning of loading process.

## Splash Screen Service 🧩
Service used to show splash screen during loadding process. It uses [ISplashScreen](Interfaces/ISplashScreen.cs) interface for showing and hiding splash screen.
```csharp
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
```