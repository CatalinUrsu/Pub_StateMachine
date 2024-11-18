using UnityEngine;

public interface IServiceCamera
{
    void RegisterMainCamera(Camera camera);
    void RegisterCamera(Camera camera, bool addToStack = true);
    void UnregisterCamera(Camera camera);
    Camera GetMainCamera();
    Camera GetCameraByName(string name);
    Camera GetCameraByTag(string tag);
}