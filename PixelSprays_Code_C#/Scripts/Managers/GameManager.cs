using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏进程中控
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager mInstance;
    public static GameManager Instance
    {
        get { return mInstance; }
    }
    public static bool IsPlaying = false;

    public GameOver mGameOver;

    #region 变量
    // 流程
    private float mGameTime = Utilities.GAME_TIME;
    private float mGameClock = 0f;

    // 生成信息
    private int mEnemyNum = 3;

    private GridPoint[,] mGrid;
    private SortedList<int, int> mAvailable = new SortedList<int, int>();
    private Vector3 mPivot;
    private Vector2 mSize;
    private float mUnitLength;

    // 传送门
    private float mPortalCooldown = 0;
    private Dictionary<Vector2, Portal> mPortals = new Dictionary<Vector2, Portal>();
    private Dictionary<Vector2, GameObject> mPortalArrows = new Dictionary<Vector2, GameObject>();

    #endregion

    private void Awake()
    {
        mInstance = this;
        DontDestroyOnLoad(this);
        PrefabManager.Instance.Init();
        Inventory.Instance.Init();
    }

    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        if (mInstance == this) mInstance = null;
    }

    private void FixedUpdate()
    {
        if (!IsPlaying) return;

        mGameClock -= Time.fixedDeltaTime;
        HUD.Current.UpdateTimer(mGameClock);
        if (mGameClock < 0)
        {
            OnGameOver();
            return;
        }

        if (mPortalCooldown > 0)
        {
            mPortalCooldown -= Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// 进入游戏场景，按钮触发
    /// </summary>
    public void StartGame()
    {
        Inventory.Instance.Init();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    /// <summary>
    /// 通知管理器开始一局游戏
    /// </summary>
    /// <param name="pWidth">地图宽度（格子数）</param>
    /// <param name="pHeight">地图高度（格子数）</param>
    /// <param name="pUnitLength">单位格子对应Unity坐标长度</param>
    public void StartGame(int pWidth, int pHeight, float pUnitLength)
    {
        GenerateMap(pWidth, pHeight, pUnitLength);

        IsPlaying = true;
        mGameClock = mGameTime;
    }

    /// <summary>
    /// 转换至传送门模式
    /// </summary>
    public void EnterPortalView()
    {
        if (mPortalCooldown > 0) return;

        CameraFollow.Current?.EnterPortalView();
    }

    /// <summary>
    /// 转换至正常游戏镜头模式
    /// </summary>
    public void EnterNormalView()
    {
        mPortalCooldown = Utilities.PORTAL_COOLDOWN;
        CameraFollow.Current?.ExitPortalView();
    }
    
    public void AddPortal(Vector3 pPos, Portal pPortal)
    {
        var gridPos = WorldPosToGrid(pPos);
        if (mPortals.ContainsKey(gridPos)) return;
        mPortals.Add(gridPos, pPortal);
        if (!mPortalArrows.ContainsKey(gridPos))
        {
            var arrowPrefab = PrefabManager.Instance.GetPrefab(Utilities.PORTAL_ARROW_NAME);
            var arrow = GameObject.Instantiate(arrowPrefab, pPortal.ExitPosition + Vector3.forward * -5,
                Quaternion.LookRotation(Vector3.forward, pPos - pPortal.ExitPosition));
            arrow.SetActive(false);
            mPortalArrows.Add(gridPos, arrow);
        }
        mPortalCooldown = Utilities.PORTAL_COOLDOWN;
    }

    public void RemovePortal(Vector3 pPos)
    {
        var gridPos = WorldPosToGrid(pPos);
        mPortals.Remove(gridPos);
        if (mPortalArrows.ContainsKey(gridPos))
        {
            var arrow = mPortalArrows[gridPos];
            mPortalArrows.Remove(gridPos);
            Destroy(arrow);
        }
    }

    public Portal GetPortal(Vector3 pPos)
    {
        var gridPos = WorldPosToGrid(pPos);

        int[] sequence = { 0, -1, 1, -2, 2, -3, 3 };
        for (int i = 0; i < sequence.Length; i++)
        {
            for (int j = 0; j < sequence.Length; j++)
            {
                var pos = new Vector2(gridPos.x + sequence[i], gridPos.y + sequence[i]);
                if (mPortals.ContainsKey(pos)) return mPortals[pos];
            }
        }

        return null;
    }

    public void ToggleShowPortalArrows(bool pShow)
    {
        foreach (var item in mPortalArrows)
        {
            item.Value.SetActive(pShow);
        }
    }

    public void UpdateGameConfig(int pEnemyNum, float pGameTime) 
        => (mEnemyNum, mGameTime) = (pEnemyNum, pGameTime);

    private void OnGameOver()
    {
        IsPlaying = false;
        HUD.Current.ToggleTimerAnimation(false);
        float player, enemy;
        int result = FloorManager.Current.CheckGameResult(out player, out enemy);
        mGameOver?.Show(result, player, enemy);
    }

    /// <summary>
    /// 随机配置场景
    /// </summary>
    private void GenerateMap(int pWidth, int pHeight, float pUnitLength)
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        mGrid = new GridPoint[pWidth, pHeight];
        mSize = new Vector2(pWidth, pHeight);
        mUnitLength = pUnitLength;
        mAvailable.Clear();
        for (int i = 0; i < pWidth; i++)
        {
            mAvailable.Add(i, pHeight);
        }

        // 1. 填充四周墙壁
        for (int i = 0; i < pWidth; i++)
        {
            mGrid[i, 0] = GridPoint.Wall;
            mGrid[i, pHeight - 1] = GridPoint.Wall;
        }
        for (int i = 0; i < pHeight; i++)
        {
            mGrid[0, i] = GridPoint.Wall;
            mGrid[pWidth - 1, i] = GridPoint.Wall;
        }
        {
            mAvailable.Remove(0);
            mAvailable.Remove(pWidth - 1);
            var keys = mAvailable.Keys;
            for (int i = 0; i < keys.Count; i++)
            {
                mAvailable[keys[i]] -= 2;
            }
        }

        // 2. 随机填充一定数量的墙体
        int wallNum = RandomSpawnNum(pWidth, pHeight, Utilities.GENERATE_WALL_RATIO, Utilities.GENERATE_WALL_VARIATION);
        RandomlySpawn(GridPoint.Wall, wallNum, pWidth, pHeight);

        // 3. 随机填充一定数量的资源块
        int blockNum = RandomSpawnNum(pWidth, pHeight, Utilities.GENERATE_BLOCK_RATIO, Utilities.GENERATE_BLOCK_VARIATION);
        RandomlySpawn(GridPoint.Block, blockNum, pWidth, pHeight);

        // 3. 随机填充一定数量的火堆
        int fireNum = RandomSpawnNum(pWidth, pHeight, Utilities.GENERATE_FIRE_RATIO, Utilities.GENERATE_FIRE_VARIATION);
        RandomlySpawn(GridPoint.Fire, fireNum, pWidth, pHeight);

        // 3. 随机生成3个敌人
        RandomlySpawn(GridPoint.Enemy, mEnemyNum, pWidth, pHeight);

        // 4. 按grid的配置生成对应物体
        mPivot = Vector3.zero;
        mPivot.x -= (pWidth - 1) * pUnitLength / 2;
        mPivot.y -= (pHeight - 1) * pUnitLength / 2;

        var wallPrefab = PrefabManager.Instance.GetPrefab(Utilities.WALL_BLOCK_NAME);
        var blockPrefab = PrefabManager.Instance.GetPrefab(Utilities.BUILD_BASIC_NAME);
        var enemyPrefab = PrefabManager.Instance.GetPrefab(Utilities.ENEMY_NAME);
        var firePrefab = PrefabManager.Instance.GetPrefab(Utilities.FIRE_NAME);

        for (int i = 0; i < pWidth; i++)
        {
            for (int j = 0; j < pHeight; j++)
            {
                var pos = mPivot + new Vector3(i * pUnitLength, j * pUnitLength, 0);
                switch (mGrid[i, j])
                {
                    case GridPoint.Wall:
                        Instantiate(wallPrefab, pos, Quaternion.identity);
                        break;
                    case GridPoint.Block:
                        Instantiate(blockPrefab, pos, Quaternion.identity);
                        break;
                    case GridPoint.Enemy:
                        Instantiate(enemyPrefab, pos, Quaternion.identity);
                        break;
                    case GridPoint.Fire:
                        Instantiate(firePrefab, pos, Quaternion.identity);
                        break;
                    default:
                        break;
                }
            }
        }

        // 5. 生成空气墙围绕场景
        var invisWallPrefab = PrefabManager.Instance.GetPrefab(Utilities.INVIS_WALL_NAME);
        {
            // 上
            var invisWall = Instantiate(invisWallPrefab, new Vector3(
                0, (pHeight + 1) * pUnitLength / 2, 0), Quaternion.identity);
            invisWall.transform.localScale = new Vector3(pWidth * pUnitLength, pUnitLength, 0);
            // 下
            invisWall = Instantiate(invisWallPrefab, new Vector3(
                0, -(pHeight + 1) * pUnitLength / 2, 0), Quaternion.identity);
            invisWall.transform.localScale = new Vector3(pWidth * pUnitLength, pUnitLength, 0);
            // 左
            invisWall = Instantiate(invisWallPrefab, new Vector3(
                -(pWidth + 1) * pUnitLength / 2, 0, 0), Quaternion.identity);
            invisWall.transform.localScale = new Vector3(pUnitLength, pHeight * pUnitLength, 0);
            // 右
            invisWall = Instantiate(invisWallPrefab, new Vector3(
                (pWidth + 1) * pUnitLength / 2, 0, 0), Quaternion.identity);
            invisWall.transform.localScale = new Vector3(pUnitLength, pHeight * pUnitLength, 0);
        }
        
    }

    public Vector3 GetClosestEmptyPos(Vector3 pPos)
    {
        var gridPos = WorldPosToGrid(pPos);
        var points = new Queue<Vector2>();
        var visited = new List<Vector2>();
        var curr = new Vector2();
        int[] sequence = { 0, -1, 1 };

        points.Enqueue(gridPos);
        visited.Add(gridPos);

        while (points.Count > 0)
        {
            curr = points.Dequeue();
            if (mGrid[(int)curr.x, (int)curr.y] == GridPoint.Empty)
            {
                return (GridToWorldPoint((int)curr.x, (int)curr.y));
            }
            for (int i = 0; i < sequence.Length; i++)
            {
                for (int j = 0; j < sequence.Length; j++)
                {
                    var next = new Vector2(curr.x + sequence[i], curr.y + sequence[j]);
                    if (!CheckGridPosValid(next)) continue;
                    if (visited.Contains(next)) continue;
                    points.Enqueue(next);
                    visited.Add(next);
                }
            }
        }
        return Vector3.zero;
    }

    public void PauseGame(GameObject pMenu)
    {
        IsPlaying = false;
        pMenu.SetActive(true);
    }

    public void ResumeGame(GameObject pMenu)
    {
        IsPlaying = true;
        pMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Destroy(gameObject);
    }

    private Vector3 GridToWorldPoint(int pX, int pY)
    {
        var pos = mPivot + new Vector3(pX * mUnitLength, pY * mUnitLength, 0);
        return pos;
    }

    private Vector2 WorldPosToGrid(Vector3 pPos)
    {
        var gridPos = new Vector2();
        gridPos.x = (int)((pPos.x - mPivot.x) / mUnitLength + .5f);
        gridPos.y = (int)((pPos.y - mPivot.y) / mUnitLength + .5f);

        return gridPos;
    }

    private bool CheckGridPosValid(Vector2 pGridPos)
    {
        if (pGridPos.x < 0 || pGridPos.x > mSize.x - 1) return false;
        if (pGridPos.y < 0 || pGridPos.y > mSize.y - 1) return false;
        return true;
    }

    private void RandomlySpawn(GridPoint pType, int pCount, float pWidth, float pHeight)
    {
        while (pCount-- > 0)
        {
            int x = Random.Range(1, mAvailable.Count);
            x = mAvailable.Keys[x];
            int y = Random.Range(0, mAvailable[x]);
            {
                for (int i = 0; i < pHeight; i++)
                {
                    if (mGrid[x, i] != GridPoint.Empty) continue;
                    if (y-- == 0)
                    {
                        y = i;
                        break;
                    }
                }
            }

            if (--mAvailable[x] == 0)
            {
                mAvailable.Remove(x);
            }
            mGrid[x, y] = pType;
        }
    }

    private int RandomSpawnNum(int pWidth, int pHeight, float pNumRatio, float pVariation)
    {
        float num = pWidth * pHeight * pNumRatio;
        num = Random.Range(num * (1 - pVariation), num * (1 + pVariation));

        return (int)num;
    }

    public enum GridPoint
    {
        Empty = 0,
        Wall = 1,
        Player = 2,
        Block = 3,
        Enemy = 4,
        Fire = 5
    }
}
