using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不实例化，集中存放常用常量和辅助方法
/// </summary>
public class Utilities
{
    #region 路径/文件名
    public const string PREFAB_DIR = "Prefabs/";
    public const string SPRITE_DIR = "Sprites/";

    public const string RESOURCE_BLOCK_NAME = "PixelBlock";
    public const string BUILD_BASIC_NAME = "Block";
    public const string ENEMY_NAME = "Enemy";
    public const string FIRE_NAME = "Firepot";
    public const string WALL_BLOCK_NAME = "WallBlock";
    public const string ENEMY_ARROW_NAME = "EnemyArrow";
    public const string PORTAL_ARROW_NAME = "PortalArrow";
    public const string INVIS_WALL_NAME = "InvisWall";

    public const string ANIM_DESTRUCT_NAME = "DestructAnim";
    public const string ANIM_PORTAL_NAME = "CreatePortalAnim";
    public const string ANIM_SPRAY_NAME = "FireSprayAnim";
    #endregion

    #region 场景
    public const float WORLD_WIDTH = 96;
    public const float WORLD_HEIGHT = 64;
    public const float PIXELS_PER_UNIT = 2;

    // 喷漆标记点
    public const int FLOOR_WIDTH = 130;
    public const int FLOOR_HEIGHT = 114;
    public const int FLOOR_TOTAL = FLOOR_WIDTH * FLOOR_HEIGHT;

    // 生成
    public const float GENERATE_WALL_RATIO = .03f;
    public const float GENERATE_WALL_VARIATION = .1f;
    public const float GENERATE_BLOCK_RATIO = .04f;
    public const float GENERATE_BLOCK_VARIATION = .1f; 
    public const float GENERATE_FIRE_RATIO = .015f;
    public const float GENERATE_FIRE_VARIATION = .1f;
    #endregion

    #region 逻辑
    // 资源获取/消耗
    public const int BUILD_COST = 4;
    public const float SPRAY_COST_PER_SEC = 4;
    public const float RESOURCE_DISPLAY_MAX = 20;

    // 移动相关
    public const float PLAYER_MOVE_SPEED = 8;
    public const float SPRAY_MOVE_BOOST = 1.5f;
    public const float ENEMY_MOVE_SPEED = 4;
    public const float ENEMY_CHASE_RANGE = 24;
    public const float ENEMY_ATTACK_RANGE = 15;

    // 射击
    public const float LAUNCH_SPEED = 40;
    public const float DAMAGE_COOLDOWN = 3;

    // 喷漆
    public const float SPRAY_OFFSET = 5;

    // 连锁破坏
    public const float CHAIN_DESTRUCT_DELAY = .5f;
    public const float CHAIN_DESTRUCT_RADIUS = 3.5f;
    public const int CHAIN_DESTRUCT_POWER = 3;

    // 镜头控制
    public const float PORTAL_COOLDOWN = 1;
    public const float ZOOM_PORTAL_TIME = .6f;
    public const float CAMERA_SIZE_ZOOMIN = 12;
    public const float CAMERA_SIZE_ZOOMOUT = 15;
    public const float CAMERA_DRAG_SPEED = .1f;
    public const float CAMERA_OFFSET = 3;
    public const float HIT_SHAKE_TIME = .3f;
    public const float HIT_SHAKE_INTERVAL = .01f;
    public const float HIT_SHAKE_RANGE = .5f;

    // 游戏时间
    public const float GAME_TIME = 90;
    public const int GAME_START_DELAY = 3;
    public const float COUNTDOWN_THRESHOLD = 10;
    public const float PERCENT_COUNT_TIME_BASE = 1.5f;
    public const float PERCENT_COUNT_TIME_CAP = 3f;
    #endregion

    /// <summary>
    /// 检查坐标是否在场景合理范围内
    /// </summary>
    public static bool CheckWithinBoundaries(Vector3 pPos)
    {
        if (pPos.x > WORLD_WIDTH / 2 || pPos.x < -WORLD_WIDTH / 2) return false;
        if (pPos.y > WORLD_HEIGHT / 2 || pPos.y < -WORLD_HEIGHT / 2) return false;
        return true;
    }

    /// <summary>
    /// 将坐标转换为距离最近的整数坐标
    /// </summary>
    /// <param name="pAwayFrom">远离该坐标取整</param>
    public static Vector3 SnapToGrid(Vector3 pPos, Vector3 pAwayFrom)
    {
        var roundX = (int)(pAwayFrom.x > pPos.x ? pPos.x : pPos.x + .5f);
        var roundY = (int)(pAwayFrom.y > pPos.y ? pPos.y : pPos.y + .5f);
        pPos.x = roundX;
        pPos.y = roundY;
        return pPos;
    }

    public static Vector3 RandomNormalizedDirection()
    {
        var v = Vector3.zero;
        while (v == Vector3.zero)
        {
            v.x = Random.Range(-1f, 1f);
            v.y = Random.Range(-1f, 1f);
        }
        return v.normalized;
    }

    public static float RandomSign()
    {
        var i = Random.Range(-1, 1);
        return Mathf.Sign(i);
    }
}
