using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GameTile
{
    TileManager tileManager;

    public TileData data;
    public bool isDug;
    public Vector2Int gridIndex;
    public float durability;
    public TileBase currentTile;

    public TileData Data
    {
        get {  return data; }
        set
        {
            data = value;
            durability = data.GetHardness;
        }
    }
    public TileBase GetCurrentTile => currentTile;

    public GameTile(TileManager tileManager, TileData data, Vector2Int gridIndex)
    {
        this.tileManager = tileManager;
        this.data = data;
        isDug = false;
        this.gridIndex = gridIndex;
        durability = data.GetHardness;
    }

    public void TakeDrillDamage(float damage)
    {
        durability -= damage;
        durability = Math.Clamp(durability, 0f, data.GetHardness);
        //Debug.Log("Tile At x: " + gridIndex.x + " y: " + gridIndex.y + " is being drilled, durability: " + durability);
        tileManager.CheckUpdateTile(gridIndex);
        if (durability < 0.5f) // when it becomes dug
        {
            OnDug();
        }
    }

    private void OnDug()
    {
        //Debug.Log("Dug At x: " + gridIndex.x + " y: " + gridIndex.y);
        isDug = true;
        durability = 0.0f;
        Vector3Int grid = new Vector3Int(gridIndex.x, gridIndex.y, 0);
        tileManager.UpdateTile(grid, data.GetTileFromDura(durability));
    }

    public void InstaDig()
    {
        OnDug();
    }
}