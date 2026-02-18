
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;


public class TrainManager : MonoBehaviour
{
	#region Class References
	private static TrainManager _instance;

    TileManager tileManager;
    #endregion


    #region Private Fields
    [Header("Calc Fields")]
    [SerializeField] private List<Vector2Int> trainPath = new List<Vector2Int>();

    [SerializeField] private int maxDrop = 1;//how far down the next cell can be
    [SerializeField] private int maxClimb = 1;

    [Header("Train")]
    [SerializeField] private bool reachedEnd;
    SplineContainer splineContainer;
    [SerializeField] private GameObject trainGO;
    [SerializeField] private GameObject trainPrefab;
    [SerializeField] private float trainSpeed = 5f;
    private float trainYOffset = 2.5f;
    private int index;
    [SerializeField]private float t;
    [SerializeField] private TileData trainTile;
    [SerializeField] private Vector2Int? failedCell = null;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject explosionGO;
    [SerializeField] private float explosionTimer = 2f;
    #endregion

    #region Properties
    public static TrainManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TrainManager>();
                if (_instance == null)
                {
                    Debug.LogError("TrainManager has not been assgined");
                }
            }
            return _instance;
        }
    }
    public GameObject GetTrainGO => trainGO;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        tileManager = TileManager.Instance;
    }
    public void OnStart()
    {

        GamePhaseManager.Instance.trainPhase.onStart += OnStartTrainPhase;
        
        trainGO = null;
        reachedEnd = false;
    }
    #endregion

    #region Class Methods
    public void CreateTrainPath(List<Vector2Int> tunnelPath)
    {
        trainPath = new List<Vector2Int>();
        //start

        trainPath = tileManager.AddStartToTrainPath(trainPath);
        for (int i = 0; i < tunnelPath.Count; i++)
        {
            Vector2Int floorCell = tunnelPath[i];
            floorCell = SnapToFloor(floorCell);
            trainPath.Add(floorCell);
        }

        trainPath = tileManager.AddEndToTrainPath(trainPath);
        

        
        tileManager.VisualisePath(trainPath, Color.blue);
        Vector2Int? failCell = ValidateTrainPath(trainPath);

        if (failCell == null)
        {
            Debug.Log("Valid train path");
            failedCell = null;
        }
        else
        {
            failedCell = failCell.Value;
            Debug.Log("Invalid Train path");
        }
        CreateSplineTrainPath(trainPath);
        SetTrainTiles(trainPath);
    }

    private void SetTrainTiles(List<Vector2Int> trainPath)
    {
        foreach(Vector2Int tile in trainPath)
        {
            tileManager.SetTopLayerTile(trainTile.GetTile[0], tile);
        }
    }
    private Vector2Int? ValidateTrainPath(List<Vector2Int> path)
    {
        HashSet<Vector2Int> set = new HashSet<Vector2Int>(path);

        for (int i = 0; i < path.Count; i++) 
        {
            Vector2Int cell = path[i]; 
            bool hasValidNext = false;

            foreach (Vector2Int dir in new[] { Vector2Int.left, Vector2Int.right }) // check left and right of cell
            {
                for (int dy = -maxDrop; dy <= maxClimb; dy++) //checks if there is a close enough cell to path cell[i] within limits
                {
                    Vector2Int candidate = new Vector2Int(
                        cell.x + dir.x,
                        cell.y + dy
                    );

                    if (!set.Contains(candidate)) //checks if cell is part of train path, hashset faster
                        continue;

                    
                    

                    hasValidNext = true; // is valid so stop checking neighbours and more to next cell in path
                    break;
                }
            }

            // did not find value path, make sure not checking end or start
            if (!hasValidNext && i != 0 && i != path.Count - 1)
                return cell; // cell where the path failed
        }

        return null;
    }

    private Vector2Int SnapToFloor(Vector2Int cell) // moves path cell down until cell bellow is not dug 
    {
        while (tileManager.IsInBounds(cell.x, cell.y - 1) &&
               tileManager.IsDug(new Vector2Int(cell.x, cell.y - 1)))
        {
            cell.y--;
        }

        return cell;
    }
    private void CreateTrain() 
    {
        trainGO = Instantiate(trainPrefab, TileManager.Instance.GetStartPos(), Quaternion.identity);
    }

    private void CreateSplineTrainPath(List<Vector2Int> path)
    {
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }
        splineContainer.Splines = new Spline[0];

        Spline spline = new Spline();
        spline.Closed = false;

        foreach(Vector2Int cell in path)
        {
            if (failedCell.HasValue && cell == failedCell.Value) break; // stop at fail 
            Vector3 worldPos = tileManager.GridToWorld(cell);
            Vector3 local = splineContainer.transform.InverseTransformPoint(worldPos);
            spline.Add(new BezierKnot(local));
        }

        splineContainer.AddSpline(spline);
    }
    #endregion

    #region Update methods
    public void OnUpdate()
    {
        if (trainGO == null) return;

        //move train along track

        UpdateTrain();
    }

    private void UpdateTrain()
    {
        if (trainGO == null || splineContainer == null)
            return;

        t += Time.deltaTime * trainSpeed;

        float splineLength = splineContainer.Spline.GetLength();
        float normalizedT = t / splineLength;

        if (normalizedT >= 1f && !reachedEnd)
        {
            OnEndOfTrack();
        }

        Vector3 pos = splineContainer.EvaluatePosition(normalizedT);
        pos.y += trainYOffset;
        trainGO.transform.position = pos;

        
        Vector3 tangent = splineContainer.EvaluateTangent(normalizedT);

        Vector2 dir = new Vector2(tangent.x, tangent.y);

        if (dir.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            trainGO.transform.rotation = Quaternion.Euler(0f, 0f, angle); 
        }
        
    }
    private void OnEndOfTrack()
    {
        reachedEnd = true;
        if (failedCell.HasValue)
        {
            //fail 
            DestroyTrain();
        }
        else
        {
            //win
            GameManager.Instance.OnTrainPhaseEnd();
        }
    }
    private void DestroyTrain()
    {
        Debug.Log("Your Train Failed");
        explosionGO = Instantiate(explosionPrefab, trainGO.transform.position, Quaternion.identity, null);
        Debug.Log(explosionGO.name);
        StartCoroutine(DestroyTrainGOAndAddExplosion());
    }

    private IEnumerator DestroyTrainGOAndAddExplosion()
    {
        yield return new WaitForSeconds(explosionTimer);

        //
        Destroy(trainGO);
        Destroy(explosionGO);
        GameManager.Instance.OnTrainPhaseFail();
        //PlayerUIManager.Instance.LevelComplete_HandleLevelFail();

    }
    #endregion

    public void OnStartTrainPhase()
    {
        CreateTrain();
        CameraHandler.Instance.SetTarget(GetTrainGO.transform);
    }

    private void OnDestroy()
    {
        if (GamePhaseManager.Instance != null)
        {
           GamePhaseManager.Instance.trainPhase.onStart -= OnStartTrainPhase;

        }
    }
}
