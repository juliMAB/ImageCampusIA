using System;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public event Action<Mine> OnEmpty;
    public float resourceAmount;

    public float TakeResource(EntityStats entityStats)
    {
        float amountToTake=0;
        if (resourceAmount > 0)
        {
            amountToTake = entityStats.minigSpeed;

            if (resourceAmount- entityStats.minigSpeed <= 0)
            {
                amountToTake = resourceAmount - entityStats.minigSpeed;
                DestroyResource();
            }
            resourceAmount -= entityStats.minigSpeed;
        }
        else
        {
            DestroyResource();
        }
        return amountToTake;
    }

    private void DestroyResource()
    {
        OnEmpty?.Invoke(this);
    }
}
