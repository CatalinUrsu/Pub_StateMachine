using System.Linq;
using UnityEngine.SceneManagement;

namespace StateMachine
{
public static class StateMachineExtension
{
    /// <summary>
    /// Find and return <b>ISceneEntry</b> component in FirstRootObject's children. RootObject's name must be "[Setup]". 
    /// </summary>
    public static IEntryPoint FindEntryPoint(this Scene scene) =>
        (from rootObject in scene.GetRootGameObjects() where rootObject.name.Equals("[Setup]")
         select rootObject.GetComponentInChildren<IEntryPoint>()).FirstOrDefault();
}
}