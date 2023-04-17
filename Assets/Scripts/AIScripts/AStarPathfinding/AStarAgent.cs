using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour {

    public Transform target;
    float speed = 10;
    Vector3[] path;
    
    int targetIndex;
    int rotationSpeed = 3;
    private Vector3 targetOldPosition;

    void Awake() {
        targetOldPosition = target.position;
    }

    void Update() {
        if (Vector3.Distance(a: targetOldPosition, b: target.position) > 3) {
            PathRequestManager.RequestPath(pathStart: transform.position, pathEnd: target.position, callback: OnPathFound);
            targetOldPosition = target.position;
        }
    }
    
    public void OnPathFound(Vector3[] newPath, bool pathIsSuccessful) {
        if (pathIsSuccessful) {
            path = newPath;
            StopCoroutine(methodName: "FollowPath");
            StartCoroutine(methodName: "FollowPath");
        }
    }

    IEnumerator FollowPath() {

        if(targetIndex >= path.Length){
            targetIndex = 0;
            path = new Vector3[0];
            yield break;
        }

        Vector3 currentWaypoint = path[0];

        while (true) {
            if(transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 targetDir = currentWaypoint - this.transform.position;
            float step = this.rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(current: transform.forward, target: targetDir, maxRadiansDelta: step, maxMagnitudeDelta: 0.0F);
            transform.rotation = Quaternion.LookRotation(forward: newDir);

            transform.position = Vector3.MoveTowards(current: this.transform.position, target: currentWaypoint, maxDistanceDelta: this.speed * Time.deltaTime);           // Moves the transform
            yield return null;                                                                              // Move over to net frame (done bc we are in a Coroutin)

        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(center: path[i], size: Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(from: transform.position, to: path[i]);
                }
                else {
                    Gizmos.DrawLine(from: path[i-1], to: path[i]);
                }
            }
        }
    }

}
