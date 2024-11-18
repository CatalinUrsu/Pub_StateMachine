using System;
using UnityEngine;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace StateMachine
{
public class SceneLoaderService : IServiceSceneLoader
{
#region Fieds

    readonly Dictionary<string, SceneInstance> _loadedAddressableScene = new();
    readonly IServiceLoadingProgress _serviceLoadingProgress;

#endregion

#region Public methods

    public SceneLoaderService(IServiceLoadingProgress serviceLoadingProgress) => _serviceLoadingProgress = serviceLoadingProgress;
    
    /// <summary>
    /// Before load scene which loading progress should be tracked, WhileLoadProgressCount in IServiceLoadingProgress should be set to total number of scenes to load for correct progress tracking 
    /// </summary>
    /// <returns></returns>
    public async UniTask<SceneLoadResult> LoadScene(SceneLoadParams sceneLoadParams)
    {
        if (sceneLoadParams.TrackProgress)
            Assert.AreNotEqual(_serviceLoadingProgress.LoadProgressCount, 0, "Before load scene which loading progress should be tracked, WhileLoadProgressCount in IServiceLoadingProgress should " +
                                                                             "be set to total number of scenes to load for correct progress tracking");
        
        if (!String.IsNullOrEmpty(sceneLoadParams.Prompt))
            _serviceLoadingProgress.UpdateLoadingPrompt(sceneLoadParams.Prompt);

        return await GetSceneLoadResult(sceneLoadParams);
    }

    public async UniTask UnloadScene(string sceneName)
    {
        try
        {
            await (_loadedAddressableScene.ContainsKey(sceneName)
                ? UnloadByAddressable(sceneName)
                : UnloadBySceneManager(sceneName));
        }
        catch (Exception e)
        {
            Debug.LogError($"Something goes wrong during scene unloading: {e.Message}.");
        }
    }

#endregion

#region Private methods

    UniTask<SceneLoadResult> GetSceneLoadResult(SceneLoadParams sceneLoadParams)
    {
        return sceneLoadParams.IsAddressable
            ? LoadAddressableScene(sceneLoadParams)
            : LoadBuildScene(sceneLoadParams);
    }

    async UniTask<SceneLoadResult> LoadAddressableScene(SceneLoadParams sceneLoadParams)
    {
        var loadingSceneData = new SceneLoadResult();

        if (_loadedAddressableScene.ContainsKey(sceneLoadParams.SceneName))
        {
            loadingSceneData.LoadedScene = _loadedAddressableScene[sceneLoadParams.SceneName].Scene;
        }
        else
        {
            var progress = sceneLoadParams.TrackProgress ? GetNewLoadingProgress(loadingSceneData) : new Progress<float>();
            var asyncOperationHandle = Addressables.LoadSceneAsync(sceneLoadParams.SceneName, sceneLoadParams.LoadParameters.loadSceneMode);

            await asyncOperationHandle.ToUniTask(progress);

            loadingSceneData.LoadedScene = asyncOperationHandle.Result.Scene;
            OnLoadingSceneComplete_handler(sceneLoadParams.SceneName, loadingSceneData.LoadedScene, sceneLoadParams.SetSceneActive);
        }

        return loadingSceneData;
    }

    async UniTask<SceneLoadResult> LoadBuildScene(SceneLoadParams sceneLoadParams)
    {
        var loadingSceneData = new SceneLoadResult();

        if (SceneManager.GetSceneByName(sceneLoadParams.SceneName).isLoaded)
        {
            loadingSceneData.LoadedScene = SceneManager.GetSceneByName(sceneLoadParams.SceneName);
        }
        else
        {
            var progress = sceneLoadParams.TrackProgress ? GetNewLoadingProgress(loadingSceneData) : new Progress<float>();
            var asyncOperation = SceneManager.LoadSceneAsync(sceneLoadParams.SceneName, sceneLoadParams.LoadParameters);

            await asyncOperation.ToUniTask(progress);

            loadingSceneData.LoadedScene = SceneManager.GetSceneByName(sceneLoadParams.SceneName);
            OnLoadingSceneComplete_handler(sceneLoadParams.SceneName, loadingSceneData.LoadedScene, sceneLoadParams.SetSceneActive);
        }

        return loadingSceneData;
    }

    IProgress<float> GetNewLoadingProgress(SceneLoadResult sceneLoadResult)
    {
        _serviceLoadingProgress.RegisterLoadingProgress(sceneLoadResult.SceneLoadProgress);

        var progress = Progress.CreateOnlyValueChanged<float>(x =>
        {
            sceneLoadResult.SceneLoadProgress.SceneProgress = x;
            _serviceLoadingProgress.UpdateProgress();
        });
        return progress;
    }
    
    UniTask UnloadBySceneManager(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded)
        {
            Debug.LogError($"There's no active scene with name '{sceneName}'.");
            return new UniTask();
        }

        return SceneManager.UnloadSceneAsync(scene).ToUniTask();
    }

    UniTask UnloadByAddressable(string sceneName)
    {
        if (!_loadedAddressableScene.ContainsKey(sceneName))
        {
            Debug.Log($"[ServiceSceneLoader]: UnloadAddressableScene - There's no scene {sceneName} in dictionary");
            return UniTask.NextFrame();
        }

        var asyncOperationHandle = Addressables.UnloadSceneAsync(_loadedAddressableScene[sceneName]);
        asyncOperationHandle.Completed += _ => _loadedAddressableScene.Remove(sceneName);
        return asyncOperationHandle.ToUniTask();
    }
    
    void OnLoadingSceneComplete_handler<T>(string sceneName, T result, bool setActiveScene)
    {
        Scene scene = default;
        switch (result)
        {
            case SceneInstance instance:
                _loadedAddressableScene.Add(sceneName, instance);
                scene = instance.Scene;
                break;
            case Scene sceneResult:
                scene = sceneResult;
                break;
        }

        if (setActiveScene)
            SceneManager.SetActiveScene(scene);
    }
    
#endregion
}
}