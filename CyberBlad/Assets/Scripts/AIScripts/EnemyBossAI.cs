/* 
    ==============================================
    
    This script was written by Kevin

    Additioanl information:
        A temporary script fot the "Main boss"
        will most likely scrap or re-write this
        script in the future.

    ==============================================
*/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossAI : MonoBehaviour {
    [Header(header: "Variables list")]
    public Transform transformToFollow; //Transform that NPC has to follow
    NavMeshAgent agent; //NavMesh Agent variable

    [Header(header: "Movement related variables")]
    private float minSpeed = 5f;
    private float maxSpeed = 10f;
    public float movementSpeed; // Stores and changes current movement speed
    public float oldMovementSpeed; // Stores starting movement speed

    [Header(header: "Distance related variables")]
    private float enemyStoppingDistance = 10.0f;
    private float triggerDistance = 15f; // Distance to trigger "sprint" mode

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>(); //Get the component of the NavMeshAgent
        agent.speed = Random.Range(minInclusive: minSpeed, maxInclusive: maxSpeed);
        movementSpeed = agent.speed;
    }
 
    // Update is called once per frame
    void Update() {
        float distance = Vector3.Distance(a: transform.position, b: agent.transform.position); // Sets the distance between the agent and its transform to follow
        agent.destination = transformToFollow.position; // sets the Agent's destination to 5f from the transform to follow

        if(distance < enemyStoppingDistance) {
           Vector3.Distance(a: transform.position, b: transformToFollow.transform.position); 
        }

        if(distance > triggerDistance) {
            movementSpeed = agent.speed * 3;
        } else if (distance < triggerDistance) {
            movementSpeed = agent.speed;
        }

        agent.stoppingDistance = enemyStoppingDistance;
    }
}
