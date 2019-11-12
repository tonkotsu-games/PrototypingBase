using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : BaseNode
{
    private List<BaseNode> nodes = new List<BaseNode>();

    public Selected(List<BaseNode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeStates Evaluate()
    {
        foreach (BaseNode node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    {
                        continue;
                    }
                case NodeStates.SUCCESS:
                    {
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;
                    }
                default:
                    {
                        continue;
                    }
            }
        }
        currentNodeState = NodeStates.FAILURE;
        return currentNodeState;
    }
}