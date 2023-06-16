using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    protected float value;
    protected override void PicksUp(Player player)
    {
        player.Heal(value);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        value = gameManager.GetHealthValue();
    }
}
