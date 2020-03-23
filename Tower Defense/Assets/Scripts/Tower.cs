using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour {

    #region Public Variables
    //base tower attributes
    public string towerName;
    public int cost;
    public int damage;
    public float attackSpeed;
    public int range;
    
    //upgrade variables
    public int upgradeCost;
    public int increaseUpgradeCost;
    [Range(0,1)]public float damageUpgrade;
    [Range(0, 1)] public float attackSpeedUpgrade;
    [Range(0, 1)] public float rangeUpgrade;
    public float cooldownTime;
    #endregion

    #region Private Variables
    private Camera mainCam;
    [SerializeField] private Transform Projectile;
    
    private int level;
    private int maxLevel = 3;

    private bool inCooldown = false;

    private Transform RangeObj;
    private List<Transform> EnemiesInRange;

    private UIHandler uiHandler;

    private RangeCollisionHandler rangeScript;
    #endregion

    /**********Notes**********
     * possibly remove cooldowntime and replace with attack speed
     
         */

    #region Unity Methods

    // Use this for initialization
    void Start () {
        InitializeTower();
        mainCam = Camera.main;
        EnemiesInRange = new List<Transform>();
        Physics.queriesHitTriggers = true;
        uiHandler = GameObject.Find("UI").GetComponent<UIHandler>();
        rangeScript = RangeObj.GetComponent<RangeCollisionHandler>();
        RangeObj.GetComponent<Collider>().enabled = true; //turned off in initializetower(), fixes bug where tower wont shoot enemies when spawned in range
    }
	
	// Update is called once per frame
	void Update () {

        if (CheckForEnemies() == true && inCooldown == false)
        {
            //shoot the prefered target
            ShootClosestToTower();
            StartCoroutine(Cooldown(cooldownTime));
        }

    }

    
    #endregion

    #region Utility Methods
    public string GetTowerName()
    {
        return towerName;
    }

    public RangeCollisionHandler GetRangeScript()
    {
        if (rangeScript != null)
        {
            return rangeScript;
        }
        else
        {
            return null;
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }

    public int GetUpgradeCost()
    {
        return upgradeCost;
    }
    
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public void Upgrade()
    {
        level += 1;
        upgradeCost += increaseUpgradeCost;
        damage += Mathf.RoundToInt((damage * damageUpgrade));
        attackSpeed += (attackSpeed * attackSpeedUpgrade);
        range += Mathf.RoundToInt((range * rangeUpgrade));
        SetRange();
        
    }

    public int GetCost()
    {
        return cost;
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetRange()
    {
        return range;
    }

    public void ToggleRange()
    {
        if(RangeObj != null)
        {
            if (RangeObj.GetComponent<Renderer>().enabled == true)
            {
                RangeObj.GetComponent<RangeCollisionHandler>().SetisActive(false);
                RangeObj.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                RangeObj.GetComponent<RangeCollisionHandler>().SetisActive(true);
                RangeObj.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    public void ShowRange()
    {
        RangeObj.GetComponent<Renderer>().enabled = true;
    }

    public void HideRange()
    {
        RangeObj.GetComponent<Renderer>().enabled = false;
    }

    public bool CheckIfInitialized()
    {
        if (level == 1)
        {
            return true;
        }
        else
            return false;
    }

    public void AddEnemyToList(Transform enemy)
    {
        if(GetComponent<Tower>().enabled == true)
        {
            EnemiesInRange.Add(enemy);
        }
        
    }

    public void RemoveEnemyFromList(Transform enemy)
    {
        if (GetComponent<Tower>().enabled == true)
        {
            EnemiesInRange.Remove(enemy);
        }
    }


    private void InitializeTower()
    {
        RangeObj = transform.Find("Range");
        RangeObj.GetComponent<Collider>().enabled = false;
        SetRange();

        level = 1;
    }

    private bool CheckForEnemies()
    {
        //if list is NOT empty, shoot a projectile
        if (EnemiesInRange.Count > 0)
        {
            return true;
        }
        else
            return false;
    }

    private void ShootClosestToTower()
    {
        Transform closestEnemy = null;
        float closestEnemyDist = 0;
        foreach(Transform enemy in EnemiesInRange)
        {
            if(enemy != null)
            {
                var tempDist = Vector3.Distance(transform.position, enemy.position);
                if (closestEnemyDist == 0 || closestEnemyDist > tempDist)
                {
                    closestEnemy = enemy;
                    closestEnemyDist = Vector3.Distance(transform.position , enemy.position);
                    var posInList = EnemiesInRange.IndexOf(enemy);
                }
            }

        }
        
        if(closestEnemy != null)
        {
            Vector3 toTarget = closestEnemy.transform.position - transform.position;
            Transform newProjectile = Instantiate(Projectile, transform.position, Quaternion.LookRotation(toTarget));

            var newProjScript = newProjectile.GetComponent<Projectile>();
            newProjScript.SetEnemy(closestEnemy);
            newProjScript.SetDamage(damage);
        }

    }

    private void SetRange()
    {
        RangeObj.localScale = new Vector3(range, .01f, range);
    }
    #endregion



    #region Enums
    private IEnumerator Cooldown(float cooldown)
    {
        inCooldown = true;
        yield return new WaitForSeconds(cooldown/attackSpeed);
        inCooldown = false;
    }

    #endregion

}
