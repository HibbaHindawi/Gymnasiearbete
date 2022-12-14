using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIgnore : MonoBehaviour {
    public Transform Player;

    void Start() {
        Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
