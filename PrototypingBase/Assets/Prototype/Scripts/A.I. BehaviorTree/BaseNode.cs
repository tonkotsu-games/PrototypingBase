using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseNode
{
    protected NodeStates currentNodeState;

    public NodeStates nodeState
    {
        get { return currentNodeState; }
    }

    public BaseNode() { }

    public abstract NodeStates Evaluate();
}