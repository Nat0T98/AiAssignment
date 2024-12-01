using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reference to the state of a node in the behaviour tree
/// </summary>
public enum NodeState
{
    SUCCESS, //Evaluation was a success
    FAILURE, //Evaluation was a failure
    RUNNING //Evaluation of node is ongoing
}


/// <summary>
/// Base class for all nodes in the behavior tree.
/// </summary>
/*[System.Serializable]*/
public abstract class Node
{
    protected NodeState nodeState;
    protected NodeState NodeState { get { return nodeState; } }

    public abstract NodeState Evaluate();
}



/// <summary>
/// A composite node that succeeds if at least one child node succeeds 
/// (OR logic).
/// </summary>
public class Selector : Node
{
    protected List<Node> SelectorNodes = new List<Node>();

    public Selector(List<Node> _nodes)
    {
        SelectorNodes = _nodes;
    }


    public override NodeState Evaluate()
    {
        foreach (var node in SelectorNodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.SUCCESS:
                    nodeState = NodeState.SUCCESS;
                    return nodeState;
                case NodeState.FAILURE: //No success yet, keep checking other nodes in sequence
                    break;
                case NodeState.RUNNING:
                    nodeState = NodeState.RUNNING;
                    return nodeState;
                default:
                    break;
            }
        }

        //All nodes in sequence failed
        nodeState = NodeState.FAILURE;
        return nodeState;
    }
}


/// <summary>
/// A composite node that evaluates child nodes in sequence.
/// Only succeeds if all child nodes succeed 
/// (AND logic).
/// </summary>
public class Sequence : Node
{
    protected List<Node> sequenceNodes = new List<Node>();

    public Sequence(List<Node> nodes)
    {
        sequenceNodes = nodes;
    }


    public override NodeState Evaluate()
    {
        bool anyNodeRunning = false;

        foreach (var node in sequenceNodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE:
                    nodeState = NodeState.FAILURE;
                    return nodeState;
                case NodeState.RUNNING:
                    anyNodeRunning = true;
                    break;
            }
        }

        nodeState = anyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return nodeState;
    }

}

/// <summary>
/// A decorator node that inverts the result of its child node.
/// </summary>
public class Opposite : Node
{
    protected Node node;

    public Opposite(Node _node)
    {
        node = _node;
    }

    public override NodeState Evaluate()
    {
        NodeState childState = node.Evaluate();
        nodeState = childState == NodeState.SUCCESS ? NodeState.FAILURE
                   : childState == NodeState.FAILURE ? NodeState.SUCCESS
                   : NodeState.RUNNING;
        return nodeState;
    }
}

