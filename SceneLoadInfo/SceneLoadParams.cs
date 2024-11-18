using System;
using UnityEngine.SceneManagement;

namespace StateMachine
{
public struct SceneLoadParams
{
    public string SceneName { get; private set; }
    public string Prompt { get; private set; }
    public bool IsAddressable { get; private set; }
    public bool TrackProgress { get; private set; }
    public bool SetSceneActive { get; private set; }
    public LoadSceneParameters LoadParameters { get; private set; }

    public class Builder
    {
        string _sceneName = String.Empty;
        string _prompt = String.Empty;
        bool _isAddressable = true;
        bool _trackProgress = true;
        bool _setSceneActive;
        LoadSceneParameters _loadParameters = new(LoadSceneMode.Additive);

        public Builder(string sceneName)
        {
            _sceneName = sceneName;
        }

        public Builder SetPrompt(string prompt = "")
        {
            _prompt = prompt;
            return this;
        }

        public Builder SetIsAddressable(bool isAddressable)
        {
            _isAddressable = isAddressable;
            return this;
        }

        public Builder SetActiveOnLoad(bool setActive)
        {
            _setSceneActive = setActive;
            return this;
        }

        public Builder SetTrackProgress(bool trackProgress)
        {
            _trackProgress = trackProgress;
            return this;
        }

        public Builder SetLoadParameters(LoadSceneParameters loadParameters)
        {
            _loadParameters = loadParameters;
            return this;
        }

        public SceneLoadParams Build()
        {
            return new SceneLoadParams()
            {
                SceneName = _sceneName,
                Prompt = _prompt,
                IsAddressable = _isAddressable,
                TrackProgress = _trackProgress,
                SetSceneActive = _setSceneActive,
                LoadParameters = _loadParameters
            };
        }
    }
}
}