using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


/// <summary>
/// Reference to the state of a node in the behaviour tree
/// </summary>
public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE,
}

/// <summary>
/// Base class for all nodes in the behavior tree.
/// </summary>
public abstract class Node
{
    protected NodeState nodeState;
    public NodeState GetNodeState() 
    { return nodeState; }

    public abstract NodeState Evaluate();
}

