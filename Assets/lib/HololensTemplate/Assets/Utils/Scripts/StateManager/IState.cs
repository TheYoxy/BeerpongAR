using UnityEngine;

public abstract class IState
{
    public abstract bool IsFinished();
    public abstract object[] GetParams();
    public virtual void OnStart(params object[] args)
    {}

    public virtual void OnUpdate()
    {}

    public virtual void OnFixedUpdate()
    {}

    public virtual void OnCancel()
    {}

    public virtual void OnStop()
    {}
}