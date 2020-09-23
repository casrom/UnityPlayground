using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInitializer : MonoBehaviour
{

    EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        Color wallColor = new Color(
          Random.Range(0f, 1f),
          Random.Range(0f, 1f),
          Random.Range(0f, 1f)
      );
        //transform.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", wallColor);

        //spawn enemy at room center
       //enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        //GameObject enemy = enemyManager.enemyPool.DePool();
        //enemy.transform.position = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
