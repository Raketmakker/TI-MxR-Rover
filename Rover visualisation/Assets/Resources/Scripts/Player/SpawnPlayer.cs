using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject player;
    private static bool spawned = false;

    private void Awake()
    {
        if (!spawned)
        {
            Instantiate(player);
            spawned = true;
        }
    }
}
