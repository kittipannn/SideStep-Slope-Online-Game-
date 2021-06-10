using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Spawner : NetworkBehaviour
{
    public GameObject[] Obstacleprefab;
    public GameObject[] ObstaleSpawnPoint;

    public override void NetworkStart()
    {
        //spawnObstacleServerRpc();
        InvokeRepeating("spawnObstacleServerRpc", 0, 1.5f);
    }


    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    void spawnObstacleServerRpc() 
    {
        //Point To Spawn the Obstacle
        int obstacleSpawnIndex = Random.Range(0, 2); // 3 point
        Transform spawnPoint = ObstaleSpawnPoint[obstacleSpawnIndex].transform;

        //Spawn the Obstacle
        int obstacleIndex = Random.Range(0, 2); // 3 obj
        
        NetworkObject Obstacle = Instantiate(Obstacleprefab[obstacleIndex], spawnPoint.position, Random.rotation).GetComponent<NetworkObject>();
        
        Obstacle.Spawn();
    }
}
