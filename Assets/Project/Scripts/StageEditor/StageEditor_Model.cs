using UnityEngine;

public class StageEditor_Model : MonoBehaviour
{
    [SerializeField] private StageData CustomData;
    public StageData customdata {get => CustomData; set => CustomData = value;}
    private int Width = 5;
    public int width {get => Width; set => Width = value;}
    private int Height = 5;
    public int height {get => Height; set => Height = value;}
    private int WallX = 0;
    public int wallx {get => WallX; set => WallX = value;}
    private int WallY = 0;
    public int wally {get => WallY; set => WallY = value;}
    private ObjectPropertiesEnum.WallDirection Dir;
    public ObjectPropertiesEnum.WallDirection dir {get => Dir; set => Dir = value;}
    private ColorType WallColor;
    public ColorType wallcolor {get => WallColor; set => WallColor = value;}
    private int Length = 0;
    public int len {get => Length; set => Length = value;}
    private WallGimmickType Gimmick;
    public WallGimmickType gimmick {get => Gimmick; set => Gimmick = value;}
    private int PBX = 0;
    public int pbx {get => PBX; set => PBX = value;}
    private int PBY = 0;
    public int pby {get => PBY; set => PBY = value;}
    private ColorType PBColor;
    public ColorType pbcolor {get => PBColor; set => PBColor = value;}
    private GimmickData PBGimmick;
    public GimmickData pbgimmick {get => PBGimmick; set => PBGimmick = value;}
    private int PBUniqueIndex = 1;
    public int pbuniqueIndex {get => PBUniqueIndex; set => PBUniqueIndex = value;}
    private int ChooseUniqueIndex;
    public int chooseuniqueindex {get => ChooseUniqueIndex; set => ChooseUniqueIndex = value;}
}
