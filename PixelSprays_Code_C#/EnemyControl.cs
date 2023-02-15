using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϊ״̬��
/// </summary>
public class EnemyControl : MonoBehaviour
{

    #region ����
    private const float MOVE_SPEED = 10f;
    #endregion

    #region ����
    // �ƶ�
    /// <summary>��ɫ�ƶ����򣨲����׼����</summary>
    private Vector3 mForward = Vector3.left;
    public Vector3 Forward
    {
        get { return mForward; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }

    // ��������
    /// <summary>��ǰ������ײ�����壬û��ʱ����Ϊnull</summary>
    public GameObject mCollideObj;
    private Transform mGun;
    private SpriteRenderer mRenderer;

    // ����״̬��
    private Dictionary<Type, EnemyStateBase> mStates = new Dictionary<Type, EnemyStateBase>();
    private EnemyStateBase mCurrState = null;
    private readonly Type DEFAULT_STATE_TYPE = typeof(EnemyPatrolState);

    private Damagable mDamagable;
    private bool mIsHit = false;
    #endregion


    private void Awake()
    {
        mDamagable = GetComponent<Damagable>();
        mGun = transform.Find("Gun");
        mRenderer = GetComponent<SpriteRenderer>();
        InitStates();
    }

    private void Start()
    {
        HUD.Current?.RegisterEnemy(transform);
    }

    private void OnDestroy()
    {
        HUD.Current?.RemoveEnemy(transform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Position, Position + mForward * 20);
    }
#endif

    private void FixedUpdate()
    {
        if (!GameManager.IsPlaying) return;

        if (mDamagable.OnCooldown && !mIsHit)
        {
            GoToState(typeof(EnemyHitState));
            mIsHit = true;
        }
        mCurrState?.OnUpdate(Time.fixedDeltaTime);

        // ����ǹ�ĳ���
        var gunLookat = PlayerControl.Current.Position - transform.position;
        mGun.position = transform.position + gunLookat.normalized * mGun.localScale.y;
        mGun.rotation = Quaternion.LookRotation(Vector3.forward, gunLookat);

        if (Mathf.Sign(gunLookat.x) == Mathf.Sign(transform.localScale.x))
        {
            mGun.localScale = new Vector3(-.8f, .8f, 1);
        }
        else
        {
            mGun.localScale = new Vector3(.8f, .8f, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mCollideObj == null) mCollideObj = collision.gameObject;

        mCurrState?.OnCollide(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (mCollideObj == collision.gameObject) mCollideObj = null;
    }

    #region Public����
    public void InitStates()
    {
        RegisterState(new EnemyPatrolState());
        RegisterState(new EnemyChaseState());
        RegisterState(new EnemyAttackState());
        RegisterState(new EnemyHitState());

        GoToState(typeof(EnemyPatrolState));
    }

    public void RegisterState(EnemyStateBase pState)
    {
        mStates.Add(pState.GetType(), pState);
    }

    public void GoToState(Type pStateType)
    {
        mCurrState?.OnLeaveState(this);
        mCurrState = pStateType == null ? mStates[DEFAULT_STATE_TYPE] : mStates[pStateType];
        mCurrState?.EnterState(this);
        mIsHit = false;
    }

    /// <summary>
    /// �ƶ���ɫ
    /// </summary>
    /// <param name="pMove">�ƶ�����;���</param>
    public void Move(Vector3 pMove)
    {
        if (FloorManager.Current.CheckIsOnSpray(transform.position, true))
        {
            pMove /= Utilities.SPRAY_MOVE_BOOST;
        }
        mForward = pMove;
        transform.position += pMove;

        UpdateSpriteFacing();
    }
    #endregion

    #region Private����
    private void UpdateSpriteFacing()
    {
        if (mForward.x > 0) // �����ƶ�
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("EnemyFront");
            transform.localScale = new Vector3(-2, 2, 1);
        }
        else // �����ƶ�
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("EnemyFront");
            mRenderer.flipX = false;
            transform.localScale = new Vector3(2, 2, 1);
        }
    }
    #endregion
}
