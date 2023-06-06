using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utility;

public enum WeaponType
{
    Melee,
    Ranged
}

public enum TeamTag
{
    Blue,
    Red
}

public delegate void OnPressKey();
public delegate void OnReleaseKey();
public class KeyDelegates
{
    public OnPressKey d_OnPressKey;
    public OnReleaseKey d_OnReleaseKey;
};

[RequireComponent(typeof(CommonStatistics), typeof(NavMeshAgent))]
public class ChampionBehavior : MonoBehaviour
{
    #region Common Properties
    [Header("Common Properties")]
    public bool isPlayer;
    public WeaponType weaponType;
    public TeamTag team;
    public CommonStatistics commonStats;
    public Effects effects;
    #endregion

    #region Bounds
    [Space, Header("Bounds")]
    public float navRadius;
    public float hitRadius;
    #endregion

    #region Key Inputs
    private List<KeyCode> monitorKeys;
    private Dictionary<KeyCode, KeyDelegates> keyDelegates;
    #endregion

    public NavMeshAgent agent;

    protected virtual void Awake()
    {
        monitorKeys = new List<KeyCode>();
        keyDelegates = new Dictionary<KeyCode, KeyDelegates>();
    }

    protected virtual void Start()
    {
        ActorManager.Get?.RegisterChampion(this);

        commonStats = GetComponent<CommonStatistics>();
        effects = GetComponent<Effects>();
        agent = GetComponent<NavMeshAgent>();
        agent.radius = navRadius * Measurements.UNIT_TO_UNITY;
    }

    public void RegisterKeyDelegate(KeyCode key, OnPressKey press, OnReleaseKey release)
    {
        if (!monitorKeys.Contains(key))
        {
            monitorKeys.Add(key);
            keyDelegates[key] = new KeyDelegates();
        }
        keyDelegates[key].d_OnPressKey += press;
        keyDelegates[key].d_OnReleaseKey += release;
    }
    public void UnregisterKeyDelegate(KeyCode key, OnPressKey press, OnReleaseKey release)
    {
        if (!monitorKeys.Contains(key))
            return;
        keyDelegates[key].d_OnPressKey -= press;
        keyDelegates[key].d_OnReleaseKey -= release;
    }

    protected virtual void Update()
    {
        if (!isPlayer)
            return;

        // handle key inputs
        if (effects.canCast)
        {
            foreach (KeyCode key in monitorKeys)
            {
                if (!keyDelegates.ContainsKey(key))
                    continue;
                if (Input.GetKeyDown(key))
                {
                    keyDelegates[key].d_OnPressKey?.Invoke();
                }
                else if (Input.GetKeyUp(key))
                {
                    keyDelegates[key].d_OnReleaseKey?.Invoke();
                }
            }
        }

        // handle movement
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Camera.main)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
                {
                    if (!hit.collider.gameObject.CompareTag("Obstacle"))
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
    }
}
