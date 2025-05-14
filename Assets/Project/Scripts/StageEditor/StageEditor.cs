using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.UIElements;


public class StageEditor : MonoBehaviour
{
    [SerializeField] private StageEditor_Model _Model;
    [SerializeField] private StageEditor_View _View;
    public StageEditor_Model model {get => _Model; set => _Model = value;}
    public StageEditor_View view {get => _View; set => _View = value;}


    void OnEnable()
    {
        // UI 루트 가져오기
        VisualElement root = view.uiDocument.rootVisualElement;

        // UI와 요소 연결 (name 기준)
        view.widthdrop = root.Q<DropdownField>("WidthDropDown");
        view.heightdrop = root.Q<DropdownField>("HeightDropDown");
        view.boardgenerate = root.Q<Button>("GenerateButton");

        view.Xdrop = root.Q<DropdownField>("Xdrop");
        view.Ydrop = root.Q<DropdownField>("Ydrop");
        view.Directiondrop = root.Q<DropdownField>("Directiondrop");
        view.Colordrop = root.Q<DropdownField>("Colordrop");
        view.Lengthdrop = root.Q<DropdownField>("Lengthdrop");
        view.Gimmickdrop = root.Q<DropdownField>("Gimmickdrop");
        view.Wallgenerate = root.Q<Button>("Wallgenerate");

        view.PBXdrop = root.Q<DropdownField>("PBxdrop");
        view.PBYdrop = root.Q<DropdownField>("PBydrop");
        view.PBColordrop = root.Q<DropdownField>("PBcolor");
        view.PBGimmickdrop = root.Q<DropdownField>("PBgimmick");
        view.PBgenerate = root.Q<Button>("PBgenerate");

        view.UniqueIndexdrop = root.Q<DropdownField>("UniqueIndex");
        view.up = root.Q<Button>("Up");
        view.down = root.Q<Button>("Down");
        view.left = root.Q<Button>("Left");
        view.right = root.Q<Button>("Right");

        view.RESET = root.Q<Button>("RESET");
        view.export = root.Q<Button>("Export");
        view.import = root.Q<Button>("Import");

        UpdateXChoices();//X, Y 드랍다운 값 초기화
        UpdateYChoices();

        // 이벤트 등록
        //boardsizeUI 
        view.widthdrop.RegisterValueChangedCallback(evt => //widthdrop 선택
        {
            Debug.Log("선택된 값: " + evt.newValue);
            model.width = int.Parse(evt.newValue);
            UpdateXChoices();
        });

        view.heightdrop.RegisterValueChangedCallback(evt => //heightdrop 선택
        {
            Debug.Log("선택된 값: " + evt.newValue);
            model.height = int.Parse(evt.newValue);
            UpdateYChoices();
        });

        view.boardgenerate.clicked += BoardSizeButtonClicked; //보드 생성
        //boardsizeUI end



        //wallcustomUI
        view.Xdrop.RegisterValueChangedCallback(evt => //x좌표 변경
        {
            model.wallx = int.Parse(evt.newValue);
        });
        view.Ydrop.RegisterValueChangedCallback(evt => //y좌표 변경
        {
            model.wally = int.Parse(evt.newValue);
        });
        view.Directiondrop.RegisterValueChangedCallback(evt => //벽 방향 선택
        {
            model.dir = (ObjectPropertiesEnum.WallDirection)System.Enum.Parse(typeof(ObjectPropertiesEnum.WallDirection), evt.newValue);
        });
        view.Colordrop.RegisterValueChangedCallback(evt => //벽 색깔 선택
        {
            model.wallcolor = (ColorType)System.Enum.Parse(typeof(ColorType), evt.newValue);
        });
        view.Lengthdrop.RegisterValueChangedCallback(evt => //벽 길이 선택
        {
            model.len = int.Parse(evt.newValue);
        });
        view.Gimmickdrop.RegisterValueChangedCallback(evt => //벽 기믹 선택 추후 추가
        {
            model.gimmick = (WallGimmickType)System.Enum.Parse(typeof(WallGimmickType), evt.newValue);
        });

        view.Wallgenerate.clicked += WallGenerateButtonClicked; // 벽 생성

        //playingblockui
        view.PBXdrop.RegisterValueChangedCallback(evt => //젤리 벽 x좌표 결정
        {
            model.pbx = int.Parse(evt.newValue);
        });
        view.PBYdrop.RegisterValueChangedCallback(evt => //젤리 벽 y좌표 결정 선택
        {
            model.pby = int.Parse(evt.newValue);
        });
        view.PBColordrop.RegisterValueChangedCallback(evt => //젤리 벽 색깔 선택
        {
            model.pbcolor = (ColorType)System.Enum.Parse(typeof(ColorType), evt.newValue);
        });
        view.PBGimmickdrop.RegisterValueChangedCallback(evt => //젤리 벽 기믹 선택 이벤트 추후 추가
        {
            model.pbgimmick = (GimmickData)System.Enum.Parse(typeof(GimmickData), evt.newValue);
        });

        view.PBgenerate.clicked += PBGenerateButtonClicked; //젤리 벽 생성
        //playingblockui end

        //playingblockshapeui
        view.UniqueIndexdrop.RegisterValueChangedCallback(evt => //젤리 벽 인덱스 선택
        {
            model.chooseuniqueindex = int.Parse(evt.newValue);
        });

        view.up.clicked += extendshapesup; // 젤리 벽 추가 방향 선택
        view.down.clicked += extendshapesdown;
        view.left.clicked += extendshapesleft;
        view.right.clicked += extendshapesright;

        //playingblockshapeui end

        view.RESET.clicked += reset; // 리셋
        view.export.clicked += ExportToJson; // JSON export
        view.import.clicked += importJSONtoStageData; // JSON import
    }

