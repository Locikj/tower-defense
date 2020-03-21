using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    #region Public Variables
    public int speed;

    #endregion

    #region Private Variables
    private int damage;
    private Transform enemy;
    #endregion

    #region Unity Methods
	// Update is called once per frame
	void Update () {
        MoveTowardsEnemy();
	}

    private void OnCollisionEnter(Collision collision)
    {
        //check if enemy, if so attack them, check for collision here as well to later 
        //add more stuff, such as piercing projectiles which can hit multiple enemies
        Destroy(gameObject);
    }
    #endregion

    #region Utility Methods

    public void SetEnemy(Transform target)
    {
        enemy = target;
    }

    public void SetDamage(int dam)
    {
        damage = dam;
    }

    public int GetDamage()
    {
        return damage;
    }
    //make this work better, projectiles should continue on same trajectory as when enemy was alive
    private void MoveTowardsEnemy()
    {
        if (enemy != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, enemy.position, (speed * Time.deltaTime));
        }
        else
            Destroy(gameObject);
    }

#endregion

}
