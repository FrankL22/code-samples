using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    private static FloorManager mCurrent;
    public static FloorManager Current
    {
        get { return mCurrent; }
    }

    #region 常量
    private const int SPRAY_RADIUS = 4;
    #endregion

    #region 变量
    private ComputeBuffer mBuffer;
    private float[] mSprays = new float[Utilities.FLOOR_TOTAL];
    private float mPlayerPercentage = 0;
    private float mEnemyPercentage = 0;

    private Vector2 mSize;
    private Vector2 mInterval;
    private const float PERCENT_PER_POINT = 1 / (float)Utilities.FLOOR_TOTAL;
    #endregion

    private void Awake()
    {
        mCurrent = this;

        var sprite = GetComponent<SpriteRenderer>().sprite;
        mSize = sprite.rect.size / sprite.pixelsPerUnit;
        mInterval = new Vector2(mSize.x / Utilities.FLOOR_WIDTH, mSize.y / Utilities.FLOOR_HEIGHT);

        Shader.SetGlobalInt("floorWidth", Utilities.FLOOR_WIDTH);
        Shader.SetGlobalInt("floorHeight", Utilities.FLOOR_HEIGHT);
        mBuffer = new ComputeBuffer(Utilities.FLOOR_TOTAL, sizeof(float));
        UpdateShaderGlobals();
    }

    private void OnDestroy()
    {
        mBuffer.Release();
        if (mCurrent == this) mCurrent = null;
    }

    #region Public方法
    /// <summary>
    /// 喷漆染色
    /// </summary>
    /// <param name="pPosition">喷漆中心点世界坐标</param>
    /// <param name="pByPlayer">是否是由玩家喷漆</param>
    /// <param name="pRadius">喷漆半径（标记点数量）</param>
    public void OnSpray(Vector3 pPosition, bool pByPlayer, int pRadius = SPRAY_RADIUS)
    {
        var uv = WorldPosToUV(pPosition);
        Spray((int)uv.x, (int)uv.y, pByPlayer, pRadius);
    }

    public Vector2 WorldPosToUV(Vector3 pPosition)
    {
        pPosition -= transform.position;
        int x = (int)((pPosition.x + mSize.x / 2) / mInterval.x + 0.5f);
        int y = (int)((pPosition.y + mSize.y / 2) / mInterval.y + 0.5f);
        return new Vector2(x, y);
    }

    public int UVToIndex(Vector2 pUV)
    {
        return UVToIndex((int)pUV.x, (int)pUV.y);
    }
    public int UVToIndex(int pX, int pY)
    {
        return pX * Utilities.FLOOR_HEIGHT + pY;
    }

    public Vector3 UVToWorldPos(Vector2 pUV)
    {
        float x = pUV.x * mInterval.x - mSize.x / 2 + transform.position.x;
        float y = pUV.y * mInterval.y - mSize.y / 2 + transform.position.y;
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// 1 = 玩家胜，0 = 平局，-1 = 对手胜
    /// </summary>
    /// <returns></returns>
    public int CheckGameResult(out float pPlayer, out float pEnemy)
    {
        pPlayer = mPlayerPercentage;
        pEnemy = mEnemyPercentage;
        if (mPlayerPercentage > mEnemyPercentage) return 1;
        else if (mPlayerPercentage < mEnemyPercentage) return -1;
        return 0;
    }

    /// <summary>
    /// 检查对应位置当前是否为指定方的喷漆
    /// </summary>
    public bool CheckIsOnSpray(Vector3 pPos, bool pIsPlayer)
    {
        var uv = WorldPosToUV(pPos);
        var index = UVToIndex(uv);
        if (index < 0 || index > mSprays.Length - 1) return false;
        var spray = mSprays[index];
        if (pIsPlayer) return spray > 0;
        else return spray < 0;
    }
    #endregion

    #region Private方法
    private void Spray(int pX, int pY, bool pByPlayer, int pRadius = SPRAY_RADIUS)
    {
        if (pX < 0 || pX > Utilities.FLOOR_WIDTH) return;
        if (pY < 0 || pY > Utilities.FLOOR_HEIGHT) return;

        for (int i = -pRadius + 1; i < pRadius; i++)
        {
            if (pX + i < 0 || pX + i > Utilities.FLOOR_WIDTH) continue;
            // 四个角不染色
            int offset = (Mathf.Abs(i) / 2);
            for (int j = -pRadius + 1 + offset; j < pRadius - offset; j++)
            {
                if (pY + j < 0 || pY + j > Utilities.FLOOR_HEIGHT) continue;
                int id = UVToIndex(pX + i, pY + j);
                if (id > mSprays.Length - 1) continue;
                if (pByPlayer)
                {
                    if (!(mSprays[id] > 0))
                    {
                        mPlayerPercentage += PERCENT_PER_POINT;
                        if (mSprays[id] < 0)
                        {
                            mEnemyPercentage -= PERCENT_PER_POINT;
                        }
                    }
                }
                else
                {
                    if (!(mSprays[id] < 0))
                    {
                        mEnemyPercentage += PERCENT_PER_POINT;
                        if (mSprays[id] > 0)
                        {
                            mPlayerPercentage -= PERCENT_PER_POINT;
                        }
                    }
                }

                mSprays[id] = (pByPlayer ? 1 : -1);
            }
        }
        HUD.Current.UpdateSprayPercentage(mPlayerPercentage, mEnemyPercentage);
        UpdateShaderGlobals();
    }

    private void UpdateShaderGlobals()
    {
        mBuffer.SetData(mSprays);
        Shader.SetGlobalBuffer("sprays", mBuffer);
    }
    #endregion
}
