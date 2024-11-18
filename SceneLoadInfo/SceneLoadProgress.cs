namespace StateMachine
{
public class SceneLoadProgress
{
    public float SceneProgress;
    public float EntryPointProgress;

    public float GetAvgProgress() => (SceneProgress + EntryPointProgress) / 2;
}
}