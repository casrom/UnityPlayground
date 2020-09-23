using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyTest : MonoBehaviour
{

    NavMeshAgent agent;

    EnemyManager manager;

    RoomsManager roomsManager;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        roomsManager = GameObject.FindGameObjectWithTag("RoomsManager").GetComponent<RoomsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnNavMesh)
            agent.destination = Camera.main.transform.position;
        if (Vector3.Distance(transform.position, Camera.main.transform.position) > 50) {
            transform.position = roomsManager.activeRoom.GetRandomNeighbor().transform.position + Vector3.up;
        }

    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log(collider);
        if (collider.gameObject.name == "Player") {
            SceneManager.LoadScene("Rooms");
        }
    }
}
