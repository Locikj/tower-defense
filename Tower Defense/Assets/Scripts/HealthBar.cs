﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    #region Private Variables
    private Enemy ThisEnemy;
    private Transform Bar;
    private float MaxHealth;
    private float CurrHealth;
    private float HealthPercent = 1f;
#endregion

#region Unity Methods

	// Use this for initialization
	void Start () {
        //pull enemy and health bar info
        ThisEnemy = transform.GetComponentInParent<Enemy>();
        Bar = transform.Find("bar").transform;
        CurrHealth = MaxHealth;
        HealthPercent = CurrHealth / MaxHealth;
	}

#endregion

#region Utility Methods

    public void TakeDamage(int damage)
    {
        CurrHealth -= damage;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        //check for damage and adjust health

        HealthPercent = CurrHealth / MaxHealth;

        if (HealthPercent > 0)
        {
            Bar.localScale = new Vector3(HealthPercent, 1f);
        }
        else
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        Destroy(ThisEnemy.gameObject);
        //reward player
        ThisEnemy.GetComponent<Enemy>().DropLoot();
    }

    //not sure if this is used
    public void SetMaxHealth(float hp)
    {
        MaxHealth = hp;
    }

#endregion

}
