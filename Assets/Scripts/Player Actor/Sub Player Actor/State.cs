using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    protected PlayerActor _pA;
    protected bool isActive = false;

    public virtual void SetValues(PlayerActor playerActor) { }
    public virtual void Activate() { }
    public virtual void Deactivate() { }
}
