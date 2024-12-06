using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A decorator node that inverts the result of its child node.
/// </summary>
public class Inverter : Node
{
    private Node node;

    public Inverter(Node node)
    {
        this.node = node;
    }


    public override NodeState Evaluate()
    {
        switch (node.Evaluate())
        {
            case NodeState.RUNNING:
                nodeState = NodeState.RUNNING;
                break;
            case NodeState.SUCCESS:
                nodeState = NodeState.FAILURE;
                break;
            case NodeState.FAILURE:
                nodeState = NodeState.SUCCESS;
                break;
            default:
                break;
        }
        return nodeState;
    }
}