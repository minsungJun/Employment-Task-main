
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayingBlockData : BlockBaseData
{
    public Vector2Int center;
    public int uniqueIndex;
    public ColorType colorType;
    public List<ShapeData> shapes;
    public List<GimmickData> gimmicks;

    public PlayingBlockData(int x, int y, int index, ColorType colorType)
    {
        this.center = new Vector2Int(x, y);
        this.uniqueIndex = index;
        this.colorType = colorType;
        this.shapes = new List<ShapeData>();
        this.shapes.Add(new ShapeData(new Vector2Int(0, 0)));
        gimmicks = new List<GimmickData>();
    }
}

[System.Serializable]
public class ShapeData
{
    public Vector2Int offset;
    public ShapeData(Vector2Int offset)
    {
        this.offset = offset;
    }
}

// public enum BlockGimmickType
// {
//     None = 0,
//     Constraint,
//     Multiple,
//     Frozen,
//     Star,
//     Key,
//     Lock
// }

