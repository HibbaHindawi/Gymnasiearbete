using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//skriven av Hibba
public class Playerhealth : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    Image img;
    public PlayerManager Health;

    private void Awake() {
        img = GameObject.Find("Image").GetComponent<Image>();
    }
    public void SetMaxHealth(int health){
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth( int health){
        slider.value = health;
    }
}
