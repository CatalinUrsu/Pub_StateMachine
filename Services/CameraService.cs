using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class CameraService : IServiceCamera
{
    Camera _mainCamera; 
    UniversalAdditionalCameraData _cameraData;
    readonly Dictionary<string, Camera> _cachedCameras = new();

    public void RegisterMainCamera(Camera camera)
    {
        if (_mainCamera != null) return;
        
        _mainCamera = camera;
        _cameraData = camera.GetUniversalAdditionalCameraData();
    }

    public void RegisterCamera(Camera camera, bool addToStack = true)
    {
        if (_cachedCameras.ContainsKey(camera.name))
            Debug.LogWarning($"[CamerasService] Camera with name '{camera.name}' will be replaced.");

        _cachedCameras.Add(camera.name, camera);

        if (addToStack && !_cameraData.cameraStack.Contains(camera))
            _cameraData.cameraStack.Add(camera);
    }

    public void UnregisterCamera(Camera camera)
    {
        _cameraData.cameraStack.Remove(camera);
            
        if (_cachedCameras.ContainsKey(camera.name))
            _cachedCameras.Remove(camera.name);
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public Camera GetCameraByName(string name)
    {
        if (_cachedCameras.ContainsKey(name))
            return _cachedCameras[name];

        var cam = _cachedCameras.FirstOrDefault(x => x.Key.Contains(name)).Value;
        if (cam != null)
            return cam;

        Debug.LogError($"[CamerasService] Not found camera with name '{name}'!");
        return null;
    }

    public Camera GetCameraByTag(string tag)
    {
        var cam = _cachedCameras.FirstOrDefault(x => x.Value.CompareTag(tag)).Value;
        if (cam != null)
            return cam;

        Debug.LogError($"[CamerasService] Not found camera with tag '{tag}'!");
        return null;
    }
}