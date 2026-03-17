public abstract class CameraBaseState
{
    public abstract void EnterState(CameraStateContext context);
    public abstract void UpdateState(CameraStateContext context);
    public abstract void ExitState(CameraStateContext context);
}