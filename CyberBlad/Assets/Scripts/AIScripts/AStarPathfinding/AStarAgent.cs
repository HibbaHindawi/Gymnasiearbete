using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour {

    public Transform target;
    float speed = 20;
    Vector3[] path;
    int targetIndex;
    int rotationSpeed = 3;

    void Update() {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathIsSuccessful) {
        if (pathIsSuccessful) {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
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
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position = Vector3.MoveTowards(this.transform.position, currentWaypoint, this.speed * Time.deltaTime);           // Moves the transform
            yield return null;                                                                              // Move over to net frame (done bc we are in a Coroutin)

        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }

}