    private void BoardSizeButtonClicked() // 보드 생성 버튼에 연결
    {
        InputBoardData();
        ReGenerate();
    }

    private void WallGenerateButtonClicked()// 벽 생성 버튼에 연결
    {
        InputWallData();
        ReGenerate();
    }

    private void PBGenerateButtonClicked()// 젤리 벽 생성 버튼에 연결
    {
        InputPlayingBlockData();
        UpdateUniqueIndex();
        model.pbuniqueIndex++; // 젤리 벽의 인덱스 증가
        ReGenerate();
    }

    void UpdateXChoices()
    {
        if (int.TryParse(view.widthdrop.value, out int width)) // 보드 크기에 따른 드롭다운 최대값 변경
        {
            List<string> xChoices = new List<string>();
            for (int i = 0; i < model.width; i++)
            {
                xChoices.Add(i.ToString());
            }

            view.Xdrop.choices = xChoices;
            view.PBXdrop.choices = xChoices;
            view.Xdrop.index = 0; // 첫 번째 값 선택으로 초기화
            view.PBXdrop.index = 0;
        }
    }
    void UpdateYChoices()
    {
        if (int.TryParse(view.heightdrop.value, out int height))// 보드 크기에 따른 드롭다운 최대값 변경
        {
            List<string> yChoices = new List<string>();
            for (int i = 0; i < model.height; i++)
            {
                yChoices.Add(i.ToString());
            }

            view.Ydrop.choices = yChoices;
            view.PBYdrop.choices = yChoices;
            view.Ydrop.index = 0; // 첫 번째 값 선택으로 초기화
            view.PBYdrop.index = 0;
        }
    }
    void UpdateUniqueIndex()
    {
        if (int.TryParse(view.heightdrop.value, out int height))// 젤리 벽의 개수 증가에 따른 최대값 변경
        {
            List<string> Choices = new List<string>();
            for (int i = 0; i < model.pbuniqueIndex; i++)
            {
                Choices.Add((i+1).ToString());
            }

            view.UniqueIndexdrop.choices = Choices;
            view.Ydrop.index = 0; // 첫 번째 값 선택으로 초기화
        }
    }

    void ReGenerate() //보드와 생성된 벽, 젤리벽 삭제하며 다시 생성
    {
        GameObject destroy = GameObject.Find("[GameBoard](Clone)");
        GameObject destroy1 = GameObject.Find("PlayingBlockParent");
        Destroy(destroy);//
        Destroy(destroy1);
        view.gameBoard.GetComponent<BoardController>().model.nowStageIndex = 5;
        Instantiate(view.gameBoard);
    }

    void extendshapesup() // 젤리 벽 생성 방향
    {
        ExtendPlayinBlock("Up");
        ReGenerate();
    }
    void extendshapesdown() // 젤리 벽 생성 방향
    {
        ExtendPlayinBlock("Down");
        ReGenerate();
    }

