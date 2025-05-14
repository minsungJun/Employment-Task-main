using UnityEngine;
using UnityEngine.UIElements;

public class StageEditor_View : MonoBehaviour
{
    public UIDocument uiDocument;
    [SerializeField] private GameObject GameBoard;
    public GameObject gameBoard {get => GameBoard; set => GameBoard = value;}

    
    private Button BoardGenerate;
    public Button boardgenerate {get => BoardGenerate; set => BoardGenerate = value;}
    private DropdownField WidthDrop;
    public DropdownField widthdrop {get => WidthDrop; set => WidthDrop = value;}
    private DropdownField HeightDrop;
    public DropdownField heightdrop {get => HeightDrop; set => HeightDrop = value;}

    private Button WallGenerate;
    public Button Wallgenerate {get => WallGenerate; set => WallGenerate = value;}
    private DropdownField XDrop;
    public DropdownField Xdrop {get => XDrop; set => XDrop = value;}
    private DropdownField YDrop;
    public DropdownField Ydrop {get => YDrop; set => YDrop = value;}
    private DropdownField ColorDrop;
    public DropdownField Colordrop {get => ColorDrop; set => ColorDrop = value;}
    private DropdownField LengthDrop;
    public DropdownField Lengthdrop {get => LengthDrop; set => LengthDrop = value;}
    private DropdownField DirectionDrop;
    public DropdownField Directiondrop {get => DirectionDrop; set => DirectionDrop = value;}
    private DropdownField GimmickDrop;
    public DropdownField Gimmickdrop {get => GimmickDrop; set => GimmickDrop = value;}

    private Button PBGenerate;
    public Button PBgenerate {get => PBGenerate; set => PBGenerate = value;}
    private DropdownField PBXDrop;
    public DropdownField PBXdrop {get => PBXDrop; set => PBXDrop = value;}
    private DropdownField PBYDrop;
    public DropdownField PBYdrop {get => PBYDrop; set => PBYDrop = value;}
    private DropdownField PBColorDrop;
    public DropdownField PBColordrop {get => PBColorDrop; set => PBColorDrop = value;}
    private DropdownField PBGimmickDrop;
    public DropdownField PBGimmickdrop {get => PBGimmickDrop; set => PBGimmickDrop = value;}


    private DropdownField UniqueIndexDrop;
    public DropdownField UniqueIndexdrop {get => UniqueIndexDrop; set => UniqueIndexDrop = value;}
    private Button Up;
    public Button up {get => Up; set => Up = value;}
    private Button Down;
    public Button down {get => Down; set => Down = value;}
    private Button Left;
    public Button left {get => Left; set => Left = value;}
    private Button Right;
    public Button right {get => Right; set => Right = value;}

    private Button Reset;
    public Button RESET {get => Reset; set => Reset = value;}
    private Button Export;
    public Button export {get => Export; set => Export = value;}
    private Button Import;
    public Button import {get => Import; set => Import = value;}
}
