using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    #region Singleton Implementation
    private static ActorManager Instance;
    public static ActorManager Get
    {
        get { return Instance; }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    public List<ChampionBehavior> champions;
    public List<BaseProjectile> projectiles;

    public void RegisterChampion(ChampionBehavior target)
    {
        champions.Add(target);
    }

    public void RegisterProjectile(BaseProjectile target)
    {
        projectiles.Add(target);
    }
    public void DeregisterProjectile(BaseProjectile target)
    {
        projectiles.Remove(target);
    }
}
