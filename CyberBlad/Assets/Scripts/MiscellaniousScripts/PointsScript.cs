using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsScript : MonoBehaviour
{
    public PlayerManager player;
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("Points").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Points: " + player.points;
    }
}
