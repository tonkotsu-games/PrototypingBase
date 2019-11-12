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
                        currentNodeState = NodeStates.FAILURE;
                        return currentNodeState;
                    }
                case NodeStates.SUCCESS:
                    {
                        continue;
                    }
                case NodeStates.RUNNING:
                    {
                        continue;
                    }
                default:
                    {
                        currentNodeState = NodeStates.SUCCESS;
                        return currentNodeState;
                    }
            }
        }
    }
}