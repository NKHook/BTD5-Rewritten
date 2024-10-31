using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Compound;

public partial class ActorNode : Node2D
{
    public int SpriteUid;
    public Node2D Node;

    public ActorNode(int uid, Node2D node)
    {
        SpriteUid = uid;
        Node = node;
        
        AddChild(Node);
    }
}