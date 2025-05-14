using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private GameObject BoardBlockPrefab;
    public GameObject boardBlockPrefab => BoardBlockPrefab;
    [SerializeField] public GameObject BlockGroupPrefab;
    public GameObject blockGroupPrefab => BlockGroupPrefab;
    [SerializeField] public GameObject BlockPrefab;
    public GameObject blockPrefab => BlockPrefab;
    [SerializeField] public Material[] BlockMaterials;
    public Material[] blockMaterials => BlockMaterials;
    [SerializeField] public Material[] TestBlockMaterials;
    public Material[] testBlockMaterials => TestBlockMaterials;
    [SerializeField] public GameObject[] WallPrefabs;
    public GameObject[] wallPrefabs => WallPrefabs;
    [SerializeField] public Material[] WallMaterials;
    public Material[] wallMaterials => WallMaterials;
    [SerializeField] public Transform SpawnerTr;
    public Transform spawnerTr => SpawnerTr;
    [SerializeField] public Transform QuadTr;
    public Transform quadTr => QuadTr;
    [SerializeField] public ParticleSystem DestroyParticlePrefab;
    public ParticleSystem destroyParticlePrefab => DestroyParticlePrefab;

    public List<SequentialCubeParticleSpawner> particleSpawners;
    public List<GameObject> walls = new List<GameObject>();

    public GameObject boardParent { get; set; }
    public GameObject playingBlockParent { get; set; }

    [SerializeField] private GameObject QuadPrefab;
    public GameObject quadPrefab => QuadPrefab;
    
    private List<GameObject> Quads = new List<GameObject>();
    public List<GameObject> quads { get => Quads; set => Quads = value; }



    [SerializeField] private GameObject GameBoard;
    public GameObject gameBoard => GameBoard;


}
