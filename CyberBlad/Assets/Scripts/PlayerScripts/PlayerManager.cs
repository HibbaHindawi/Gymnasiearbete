using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//skriven av Hibba och med hjälp av Johannes
public class PlayerManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int _healthpoints;
    public TMP_Text text;
    public Playerhealth health;
    public GameObject Gameover;
    void Start(){
        _healthpoints = maxHealth;
        health.SetMaxHealth(maxHealth);
        text = GameObject.Find("Health").GetComponent<TMP_Text>();
        text.text = "Health: 100";
        
        
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            TakeHit();
        }    
    }

    public bool TakeHit() {
        _healthpoints -= 5;
        health.SetHealth(_healthpoints);
        text.text = "Health: " + _healthpoints;
        bool isDead = _healthpoints <= 0;
        if (isDead) _Die();
        return isDead;
    }

    private void _Die() {
        Camera.main.transform.parent = null;
        
        Gameover.SetActive(true);
    }
}
