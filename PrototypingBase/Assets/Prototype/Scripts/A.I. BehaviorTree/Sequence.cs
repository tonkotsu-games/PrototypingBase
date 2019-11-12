using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : BaseNode
{
    private List<BaseNode> nodes = new List<BaseNode>();

    public Sequence(List<BaseNode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeStates Evaluate()
    {
        bool childrunning = false;

        foreach (BaseNode node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    {
                        currentNodeState = NodeStates.FAILURE;
                        return currentNodeState;
                    }
                case NodeStates.SUCCESS:
                    {
                        continue;
                    }
                case NodeStates.RUNNING:
                    {
                        childrunning = true;
                        continue;
                    }
                default:
                    {
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;
                    }
            }
        }
        currentNodeState = childrunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return currentNodeState;
    }
}