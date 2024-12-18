using System.Collections.Generic;

/// <summary>
/// A composite node that evaluates child nodes in sequence.
/// Only succeeds if all child nodes succeed 
/// (AND logic).
/// </summary>
public class Sequence : Node
{
    protected List<Node> nodes = new List<Node>();

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            switch (nodes[i].Evaluate())
            {
                case NodeState.RUNNING:
                    nodeState = NodeState.RUNNING;
                    return nodeState;
                case NodeState.FAILURE:
                    nodeState = NodeState.FAILURE;
                    return nodeState;
            }
        }
        nodeState = NodeState.SUCCESS;
        return nodeState;
    }
}