    void extendshapesleft() // 젤리 벽 생성 방향
    {
        ExtendPlayinBlock("Left");
        ReGenerate();
    }

    void extendshapesright() // 젤리 벽 생성 방향
    {
        ExtendPlayinBlock("Right");
        ReGenerate();
    }

    void reset() // Custom StageData 초기화
    {
        model.customdata.ResetData();
        model.pbuniqueIndex = 1;
    }

   void ExtendPlayinBlock(string dir) // 젤리 벽 추가 생성 로직 (미완)
{
    foreach (var data in model.customdata.playingBlocks)
    {
        if (model.chooseuniqueindex != data.uniqueIndex)
            continue;

        Vector2Int offset = dir switch
        {
            "Up" => new Vector2Int(0, 1),
            "Down" => new Vector2Int(0, -1),
            "Left" => new Vector2Int(-1, 0),
            "Right" => new Vector2Int(1, 0),
            _ => Vector2Int.zero
        };

        // 기준점: 가장 마지막 블록이 있던 위치 or center
        Vector2Int basePosition = data.center;
        if (data.shapes.Count > 0)
        {
            var lastOffset = data.shapes[data.shapes.Count - 1].offset;
            basePosition += lastOffset;
        }

        // 새로운 좌표 시도
        Vector2Int next = basePosition + offset;

        // 이미 존재하면 offset을 누적해 비어있는 곳 찾기
        while (data.shapes.Any(s => s.offset == next - data.center))
        {
            next += offset;
        }

        // 범위 확인
        if (next.x >= 0 && next.x < model.width && next.y >= 0 && next.y < model.height)
        {
            data.shapes.Add(new ShapeData(next - data.center));
            Debug.Log($"추가된 셀: {next}");
        }
        else
        {
            Debug.LogWarning($"범위를 벗어남: {next}");
        }
    }
}



    private void InputBoardData() //customdata에 boardblocks 값을 지정된 크기만큼 저장
    {
        
        for(var w = 0; w < model.width; w++)
        {
            for(var h = 0; h < model.height; h++)
            {
                model.customdata.boardBlocks.Add(new BoardBlockData(w, h));
            }
        }
    }

    private void InputWallData() //customdata.WallData에 생성한 벽 정보를 저장
    {
        model.customdata.Walls.Add(new Project.Scripts.Data_Script.WallData(model.wallx, model.wally, model.dir, model.len, model.wallcolor, model.gimmick));
    }

    private void InputPlayingBlockData() //customdata.WallData에 생성한 젤리 벽 정보를 저장
    {
        model.customdata.playingBlocks.Add(new PlayingBlockData(model.pbx, model.pby, model.pbuniqueIndex, model.pbcolor));
    }

    public void ExportToJson() //StageData을 JSON으로 변환
    {
        string json = JsonUtility.ToJson(model.customdata, true);
        string customPath = Path.Combine(Application.dataPath, "Project/Resource/Data/Json");


        string path = Path.Combine(customPath, "CustomStageData.json");
        File.WriteAllText(path, json);

        Debug.Log($"Stage data exported to: {path}");
    }

    public StageData LoadStageDataFromJson(string jsonPath) //JSON을 StageData로 변환
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("파일이 존재하지 않습니다: " + jsonPath);
            return null;
        }

        string json = File.ReadAllText(jsonPath);

        // 임시 인스턴스 생성
        StageData stageData = ScriptableObject.CreateInstance<StageData>();

        // JSON 덮어쓰기
        JsonUtility.FromJsonOverwrite(json, stageData);

        return stageData;
    }

    public void importJSONtoStageData() //JSON을 변환한 값을 인스턴스에 적용
    {
        string jsonPath = Application.dataPath + "/Project/Resource/Data/Json/CustomStageData.json";
        StageData loaded = LoadStageDataFromJson(jsonPath);
        model.customdata.boardBlocks = loaded.boardBlocks;
        model.customdata.playingBlocks = loaded.playingBlocks;
        model.customdata.Walls = loaded.Walls;
        model.pbuniqueIndex = loaded.playingBlocks.Count()+1;
        ReGenerate();
        Debug.Log($"Stage data imported to: {jsonPath}");
    }
}