
using System.Collections.Generic;

[System.Serializable]
public class BoardBlockData : BlockBaseData
{
    public List<ColorType> colorType;
    public List<int> dataType;

    public BoardBlockData(int x, int y)
    {
        this.x = x;
        this.y = y;
        colorType = new List<ColorType>();
        dataType = new List<int>();
    }
}

public enum DestroyWallDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4
}

