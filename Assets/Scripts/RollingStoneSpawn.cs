using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStoneSpawn : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject RollingStonePrefab;
    
    public GameObject Spawn()
    {
        return Instantiate(RollingStonePrefab, SpawnPoint.position, SpawnPoint.rotation);
    }
}
