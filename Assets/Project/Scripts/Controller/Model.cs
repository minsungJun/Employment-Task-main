using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{

    [SerializeField] private StageData[] StageDatas;
    public StageData[] stageDatas => StageDatas;

    [SerializeField] private Dictionary<int, List<BoardBlockObject>> checkBlockGroupDic = new Dictionary<int, List<BoardBlockObject>>(); 
    public Dictionary<int, List<BoardBlockObject>> CheckBlockGroupDic { get => checkBlockGroupDic; set => checkBlockGroupDic = value; }
    [SerializeField] private Dictionary<(int x, int y), BoardBlockObject> BoardBlockDic = new Dictionary<(int x, int y), BoardBlockObject>();
    public Dictionary<(int x, int y), BoardBlockObject> boardBlockDic { get => BoardBlockDic; set => BoardBlockDic = value; }
    [SerializeField] private Dictionary<(int, bool), BoardBlockObject> StandardBlockDic = new Dictionary<(int, bool), BoardBlockObject>();
    public Dictionary<(int, bool), BoardBlockObject> standardBlockDic { get => StandardBlockDic; set => StandardBlockDic = value; }
    [SerializeField] private Dictionary<(int x, int y), Dictionary<(DestroyWallDirection, ColorType), int>> WallCoorInfoDic;
    public Dictionary<(int x, int y), Dictionary<(DestroyWallDirection, ColorType), int>> wallCoorInfoDic { get => WallCoorInfoDic; set => WallCoorInfoDic = value; }


    public int boardWidth;
    public int boardHeight;
    public readonly float blockDistance = 0.79f;
    public int nowStageIndex = 0;



    private float Yoffset = 0.625f;
    public float yoffset => Yoffset;
    private float WallOffset = 0.225f;
    public float wallOffset => WallOffset;

}
