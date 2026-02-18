using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;


[CreateAssetMenu(fileName = "New Tile Data", menuName = "Scriptable Objects/Tiles/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Tile Data")]
    [SerializeField] private bool canDig;
    [SerializeField] private float hardness; // liek the hp of a tile, drill will decrease "durability" on the instance until its past a "dug" threshold which then would convert it into a wall/background
    [SerializeField] private TileBase[] tileBases;
    [SerializeField] private ParticleSystem drillParticle;

    public ParticleSystem DrillParticle => drillParticle;

    
    public TileBase[] GetTile => tileBases;
    public float GetHardness => hardness;

    public TileBase GetTileFromDura(float durability)
    {
        
        
        float t = 1f - (durability / hardness);
        t = Mathf.Clamp01(t);
        int index = Mathf.RoundToInt(t * (tileBases.Length - 1));

        return tileBases[index];
    }
}
