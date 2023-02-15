using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnly : MonoBehaviour
{
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
