using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
	public virtual void StateStart() { }
    public virtual void StateEnd() { }
    public virtual void Update(float deltaTime) { }
}
