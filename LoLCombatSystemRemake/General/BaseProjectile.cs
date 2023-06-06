using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public delegate void OnProjectileHit(BaseProjectile projectile, MonoBehaviour target, System.Type type);
public delegate void OnProjectileExpired(BaseProjectile projectile);

public class BaseProjectile : MonoBehaviour
{
    #region Properties
    [Header("Movement Properties")]
    public Vector3 direction;
    public float speed;
    public float lifetime;

    [Header("Gameplay Info")]
    public ChampionBehavior owner;
    public OnProjectileHit d_OnProjectileHit;
    public OnProjectileExpired d_OnProjectileExpired;
    public bool hitEnemyChampions = true;
    public bool hitAllyChampions = false;
    #endregion

    protected virtual void Start()
    {
        ActorManager.Get?.RegisterProjectile(this);
    }
    protected virtual void OnDestroy()
    {
        ActorManager.Get?.DeregisterProjectile(this);
    }

    public void InitializeProjectile(Vector3 pDirection, float pSpeed, float pLifetime, 
        ChampionBehavior pOwner, bool bDestroyWhenExpired)
    {
        direction = pDirection;
        speed = pSpeed;
        lifetime = pLifetime;
        owner = pOwner;
        if (bDestroyWhenExpired)
        {
            d_OnProjectileExpired += delegate (BaseProjectile projectile)
            {
                Destroy(gameObject);
            };
        }
            
    }

    protected virtual void Update()
    {
        // update remaining lifetime and position
        if (lifetime > 0f)
        {
            lifetime -= Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;
        }
        if (lifetime <= 0f)
        {
            d_OnProjectileExpired?.Invoke(this);
        }

        // TODO: check for projectile hits against requested types
        if (hitEnemyChampions || hitAllyChampions)
        {
            foreach (ChampionBehavior champion in ActorManager.Get.champions)
            {
                if ((champion.team == owner.team && hitAllyChampions)
                    || (champion.team != owner.team && hitEnemyChampions))
                {
                    if (Cast2D.CastCircleAgainstCircle(transform.position, 0f,
                        champion.transform.position, champion.hitRadius * Measurements.UNIT_TO_UNITY))
                    {
                        d_OnProjectileHit?.Invoke(this, champion, typeof(ChampionBehavior));
                    }
                }
            }
        }
    }
}
