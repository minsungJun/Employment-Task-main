namespace Project.Scripts.Data_Script
{
    [System.Serializable]
    public class WallData
    {
        public int x;
        public int y;
        public ObjectPropertiesEnum.WallDirection WallDirection;
        public int length;
        public ColorType wallColor;
        public WallGimmickType wallGimmickType;


        public WallData(int x, int y, ObjectPropertiesEnum.WallDirection WallDirection, int length, ColorType wallColor, WallGimmickType wallGimmickType)
        {
            this.x = x;
            this.y = y;
            this.WallDirection = WallDirection;
            this.length = length;
            this.wallColor = wallColor;
            this.wallGimmickType = wallGimmickType;
        }
    }

    
}

public enum WallGimmickType
{
    None = 0,
    Star
}