using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollisionHandler : MonoBehaviour {

    #region Private Variables
    private Tower myTower;
    private bool isActive;
    #endregion

    #region Unity Methods


    // Use this for initialization
    void Start () {
        myTower = transform.GetComponentInParent<Tower>();
	}


    //add enemy to a list held by the Tower class
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<Enemy>() != null)
        {
            myTower.AddEnemyToList(collision.transform);
        }
    }

    //Remove an enemy from a list held by the Tower class
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.GetComponent<Enemy>() != null)
        {
            myTower.RemoveEnemyFromList(collision.transform);
        }
        else if(collision.transform.GetComponent<Projectile>() != null)
        {
            Destroy(collision.gameObject);
        }
    }

    public void SetisActive(bool value)
    {
        isActive = value;
    }

    public bool GetisActive()
    {
        return isActive;
    }

    #endregion
}
