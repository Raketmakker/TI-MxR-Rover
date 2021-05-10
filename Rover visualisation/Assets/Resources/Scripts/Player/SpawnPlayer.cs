using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject player;
    public string playerTag = "Player";

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag(this.playerTag) == null)
        {
            Instantiate(player);
        }
    }
}
