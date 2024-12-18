using System.Collections.Generic;


/// <summary>
/// A composite node that succeeds if at least one child node succeeds 
/// (OR logic).
/// </summary>
public class Selector : Node
{
    protected List<Node> nodes = new List<Node>();

    public Selector(List<Node> nodes)
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
                case NodeState.SUCCESS:
                    nodeState = NodeState.SUCCESS;
                    return nodeState;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        nodeState = NodeState.FAILURE;
        return nodeState;
    }
}