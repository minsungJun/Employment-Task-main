using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public partial class BoardController : MonoBehaviour
{
    public static BoardController Instance;

    [SerializeField] private Model _model;
    [SerializeField] private View _view;

    public Model model => _model;
    public View view => _view;


    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Init(model.nowStageIndex);
    }

    private async void Init(int stageIdx)
    {
        if (model.stageDatas == null)
        {
            Debug.LogError("StageData가 할당되지 않았습니다!");
            return;
        }

        

        view.boardParent = new GameObject("BoardParent");
        view.boardParent.transform.SetParent(transform);

        
        await CreateCustomWalls(stageIdx);
        
        await CreateBoardAsync(stageIdx);

        await CreatePlayingBlocksAsync(stageIdx);

        CreateMaskingTemp();
    }
    private async Task CreateBoardAsync(int stageIdx = 0)
    {


        model.nowStageIndex = stageIdx;

        //setdata 추가
        int standardBlockIndex = -1;
        
        // 보드 블록 생성
        foreach(var data in model.stageDatas[stageIdx].boardBlocks)
        {
            GameObject blockObj = Instantiate(view.boardBlockPrefab, view.boardParent.transform);//보드블럭 프리팹 생성
            blockObj.transform.localPosition = new Vector3( //프리팹 위치 초기화
                data.x * model.blockDistance,
                0,
                data.y * model.blockDistance
            );

            if(!blockObj.TryGetComponent(out BoardBlockObject boardBlock)) // 컴포넌트가 없으면 다음 루프로
            {
                Debug.LogWarning("boardBlockPrefab에 BoardBlockObject 컴포넌트가 필요합니다!");
                continue;
            }
            boardBlock._ctrl = this;
            boardBlock.x = data.x;
            boardBlock.y = data.y;

            if(model.wallCoorInfoDic.TryGetValue((boardBlock.x, boardBlock.y), out var wallInfo)) //TryGetValue로 딕셔너리 조회를 줄임
            {
                foreach(var info in wallInfo)
                {
                    //Dictionary<(int x, int y), Dictionary<(DestroyWallDirection, ColorType), int (화살표 방향, 색깔, 개수)
                    DestroyWallDirection dir = info.Key.Item1;
                    ColorType color = info.Key.Item2;
                    int len = info.Value;

                    bool horizon = dir == DestroyWallDirection.Up || dir == DestroyWallDirection.Down;

                    boardBlock.colorType.Add(color);
                    boardBlock.len.Add(len);
                    boardBlock.isHorizon.Add(horizon);

                    model.standardBlockDic.Add((++standardBlockIndex, horizon), boardBlock);
                    
                }
                boardBlock.isCheckBlock = true; //블럭 정보 입력 완료 후 true 리턴
            }
            else
            {
                boardBlock.isCheckBlock = false; // 없을시 false 리턴
            }

            model.boardBlockDic.Add((data.x, data.y), boardBlock);

        }

        // standardBlockDic에서 관련 위치의 블록들 설정
        foreach (var kv in model.standardBlockDic)
        {
            BoardBlockObject boardBlockObject = kv.Value; //딕셔너리 요소 명시화
            bool horizon = kv.Key.Item2;

            for (int i = 0; i < boardBlockObject.colorType.Count; i++)
            {
                ColorType color = boardBlockObject.colorType[i];
                int len = boardBlockObject.len[i];

                if (horizon) // 가로 방향
                {
                    for (int j = boardBlockObject.x + 1; j < boardBlockObject.x + boardBlockObject.len[i]; j++)
                    {
                        if (model.boardBlockDic.TryGetValue((j, boardBlockObject.y), out BoardBlockObject targetBlock))
                        {
                            targetBlock.colorType.Add(color);
                            targetBlock.len.Add(len);
                            targetBlock.isHorizon.Add(horizon);
                            targetBlock.isCheckBlock = true;
                        }
                    }
                }
                else // 세로 방향
                {
                    for (int k = boardBlockObject.y + 1; k < boardBlockObject.y + boardBlockObject.len[i]; k++)
                    {
                        if (model.boardBlockDic.TryGetValue((boardBlockObject.x, k), out BoardBlockObject targetBlock))
                        {
                            targetBlock.colorType.Add(color);
                            targetBlock.len.Add(len);
                            targetBlock.isHorizon.Add(horizon);
                            targetBlock.isCheckBlock = true;
                        }
                    }
                }
            }
        }

        // 3체크 블록 그룹 생성
        int checkBlockIndex = -1;
        model.CheckBlockGroupDic = new Dictionary<int, List<BoardBlockObject>>();

        foreach (var blockPos in model.boardBlockDic.Keys)
        {
            BoardBlockObject boardBlock = model.boardBlockDic[blockPos];
            
            for (int j = 0; j < boardBlock.colorType.Count; j++)
            {
                if (!boardBlock.isCheckBlock && boardBlock.colorType[j] == ColorType.None)
                {
                    continue;
                }
                    // 이 블록이 이미 그룹에 속해있는지 확인
                if (boardBlock.checkGroupIdx.Count <= j)
                {
                    int? neighborGroup = GetNearGroup(boardBlock, j);
                    if (neighborGroup.HasValue)
                    {
                        int idx = neighborGroup.Value;
                        model.CheckBlockGroupDic[idx].Add(boardBlock);
                        boardBlock.checkGroupIdx.Add(idx);
                    }
                    else
                    {
                        checkBlockIndex++;
                        model.CheckBlockGroupDic[checkBlockIndex] = new List<BoardBlockObject> { boardBlock };
                        boardBlock.checkGroupIdx.Add(checkBlockIndex);
                    }
                }
                        
            }
        }

        await Task.Yield();
        
        model.boardWidth = model.boardBlockDic.Keys.Max(k => k.x);
        model.boardHeight = model.boardBlockDic.Keys.Max(k => k.y);
    }

    int? GetNearGroup(BoardBlockObject boardBlock, int index) //왼쪽 위쪽 판별 함수
    {
        (int x, int y) nearPos = boardBlock.isHorizon[index]
            ? (boardBlock.x - 1, boardBlock.y)  // 왼쪽
            : (boardBlock.x, boardBlock.y - 1); // 위쪽

        if (model.boardBlockDic.TryGetValue(nearPos, out var nearBlock))
        {
            if (index < nearBlock.colorType.Count &&
                nearBlock.colorType[index] == nearBlock.colorType[index] &&
                nearBlock.checkGroupIdx.Count > index)
            {
                return nearBlock.checkGroupIdx[index];
            }
        }
        return null;
    }

     private async Task CreatePlayingBlocksAsync(int stageIdx = 0) //조종 가능한 블럭 생성
     {
         view.playingBlockParent = new GameObject("PlayingBlockParent");
         
         for (int i = 0; i < model.stageDatas[stageIdx].playingBlocks.Count; i++)
         {
             var pbData = model.stageDatas[stageIdx].playingBlocks[i];

             GameObject blockGroupObject = Instantiate(view.blockGroupPrefab, view.playingBlockParent.transform);
             blockGroupObject.transform.position = new Vector3(
                 pbData.center.x * model.blockDistance, 
                 0.33f, 
                 pbData.center.y * model.blockDistance
             );

             BlockDragHandler dragHandler = blockGroupObject.GetComponent<BlockDragHandler>(); //playingblock 그룹의 핸들러 컴포넌트 
             if (dragHandler != null) dragHandler.model.blocks = new List<BlockObject>();

             dragHandler.model.uniqueIndex = pbData.uniqueIndex;
             foreach (var gimmick in pbData.gimmicks)
             {
                 if (Enum.TryParse(gimmick.gimmickType, out ObjectPropertiesEnum.BlockGimmickType gimmickType))
                 {
                     dragHandler.model.gimmickType.Add(gimmickType);
                 }
             }
             
             int maxX = 0;
             int minX = model.boardWidth;
             int maxY = 0;
             int minY = model.boardHeight;
             foreach (var shape in pbData.shapes)
             {
                 GameObject singleBlock = Instantiate(view.blockPrefab, blockGroupObject.transform);
                 
                 singleBlock.transform.localPosition = new Vector3(
                     shape.offset.x * model.blockDistance,
                     0f,
                     shape.offset.y * model.blockDistance
                 );
                 dragHandler.model.blockOffsets.Add(new Vector2(shape.offset.x, shape.offset.y));


                 var renderer = singleBlock.GetComponentInChildren<SkinnedMeshRenderer>();
                 if (renderer != null && pbData.colorType >= 0)
                 {
                     renderer.material = view.testBlockMaterials[(int)pbData.colorType];
                 }

                 if (singleBlock.TryGetComponent(out BlockObject blockObj))
                 {
                     blockObj.colorType = pbData.colorType;
                     blockObj.x = pbData.center.x + shape.offset.x;
                     blockObj.y = pbData.center.y + shape.offset.y;
                     blockObj.offsetToCenter = new Vector2(shape.offset.x, shape.offset.y);
                     
                     if (dragHandler != null)
                         dragHandler.model.blocks.Add(blockObj);
                     model.boardBlockDic[((int)blockObj.x, (int)blockObj.y)].playingBlock = blockObj;
                     blockObj.preBoardBlockObject = model.boardBlockDic[((int)blockObj.x, (int)blockObj.y)];
                     if(minX > blockObj.x) minX = (int)blockObj.x;
                     if(minY > blockObj.y) minY = (int)blockObj.y;
                     if(maxX < blockObj.x) maxX = (int)blockObj.x;
                     if(maxY < blockObj.y) maxY = (int)blockObj.y;
                 }
             }

             dragHandler.model.horizon = maxX - minX + 1;
             dragHandler.model.vertical = maxY - minY + 1;
         }

         await Task.Yield();
         
     }

    public void GoToPreviousLevel()
    {
        if (model.nowStageIndex == 0) return;

        Destroy(view.boardParent);
        Destroy(view.playingBlockParent.gameObject);
        Init(--model.nowStageIndex);
        
        StartCoroutine(Wait());
    }

    public void GotoNextLevel()
    {
        if (model.nowStageIndex == model.stageDatas.Length - 1) return;
        
        Destroy(view.boardParent);
        Destroy(view.playingBlockParent.gameObject);
        Init(++model.nowStageIndex);
        
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return null;
        
        Vector3 camTr = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(1.5f + 0.5f * (model.boardWidth - 4),camTr.y,camTr.z);
    } 
    private async Task CreateCustomWalls(int stageIdx)
    {
        if (stageIdx < 0 || stageIdx >= model.stageDatas.Length || model.stageDatas[stageIdx].Walls == null)
        {
            Debug.LogError($"유효하지 않은 스테이지 인덱스이거나 벽 데이터가 없습니다: {stageIdx}");
            return;
        }

        GameObject wallsParent = new GameObject("CustomWallsParent");
        
        wallsParent.transform.SetParent(view.boardParent.transform);
        model.wallCoorInfoDic = new Dictionary<(int x, int y), Dictionary<(DestroyWallDirection, ColorType), int>>();
        
        foreach (var wallData in model.stageDatas[stageIdx].Walls)
        {
            Quaternion rotation;

            // 기본 위치 계산
            var position = new Vector3(
                wallData.x * model.blockDistance, 
                0f, 
                wallData.y * model.blockDistance);
            
            DestroyWallDirection destroyDirection = DestroyWallDirection.None;
            bool shouldAddWallInfo = false;

            // 벽 방향과 유형에 따라 위치와 회전 조정
            switch (wallData.WallDirection)
            {
                case ObjectPropertiesEnum.WallDirection.Single_Up:
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    shouldAddWallInfo = true;
                    destroyDirection = DestroyWallDirection.Up;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Single_Down:
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    shouldAddWallInfo = true;
                    destroyDirection = DestroyWallDirection.Down;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Single_Left:
                    position.x -= 0.5f;
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    shouldAddWallInfo = true;
                    destroyDirection = DestroyWallDirection.Left;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Single_Right:
                    position.x += 0.5f;
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    shouldAddWallInfo = true;
                    destroyDirection = DestroyWallDirection.Right;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Left_Up:
                    // 왼쪽 위 모서리
                    position.x -= 0.5f;
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Left_Down:
                    // 왼쪽 아래 모서리
                    position.x -= 0.5f;
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Right_Up:
                    // 오른쪽 위 모서리
                    position.x += 0.5f;
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 270f, 0f);
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Right_Down:
                    // 오른쪽 아래 모서리
                    position.x += 0.5f;
                    position.z -= 0.5f;
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Open_Up:
                    // 위쪽이 열린 벽
                    position.z += 0.5f;
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Open_Down:
                    // 아래쪽이 열린 벽
                    position.z -= 0.5f;
                    rotation = Quaternion.identity;
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Open_Left:
                    // 왼쪽이 열린 벽
                    position.x -= 0.5f;
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                    
                case ObjectPropertiesEnum.WallDirection.Open_Right:
                    // 오른쪽이 열린 벽
                    position.x += 0.5f;
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                    
                default:
                    Debug.LogError($"지원되지 않는 벽 방향: {wallData.WallDirection}");
                    continue;
            }
            
            if (shouldAddWallInfo && wallData.wallColor != ColorType.None)
            {
                var pos = (wallData.x, wallData.y);
                var wallInfo = (destroyDirection, wallData.wallColor);
    
                if (!model.wallCoorInfoDic.ContainsKey(pos))
                {
                    Dictionary<(DestroyWallDirection, ColorType), int> wallInfoDic = 
                        new Dictionary<(DestroyWallDirection, ColorType), int> { { wallInfo, wallData.length } };
                    model.wallCoorInfoDic.Add(pos, wallInfoDic);
                }
                else
                {
                    model.wallCoorInfoDic[pos].Add(wallInfo, wallData.length);
                }
            }

            // 길이에 따른 위치 조정 (수평/수직 벽만 조정)
            if (wallData.length > 1)
            {
                // 수평 벽의 중앙 위치 조정 (Up, Down 방향)
                if (wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Up || 
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Down ||
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Up || 
                    wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Down)
                {
                    // x축으로 중앙으로 이동
                    position.x += (wallData.length - 1) * model.blockDistance * 0.5f;
                }
                // 수직 벽의 중앙 위치 조정 (Left, Right 방향)
                else if (wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Left || 
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Single_Right ||
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Left || 
                         wallData.WallDirection == ObjectPropertiesEnum.WallDirection.Open_Right)
                {
                    // z축으로 중앙으로 이동
                    position.z += (wallData.length - 1) * model.blockDistance * 0.5f;
                }
            }

            // 벽 오브젝트 생성, isOriginal = false
            // prefabIndex는 length-1 (벽 프리팹 배열의 인덱스)
            if (wallData.length - 1 >= 0 && wallData.length - 1 < view.wallPrefabs.Length)
            {
                GameObject wallObj = Instantiate(view.wallPrefabs[wallData.length - 1], wallsParent.transform);
                wallObj.transform.position = position;
                wallObj.transform.rotation = rotation;
                WallObject wall = wallObj.GetComponent<WallObject>();
                wall.SetWall(view.wallMaterials[(int)wallData.wallColor], wallData.wallColor != ColorType.None);
                view.walls.Add(wallObj);
            }
            else
            {
                Debug.LogError($"프리팹 인덱스 범위를 벗어남: {wallData.length - 1}, 사용 가능한 프리팹: 0-{view.wallPrefabs.Length - 1}");
            }
        }
        
        await Task.Yield();
    }

    private void CreateMaskingTemp()
    {
        foreach (var quad in view.quads)
        {
            Destroy(quad);
        }
        view.quads.Clear();
        
        for (int i = -3; i <= model.boardWidth + 3; i++)
        {
            for (int j = -3; j <= model.boardHeight + 3; j++)
            {
                if (model.boardBlockDic.ContainsKey((i, j))) continue;

                float xValue = i;
                float zValue = j;
                if (i == -1 && j <= model.boardHeight) xValue -= model.wallOffset;
                if (i == model.boardWidth + 1 && j <= model.boardHeight + 1) xValue += model.wallOffset;
                
                if (j == -1 && i <= model.boardWidth) zValue -= model.wallOffset;
                if (j == model.boardHeight + 1 && i <= model.boardWidth + 1) zValue += model.wallOffset;
                
                GameObject quad = GameObject.Instantiate(view.quadPrefab, view.quadTr);
                view.quads.Add(quad);
                
                quad.transform.position = model.blockDistance * new Vector3(xValue, model.yoffset, zValue);
            }
        }
    }
 
}