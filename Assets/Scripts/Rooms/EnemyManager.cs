using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObjectPool enemyPool;
    public GameObject enemy;

    public RoomsManager roomsManager;

    void Start(){
        enemyPool = new GameObjectPool();

        for(int i = 0; i < 20; i++) {
            enemyPool.AddToPool(Instantiate(enemy));
        }


    }

    bool spawnReturnEnemy = false;
    GameObject returnEnemy;
    // Update is called once per frame
    void Update()
    {
        if (roomsManager.returnMode && !spawnReturnEnemy) {
            returnEnemy = enemyPool.DePool();
            spawnReturnEnemy = true;
            returnEnemy.transform.position = roomsManager.activeRoom.transform.position;
        }


        if (!roomsManager.returnMode && spawnReturnEnemy) {
            enemyPool.Pool(returnEnemy);
            spawnReturnEnemy = false;
        }
    }
}
