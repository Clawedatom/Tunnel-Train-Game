using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileManager : MonoBehaviour // manages game tiles, creates them and updates
{
    #region Class References
    private static TileManager _instance;

    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Tile Fields")]
    [SerializeField] private GameTile[,] gameTiles;

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Vector2Int startBL;
    [SerializeField] private Vector2Int endBL;
    [SerializeField] private int zoneX = 5;
    [SerializeField] private int zoneY = 5;

    [Header("----Map Creation Fields----")]
    [Header("Rock Cluster Fields")]
    [SerializeField] private int rockClusterCount = 5;
    [SerializeField] private int rockClusterMaxWidth = 10;
    //[SerializeField] private int rockClusterCleanupCount = 2;
    [SerializeField] private float noiseScale = 0.08f;
    
    [Header("COlliders")]
    [SerializeField] private BoxCollider2D startZoneCol;
    [SerializeField] private BoxCollider2D endZoneCol;
    [SerializeField] private GameObject startZoneMMGO; // start zone mini map icon, seen on mini map
    [SerializeField] private GameObject endZoneMMGO;

    [Header("Tile Data")]
    [SerializeField] private Tilemap bgLayerTileMap;
    [SerializeField] private Tilemap digLayerTileMap;
    [SerializeField] private TileData dirtTile;
    [SerializeField] private TileData clusterRockTile;
    [SerializeField] private TileData rockTile;
    [SerializeField] private TileData bombTile;

    [Header("Explosion")]
    [SerializeField] private int bombTileCount = 50;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int explosionCellRadius = 3;
    [SerializeField] private AudioClip explosionClip;

    [SerializeField] private bool debugMode = false;
    #endregion

    #region Properties
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TileManager>();
                if (_instance == null)
                {
                    Debug.LogError("TileManager has not been assgined");
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Start Up
    public void OnAwake()
    {
        
    }

    public void OnStart()
    {
        gameTiles = new GameTile[width, height];

        GamePhaseManager.Instance.tunnelPhase.onEnd += HandleTunnelCheck;

        if (startBL.x > width) startBL.x = width;
        if (startBL.y > height) startBL.y = height;
        if (endBL.x > width) endBL.x = width;
        if (endBL.y > height) endBL.y = height;

    }

    public void SetPM()
    {
        if (playerManager == null)
        {
            playerManager = PlayerManager.Instance;
        }
    }

    public void SetUpGameMap()
    {
        
        CreateGameTiles();

        CreateMap();


        //after map/ gametiles is complete
        RenderTileGrid();

        SetUpZones();
        

    }
    private void SetUpZones()
    {
        SetZoneY();

        ClearLevelZones();

        PositionColliders();
    }

    private void SetZoneY()
    {
        int spawnY = UnityEngine.Random.Range(10, (height) + 10);
        startBL.y = spawnY;
        int endY = UnityEngine.Random.Range(10, (height) + 10);
        endBL.y = endY;
    }
    private void CreateGameTiles() // create tile instances 
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileData chosen = ChooseTileData(x, y);
                gameTiles[x, y] = new GameTile(this,chosen, new Vector2Int(x, y));
            }
        }
    }
    private void RenderTileGrid() 
    {
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = GetCellFromGrid(x, y);
              
                TileBase tile = gameTiles[x, y].data.GetTileFromDura(gameTiles[x,y].durability);
                //Debug.Log(gameTiles[x, y].data.GetTileFromDura(gameTiles[x, y].durability));
                
                
                digLayerTileMap.SetTile(cellPos, tile);
            }
        }
    }
    private void ClearLevelZones() // digs out start and finish zones
    {
        //spawn
        DigZone(startBL, zoneX, zoneX);
        startZoneMMGO.transform.localScale = new Vector3(zoneX, zoneY, 1);

        //finish
        DigZone(endBL, zoneX, zoneY);
        endZoneMMGO.transform.localScale = new Vector3(zoneX, zoneY, 1);
    }

    private void PositionColliders() // positions colliders in start and finush zones to detect player
    {
        Vector2 size = new Vector2(zoneX, zoneY);
        startZoneCol.size = size;
        endZoneCol.size = size;

        Vector2Int startCenterIndex = startBL + new Vector2Int(zoneX / 2, zoneX / 2);
        Vector3 startWorld = GridToWorld(startCenterIndex);
        startZoneCol.transform.position = startWorld;

        Vector2Int endCenterIndex = endBL + new Vector2Int(zoneX / 2, zoneY / 2);
        Vector3 endWorld = GridToWorld(endCenterIndex);
        endZoneCol.transform.position = endWorld;

    }

   


    
    

    #endregion

    #region Class Methods

    public void CheckUpdateTile(Vector2Int grid) // Checks if tile durability is low enough to change tile. runs from gameTile, might change.
    {
        if (!IsInBounds(grid.x, grid.y)) return;

        TileBase targetTile = gameTiles[grid.x, grid.y].data.GetTileFromDura(gameTiles[grid.x, grid.y].durability);

        if (targetTile != gameTiles[grid.x,grid.y].currentTile)
        {
            Vector3Int cell = GetCellFromGrid(grid.x,grid.y);
            digLayerTileMap.SetTile(cell, targetTile);
            gameTiles[grid.x,grid.y].currentTile = targetTile;
        }
    }

    public Vector3 GetStartPos() // gets center of start zone as a start position for player/ bfs
    {
        Vector2Int startCenterIndex = startBL;
        startCenterIndex.x += zoneX/2;
        startCenterIndex.y += zoneY/2;
        Vector3 world = GridToWorld(startCenterIndex);

        return world;
    }

    public float GetDistToEnd(Vector3 pos)
    {
        return Vector3.Distance(GridToWorld(endBL), pos);
    }

    #endregion

    #region Dig Methods
    public void DrillAtIndex(Vector2Int index, int range, float damage) // gets index from drill tip, reduces durability of tiles within a range
    {
        
        for (int x = index.x - range; x <= index.x + range; x++)
        {
            for (int y = index.y - range; y <= index.y + range; y++)
            {
                if (IsInBounds(x, y) && !gameTiles[x, y].isDug)
                {
                    
                    gameTiles[x, y].TakeDrillDamage(damage);
                    CheckDrillTile(x, y);   
                    
                }
            }
        }
       
    }

    private void CheckDrillTile(int x, int y)
    {
        TileData data = gameTiles[x, y].data;
        if (data == bombTile)
        {

            //expolsion
            CreateExplosion(x, y);
            //damage random player part
            playerManager.TakeRandomDamage();
        }
        else if (data == clusterRockTile)
        {
            playerManager.GetStats.DamageDrill();
        }
    }


    private void CreateExplosion(int x, int y) //grid x y
    {
        for (int gx = x - explosionCellRadius; gx <= x + explosionCellRadius; gx++)
        {
            for (int gy = y - explosionCellRadius; gy <= y + explosionCellRadius;gy++)
            {
                if (!IsInBounds(gx, gy)) continue;
                gameTiles[gx, gy].InstaDig();
            }
        }
        StartCoroutine(CreateExplosionAtPos(GridToWorld(new Vector2Int(x, y))));
    }

    public IEnumerator CreateExplosionAtPos(Vector3 pos)
    {
        GameObject explosionGO = Instantiate(explosionPrefab, pos, Quaternion.identity);
        SoundManager.Instance.PlaySoundAtPos(explosionClip, pos);
        yield return new WaitForSeconds(1.5f);

        //
        
        Destroy(explosionGO);
        
        

    }
    private void DigZone(Vector2Int bottomLeft, int w, int h) // insta digs out a defined zones
    {
        for (int x = bottomLeft.x; x < bottomLeft.x + w; x++)
        {
            for (int y = bottomLeft.y; y < bottomLeft.y + h; y++)
            {
                if (!IsInBounds(x, y)) continue;
                //dig
                //Debug.Log("Dug At x: " + x + " y: " + y);
                gameTiles[x, y].InstaDig();
            }
        }
    }

    public void UpdateTile(Vector3Int gridPos, TileBase tile) // uses 2 tilemaps, dig layer for collision, bg to see dug tiles
    {
        if (!IsInBounds(gridPos.x, gridPos.y)) return;

        Vector3Int cell = GetCellFromGrid(gridPos.x, gridPos.y);


        digLayerTileMap.SetTile(cell, null);

        bgLayerTileMap.SetTile(cell, tile);

        gameTiles[gridPos.x, gridPos.y].currentTile = tile;
        gameTiles[gridPos.x,gridPos.y].isDug = true;
    }
    #endregion

    #region Debug methods

    private void OnDrawGizmos()
    {

        if (width <= 0 || height <= 0)
            return;

        // Bounds box
        Gizmos.color = new Color32(92, 64, 51, 60);

        Vector3 origin = transform.position;
        Vector3 center = origin + new Vector3(width * 0.5f, height * 0.5f, 0f);

        Gizmos.DrawCube(center, new Vector3(width, height, 1));

        // Grid lines
        Gizmos.color = new Color32(255, 255, 255, 80);

        // Vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 from = origin + new Vector3(x, 0, 0);
            Vector3 to = origin + new Vector3(x, height, 0);
            Gizmos.DrawLine(from, to);
        }

        // Horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 from = origin + new Vector3(0, y, 0);
            Vector3 to = origin + new Vector3(width, y, 0);
            Gizmos.DrawLine(from, to);
        }


        //start and finish 
        VisualiseDigBox(startBL, Color.green);
        VisualiseDigBox(endBL, Color.red);
    }

    private void VisualiseDigBox(Vector2 bottomLeft, Color col)
    {


        Vector3 worldBL = GridToWorld(new Vector2Int((int)bottomLeft.x, (int)bottomLeft.y));
        //top
        Vector3 to = new Vector3(worldBL.x, worldBL.y + zoneY, 0);
        Vector3 from = new Vector3(worldBL.x + zoneX, worldBL.y + zoneY, 0);
        Debug.DrawLine(to, from, col);
        //bottom
        to = new Vector3(worldBL.x, worldBL.y, 0);
        from = new Vector3(worldBL.x + zoneX, worldBL.y, 0);
        Debug.DrawLine(to, from, col);
        //left
        to = new Vector3(worldBL.x, worldBL.y, 0);
        from = new Vector3(worldBL.x, worldBL.y + zoneY, 0);
        Debug.DrawLine(to, from, col);
        //right
        to = new Vector3(worldBL.x + zoneX, worldBL.y, 0);
        from = new Vector3(worldBL.x + zoneX, worldBL.y + zoneY, 0);
        Debug.DrawLine(to, from, col);
    }

    

    public void VisualisePath(List<Vector2Int> path, Color col) // changes colour of each tile in path so it is visible - debug
    {
        if (path == null || !debugMode) return;
        foreach (Vector2Int index in path)
        {
            Vector3Int cell = GetCellFromGrid(index.x, index.y);

            bgLayerTileMap.SetTileFlags(cell, TileFlags.None);
            bgLayerTileMap.SetColor(cell, col);
        }
    }
    #endregion

    #region Tunnel Check Methods
    public void HandleTunnelCheck() 
    {
        List<Vector2Int> path = TunnelChecker.SearchTunnel(Instance); // returns a tunnel, not checked if sutiable for train yet

        //visualise 
        if (path != null)
        {
            VisualisePath(path, Color.red);
            TrainManager.Instance.CreateTrainPath(path); // if there is a path, move on to making the train path

        }
        else
        {
            Debug.Log("No Valid Tunnel Exists");
        }

    }

    public List<Vector2Int> AddStartToTrainPath(List<Vector2Int> trainPath)
    {
        Vector2Int startCell = startBL;
                
        for (int x = startCell.x; x < (startCell.x + zoneX / 2); x++) 
        {
            trainPath.Add(new Vector2Int(x, startBL.y));
        }

        return trainPath;
    }

    public List<Vector2Int> AddEndToTrainPath(List<Vector2Int> trainPath)
    {
        Vector2Int endCell = endBL;
       
         
        for (int x = endCell.x; x < endCell.x + zoneX; x++)
        {
            trainPath.Add(new Vector2Int(x, endCell.y));
        }

        return trainPath;
    }
    #endregion


    #region Map Creation
    private void CreateMap()
    {
     

       CreateRockClustersPerlin();

        //small open zones
        AddBombTiles();
        // death tiles
    }

    private void AddBombTiles()
    {
        for (int i = 0; i <= bombTileCount; i++)
        {
            Vector2Int grid = RandomGridIndex();

            
            gameTiles[grid.x, grid.y].data = bombTile;
        }
    }

    private void CreateRockClustersPerlin()
    {
        float scale = 0.08f;      // bigger = smaller blobs
        float threshold = 0.55f;  // lower = more rock
        float offsetX = UnityEngine.Random.Range(0f, 9999f);
        float offsetY = UnityEngine.Random.Range(0f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = (x + offsetX) * scale;
                float ny = (y + offsetY) * scale;

                float n = Mathf.PerlinNoise(nx, ny);

                if (n > threshold)
                {
                    gameTiles[x, y].Data = clusterRockTile;
                    
                }
            }
        }
    }

    //NOT USED ROCK CLUSTERS - looked like fireworks

    private void CreateRockClusters()
    {
        if (rockClusterCount == 0) return;
        

        float offsetX = UnityEngine.Random.Range(0f, 9999f);
        float offsetY = UnityEngine.Random.Range(0f, 9999f);

        int radius = Mathf.Max(4, rockClusterMaxWidth); // min radius is 4

        for (int i = 0; i < rockClusterCount; i++)
        {
           //Create cluster

            Vector2Int center = RandomGridIndex(); // get random grid point for center of cluster
            

            for (int dx = -radius; dx <= radius; dx++) // loop through radius square
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int x = center.x + dx; 
                    int y = center.y + dy;

                    if (!IsInBounds(x,y)) continue;

                    float dist = Mathf.Sqrt((x - center.x) * (x - center.x) +
                        (y - center.y) * (y - center.y));
                    if (dist > radius) continue; // shouldnt be

                    float radialFalloff = 1f - (dist / radius);

                    float nx = (x + offsetX) * noiseScale;
                    float ny = (y + offsetY) * noiseScale;

                    float noise = Mathf.PerlinNoise(nx, ny);

                    float combined = noise * radialFalloff;

                    if (combined > 0.35f)
                    {
                        gameTiles[x, y].data = clusterRockTile;
                    }
                }
            }
            Debug.Log("Created Rock Cluster at: " + center);
            
        }
    }

    private void SmoothRockClusters()
    {
        if (digLayerTileMap == null || gameTiles == null) return;

        // read current state into temporary buffer
        GameTile[,] ogTiles = gameTiles;
        

       
        GameTile[,] result = new GameTile[width, height];

        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result[x, y] = gameTiles[x, y];

                int neighbourCount = CountRockClusterNeighbours(ogTiles, x, y);

                // default to current buffer state
                GameTile current = ogTiles[x, y];

                // If cell currently cluster rock and has very few neighbours -> turn to dirt
                

                if (current.data == clusterRockTile) // if tile is a cluster rock
                {
                    if (neighbourCount <= 1) // and it has less than 1 neighbour
                    {
                        // become dirt
                        result[x, y].Data = dirtTile;
                        //current = dirtTile.GetTileFromDura(gameTiles[x, y].durability);
                    }
                }
                else
                {
                    // if many neighbors are cluster rock, convert this cell to cluster rock
                    if (neighbourCount >= 3)
                    {
                        result[x, y].Data = clusterRockTile;
                        
                    }
                }

               
            }
        }

        gameTiles = result;
    }

    private int CountRockClusterNeighbours(GameTile[,] tiles, int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // exclude self
                int nx = x + i;
                int ny = y + j;
                if (!IsInBounds(nx, ny)) continue;

                GameTile neighborTile = tiles[nx, ny];
                if (neighborTile == null) continue;

                // Compare against cluster tile computed for that neighbor's durability
                if (neighborTile.data == clusterRockTile)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private Vector2Int RandomGridIndex()
    {
        // pick a point not too close to borders so cluster radius fits
        //int attempt = 0;
        //while (true)
        //{
        //    attempt++;
        //    if (attempt > 1000) 
        //        return new Vector2Int(Mathf.Clamp(width / 2, 0, width - 1), Mathf.Clamp(height / 2, 0, height - 1));

        //    int x = UnityEngine.Random.Range(rockClusterMaxWidth, width - rockClusterMaxWidth);
        //    int y = UnityEngine.Random.Range(rockClusterMaxWidth, height - rockClusterMaxWidth);

        //    if (x >= rockClusterMaxWidth &&
        //        x < width - rockClusterMaxWidth &&
        //        y >= rockClusterMaxWidth &&
        //        y < height - rockClusterMaxWidth) // far enough away from sides
        //    {
        //        return new Vector2Int(x, y);
        //    }
        //}
        int x = UnityEngine.Random.Range(rockClusterMaxWidth, width - rockClusterMaxWidth);
        int y = UnityEngine.Random.Range(rockClusterMaxWidth, height - rockClusterMaxWidth);

        return new Vector2Int(x, y);
    }

    public void SetTopLayerTile(TileBase tile, Vector2Int pos)
    {
        digLayerTileMap.SetTile(GetCellFromGrid(pos.x,pos.y), tile);
    }
    #endregion




    #region Helper Methods
    private Vector3Int GetCellFromGrid(int gx, int gy)
    {
        
        int originX = Mathf.RoundToInt(transform.position.x);
        int originY = Mathf.RoundToInt(transform.position.y);
        return new Vector3Int(gx + originX, gy + originY, 0);
    }
    public bool IsDug(Vector2Int cell) // used in train manager
    {
        return gameTiles[cell.x, cell.y].isDug;
    }

    public bool IsInBounds(int x, int y) // returns true is cell is in grid bounds
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;
        return true;
    }
    TileData ChooseTileData(int x, int y) // temp - planning on a better map
    {


        if (y > height / 4)
            return dirtTile;

        return rockTile;
    }

    public bool IsGridIndexInFinish(int x, int y) // helper for bfs, checks if cell is in the end zone
    {
        for (int i = endBL.x; i < endBL.x + zoneX; i++)
        {
            for (int j = endBL.y; j < endBL.y + zoneY; j++)
            {
                if (x == i && y == j) { return true; }
            }
        }
        return false;
    }
    public Vector2Int WorldToGrid(Vector3 worldPos) 
    {
        float x = worldPos.x;
        float y = worldPos.y;

        float lx = x - transform.position.x;
        float ly = y - transform.position.y;

        Vector2Int grid = new Vector2Int(Mathf.FloorToInt(lx), Mathf.FloorToInt(ly));

        return grid;
    }

    public Vector3 GridToWorld(Vector2Int gridIndex)
    {
        float lx = transform.position.x + gridIndex.x;
        float ly = transform.position.y + gridIndex.y;

        return new Vector3(lx, ly, 0);
    }
    #endregion

    private void OnDestroy()
    {
        if (GamePhaseManager.Instance != null)
        {
            GamePhaseManager.Instance.tunnelPhase.onEnd -= HandleTunnelCheck;

        }
    }

    public TileData TileDataFromWorldPos(Vector3 worldPos)
    {
        //world to grid
        Vector2Int gridPos = WorldToGrid(worldPos);

        if (IsInBounds(gridPos.x,gridPos.y))
        {
            return gameTiles[gridPos.x, gridPos.y].Data;

        }
        return null;
        
    }
}

