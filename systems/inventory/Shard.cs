using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using framework.systems.tag;
using Godot;
 
namespace framework.systems.inventory
{
    [Serializable]
    [ShardHidden]
    public class Shard
    {
        public List<Tag> Tags;
        
        public virtual string GetDebugPrintString() { return "";  }

        public void OnAddedToItem(Item item)
        {

        }

        public void OnRemovedFromItem(Item item)
        {

        }
    }

    public class ShardText : Shard
    {
        public string Text;

        public override string GetDebugPrintString()
        {
            return Text;
        }
    }

    public class ShardNumberFloat : Shard
    {
        public float Number;

        public override string GetDebugPrintString()
        {
            return Number.ToString();
        }
    }

    public class ShardNumberInt : Shard
    {
        public int number;


        public override string GetDebugPrintString()
        {
            return number.ToString();
        }
    }

    public class ShardNumberDouble : Shard
    {
        public double number;


        public override string GetDebugPrintString()
        {
            return number.ToString();
        }
    }

    public class ShardBool : Shard
    {
        public bool value;


        public override string GetDebugPrintString()
        {
            return value.ToString();
        }
    }

    public class ShardVector2 : Shard
    {
        public Vector2 vector2;


        public override string GetDebugPrintString()
        {
            return vector2.ToString();
        }
    }

    public class ShardVector3 : Shard
    {
        public Vector3 vector3;

        public override string GetDebugPrintString()
        {
            return vector3.ToString();
        }
    }

    // public class MKShard_Rect : Shard
    // {
    //     public Rect rect;
    //
    //     public override string GetDebugPrintString()
    //     {
    //         return rect.ToString();
    //     }
    // }

    public class ShardColor : Shard
    {
        public Color color;

        public override string GetDebugPrintString()
        {
            return color.ToString();
        }
    }

    public class ShardTag : Shard
    {
        public Tag AbilityTag;
    }

    public class ShardSprite2D : Shard
    {
        public Sprite2D Sprite;
    }
    
    public class ShardSprite3D : Shard
    {
        public Sprite3D Sprite;
    }

    public class ShardBox : Shard
    {
        public Vector3 Size;
        public Vector3 Offset;
        public Vector3 Rotation;
    }
}
