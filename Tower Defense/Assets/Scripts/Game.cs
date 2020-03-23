using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    #region Public Variables
    public bool PlayerIsTakingDamage;
    #endregion

    #region Private Variables

    private int currency;
    private float playerHealth;

    private UIHandler uiHandler;
    private List<Spawn> spawns;

    private int enemiesLeft;

    private int currScene;
    #endregion

    #region Unity Methods

    // Use this for initialization
    void Start () {
        currency = 100;
        playerHealth = 5;
        uiHandler = GameObject.Find("UI").GetComponent<UIHandler>();

        spawns = new List<Spawn>();
        currScene = SceneManager.GetActiveScene().buildIndex;
    }
    
    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonDown(0) == true)
        {
            if (uiHandler.CheckIfDragging() == false)
            {

                var mousePos = Input.mousePosition;
                mousePos.z = 10f;

                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Tower"))
                    {
                        
                        var towerScript = hit.transform.GetComponent<Tower>();
                        if (!towerScript.GetRangeScript().GetisActive())
                        {
                            uiHandler.DisplayTowerDetails(towerScript,        towerScript.GetDamage(), towerScript.GetRange(), false);  
                        }
                        else
                        {
                            uiHandler.DisplayDefaultDetails();
                        }
                    }
                    else
                    {
                        uiHandler.DisplayDefaultDetails();
                    }

                }
                
            }


        }
    }

    #endregion

    #region Utility Methods

    public void AddSpawnToList(Spawn thisSpawn)
    {
        spawns.Add(thisSpawn);
    }

    public float GetHealth()
    {
        return playerHealth;
    }

    public int ReturnCurrency()
    {
        return currency;
    }

    public int GetWave() 
    {
        return spawns[0].GetCurrentWave();
    }

    public void UseCurrency(int currUsed)
    {
        currency -= currUsed;
        uiHandler.DisplayPlayerInfo();        
    }

    public void AcquireCurrency(int gains)
    {
        currency += gains;
        uiHandler.DisplayPlayerInfo();
    }

    public void TakeDamage(float damage)
    {
        PlayerIsTakingDamage = true;
        playerHealth -= damage;
        uiHandler.DisplayPlayerInfo();

        if (playerHealth <= 0)
        {
            playerHealth = 0;
            uiHandler.DisplayPlayerInfo();
            YouLose();        
        }

        PlayerIsTakingDamage = false;   
    }

    public void SetEnemiesLeft(int enemies)
    {
        enemiesLeft += enemies;
    }

    public void ReduceEnemiesLeft()
    {
        enemiesLeft--;
    }

    public void CheckForWin()
    {
        //destroyed enemies check for this
        enemiesLeft -= 1;

        if (enemiesLeft == 0 && playerHealth >= 1)
        {
            if (spawns[0].CheckForNextWave())
            {
                uiHandler.EnableSpawnButton();
                uiHandler.DisplayPlayerInfo();
            }
            else if (SceneManager.sceneCount > currScene)
            {
                SceneManager.LoadScene(currScene + 1);
            }
            else
            {
                YouWin();
            }
            
        }
            
    }

    public void StartNextWave()
    {
        foreach(Spawn spawn in spawns)
        {
            spawn.StartNextWave();
        }
        uiHandler.DisplayPlayerInfo();
    }

    private void YouWin()
    {
        uiHandler.EnableWinnerPanel();
    }

    private void YouLose()
    {
        uiHandler.EnableLoserPanel();        
    }

#endregion
    IEnumerator WaitWhilePlayerIsTakingDamage()
    {
        yield return new WaitWhile(() => PlayerIsTakingDamage == true);
    }

}
