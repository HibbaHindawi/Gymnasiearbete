/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAI : MonoBehaviour {
    // Stuck handeling 
    bool isRunningStuckCheck = false;
    bool isFirstTemporaryWaypoint = false;
    int stuckCheckCounter = 0;
    List<Vector3> temporaryWaypoints = new List<Vector3>();

    // Referances
    AStarLite aStarLite;

    // Start is called before the first frame update
    void Start() {
        aStarLite = GetComponent<AStarLite>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (topDown) {
            if (getVelocetyMagnitude() < 0.5f && Mathf.Abs(inputVector.z) > 0.01f && !isRunningStuckCheck) {
                StartCorutine(StuckCheck());
            }
        }
    }

    IEnumerator StuckCheck() {
        Vector3 initialStuckPosition = transform.position;

        isRunningStuckCheck = true;

        yield return new WaitForSeconds(0.7f);

        // if we havent moved for a second than we are stuck
        if ((transform.position - initialStuckPosition).sqrMagnitude < 3) {
            temporaryWaypoints = aStarLite.FindOurPath(currentWaypoint.transform.position);

            // if no path found make empty list
            if (temporaryWaypoints == null) 
                temporaryWaypoints = new List<Vector3>();

            stuckCheckCounter++;

            isFirstTemporaryWaypoint == true;

        } else stuckCheckCounter = 0;

        isRunningStuckCheck == false;
    }
}
*/