using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    #region Public Variables
    public Transform pathToFollow;
    public int killReward;
    [SerializeField] Game gameScript;

    public float speed = 1;
    #endregion

    #region Private Variables
    private Transform[] PathPoints;
    private int CurrPoint;
    private HealthBar myHealthBar;
    [SerializeField] private float healthPoints;
    
    #endregion

    #region Unity Methods

    // Use this for initialization
    void Start () {

        gameScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Game>();
        PathPoints = new Transform[pathToFollow.childCount];
        for(int i = 0; i < pathToFollow.childCount; i++) 
        {
            PathPoints[i] = pathToFollow.GetChild(i);
        }

        CurrPoint = 0;
        myHealthBar = transform.Find("HealthBar").GetComponent<HealthBar>();
        myHealthBar.SetMaxHealth(healthPoints);
	}
	
	// Update is called once per frame
	void Update () {
		
        if(transform.position == PathPoints[CurrPoint].position)
        {
            CurrPoint++;
        }

        if(CurrPoint >= PathPoints.Length)
        {
            gameScript.PlayerIsTakingDamage = true;
            gameScript.TakeDamage(1);
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, PathPoints[CurrPoint].position, (speed * Time.deltaTime));
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        Projectile projScript = collision.gameObject.GetComponent<Projectile>();
        if(projScript != null)
        {
            myHealthBar.TakeDamage(projScript.GetDamage());
        }

    }

    private void OnDestroy()
    {
        gameScript.CheckForWin();
    }

    #endregion

    #region Utility Methods

    public void DropLoot()
    {
        gameScript.AcquireCurrency(killReward);
    }

    public void SetPath(Transform path)
    {
        pathToFollow = path;
    }

    #endregion

}
