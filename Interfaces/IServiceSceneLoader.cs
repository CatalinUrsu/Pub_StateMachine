using Cysharp.Threading.Tasks;

namespace StateMachine
{
public interface IServiceSceneLoader
{
    /// <summary>
    /// Load scene asynchronously using Addressable or resource manager
    /// </summary>
    /// <param name="sceneLoadParams">List of parameters (name, progress prompt, if scene IsAddressable...)</param>
    /// <returns></returns>
    UniTask<SceneLoadResult> LoadScene(SceneLoadParams sceneLoadParams);
    
    /// <summary>
    /// Unload scene asynchronously by name. If scene was cached in dictionary, then use <b>Addressable</b>
    /// If not (ex: init scene), then it is removed using <b>SceneManager</b>
    /// </summary>
    UniTask UnloadScene(string sceneName);
}
}