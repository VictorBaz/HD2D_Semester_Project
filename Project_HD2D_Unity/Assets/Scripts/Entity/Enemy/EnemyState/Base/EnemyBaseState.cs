public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyContext actx);
    public abstract void UpdateState(EnemyContext actx);
    public abstract void ExitState(EnemyContext actx);

    public virtual string Name { get; private set; }

    public virtual bool CanAttack    => false;
    public virtual bool CanMove      => true;
    public virtual bool CanTakeDamage => true;
    public virtual bool IsFalling    => false;
    public virtual bool CanBeParry   { get; protected set; } = false;
}