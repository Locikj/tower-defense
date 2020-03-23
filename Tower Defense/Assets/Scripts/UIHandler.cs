using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour {

    #region Public Variables

    public static bool isPaused = false;

    public GameObject[] towerOptions = new GameObject[3];

    #endregion

    #region Private Variables
    private Camera mainCam;
    [SerializeField] private Game gameScript;

    //tower purchase/drag and drop variables
    private GameObject selectedTower; 
    private bool beingDragged = false;
    private Transform towerBeingDragged = null;
    private Tower towerScriptDisplayingInfo;
    private bool isPlaceable;
    private Transform tile;

    private bool startMenuActive = true;

    //text variables
    [SerializeField]private Text towerDetails;
    private string defaultText;
    [SerializeField] private Text playerInfoText;
    [SerializeField] private Text errorText;

    //ui panels & buttons
    [SerializeField] private Transform startPanel;
    [SerializeField] private Transform winnerPanel;
    [SerializeField] private Transform loserPanel;
    [SerializeField] private Transform pausePanel;
    [SerializeField] private Transform buyButton;
    [SerializeField] private Transform upgradeButton;
    [SerializeField] private Transform sellButton;
    [SerializeField] private Transform errorPanel;
    [SerializeField] private Transform spawnNextWaveButton;

    //overlays
    [SerializeField] private Transform greenOverlay;
    [SerializeField] private Transform redOverlay;

    //screen res
    private int screenWidth;
    private int screenHeight;

#endregion
    /*******Notes*********
     * allow for dragging and dropping
     * onclick display tower attributes
     * only allow tower to be set in the center of non path elements
     
         */
#region Unity Methods
	// Use this for initialization
	void Start () {

        mainCam = Camera.main;
        //pause the game until start is pressed
        Time.timeScale = 0f;

        defaultText = "Alien Tower Defense \n Good Luck Hero! \n \n Press P to pause" ;
        DisplayDefaultDetails();

        DisplayPlayerInfo();

        DisableButton(buyButton.gameObject); 
        DisableButton(upgradeButton.gameObject);
        DisableButton(sellButton.gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        if (beingDragged == true)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10f;

            Ray ray = mainCam.ScreenPointToRay(mousePos);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) == true)
            {
                if(hit.collider.tag == "Placeable")
                {
                    isPlaceable = true;

                    redOverlay.gameObject.SetActive(false);
                    greenOverlay.gameObject.SetActive(true);

                    var tilePos = hit.transform.position;
                    greenOverlay.position = new Vector3(tilePos.x, tilePos.y, -.35f);
                }
                else if (hit.collider.tag == "Not Placeable")
                {
                    isPlaceable = false;

                    greenOverlay.gameObject.SetActive(false);
                    redOverlay.gameObject.SetActive(true);

                    var tilePos = hit.transform.position;
                    redOverlay.position = new Vector3(tilePos.x, tilePos.y, -.35f);
                }

                tile = hit.transform;
            }
            else
            {
                isPlaceable = false;
            }

            DragTower(selectedTower);
        }

        if (Input.GetKeyDown(KeyCode.P)){
            if (isPaused == false && startMenuActive == false)
            {
                Pause();
            }
            else if (isPaused == true)
            {
                Resume();
            }
        }
	}


    #endregion

    #region PanelActivation

    public void EnableStartPanel()
    {
        startPanel.gameObject.SetActive(true);
        startMenuActive = true;
    }

    public void DisableStartPanel()
    {
        startPanel.gameObject.SetActive(false);
        startMenuActive = false;
    }

    public void EnableWinnerPanel()
    {
        if (winnerPanel != null)
        {
            winnerPanel.gameObject.SetActive(true);
        }
        
    }

    public void DisableWinnerPanel()
    {
        winnerPanel.gameObject.SetActive(false);
    }

    public void EnableLoserPanel()
    {
        loserPanel.gameObject.SetActive(true);
    }

    public void DisableLoserPanel()
    {
        loserPanel.gameObject.SetActive(false);
    }

    public void EnableErrorPanel()
    {
        errorPanel.gameObject.SetActive(true);
        StartCoroutine(WaitAndDisable());
    }

    public void DisableErrorPanel()
    {
        errorPanel.gameObject.SetActive(false);
    }

    public void EnablePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void DisablePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void EnableButton(GameObject button)
    {
        button.SetActive(true);
    }

    public void DisableButton(GameObject button)
    {
        button.SetActive(false);
    }

    public void EnableSpawnButton()
    {
        spawnNextWaveButton.gameObject.SetActive(true);
    }

    public void DisableSpawnButton()
    {
        spawnNextWaveButton.gameObject.SetActive(false);
    }

    #endregion

    #region ButtonMethods
    public void StartGame()
    {
        DisableStartPanel();
        Time.timeScale = 1f;
        EnableSpawnButton();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        EnablePanel(pausePanel.gameObject);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        DisablePanel(pausePanel.gameObject);
        Time.timeScale = 1f;

        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BuyTower()
    {
        StartDrag();
    }

    public void UpgradeTower()
    {
        if(towerScriptDisplayingInfo.GetLevel() < towerScriptDisplayingInfo.GetMaxLevel() && gameScript.ReturnCurrency() >= towerScriptDisplayingInfo.GetUpgradeCost())
        {
            gameScript.UseCurrency(towerScriptDisplayingInfo.GetUpgradeCost());
            towerScriptDisplayingInfo.Upgrade();
            DisplayTowerDetails(towerScriptDisplayingInfo, towerScriptDisplayingInfo.GetDamage(), towerScriptDisplayingInfo.GetRange(), true);
            
        }
        else
        {
            if (!errorPanel.gameObject.activeSelf)
            {
                string text;
                if (towerScriptDisplayingInfo.GetLevel() == towerScriptDisplayingInfo.GetMaxLevel())
                {
                    text = "Tower already at max level";

                }
                else
                {
                    text = "Insufficient funds";
                }

                EnableErrorPanel();
                errorText.text = text;
            }
        }
    }

    public void SelectTowerButton(GameObject tower)
    {
        selectedTower = tower;
        var towerScript = tower.GetComponent<Tower>();
        DisplayTowerPurchaseDetails(towerScript);
    }

    public void SellTower()
    {
        if(towerScriptDisplayingInfo != null)
        {    
            gameScript.AcquireCurrency(towerScriptDisplayingInfo.GetUpgradeCost() / 2);
            Destroy(towerScriptDisplayingInfo.gameObject);

            towerDetails.text = defaultText;

            DisableButton(upgradeButton.gameObject);
            DisableButton(sellButton.gameObject);
        }
    }

    //the following 3 are for screen resolution
    public void SetWidth(int val)
    {
        screenWidth = val;
    }

    public void SetHeight(int val)
    {
        screenHeight = val;
    }

    public void SetScreenRes()
    {
        Screen.SetResolution(screenWidth, screenHeight, false);
    }

    #endregion

    #region Utility Methods

    public bool CheckIfDragging()
    {
        return beingDragged;
    }

    public void DisplayPlayerInfo()
    {
        string newText = "Current Level: " +(SceneManager.GetActiveScene().buildIndex + 1) + "\n Wave: " + (gameScript.GetWave() + 1) + "\n Gold: " + gameScript.ReturnCurrency() + "\n Health: " + gameScript.GetHealth();
        playerInfoText.text = newText;
    }

    //when selecting another tower, turn off the range of the previous one
    public void DisplayTowerDetails(Tower selectedTowerScript, int damage, int range, bool isUpgrade)
    {
        if(towerScriptDisplayingInfo != null && !isUpgrade)
        {
        towerScriptDisplayingInfo.ToggleRange();
        }

        if(selectedTowerScript.GetLevel() < selectedTowerScript.GetMaxLevel())
        {
            var newText = selectedTowerScript.GetTowerName() + "\n Level: " + selectedTowerScript.GetLevel() + "\n Damage: " + damage + "\n Attack Speed: " + selectedTowerScript.GetAttackSpeed() + "\n Range: " + range + "\n Upgrade Cost: " + selectedTowerScript.GetUpgradeCost()
                + "\n Sell Value: " + (selectedTowerScript.GetUpgradeCost() / 2);
            towerDetails.text = newText;
            EnableButton(upgradeButton.gameObject);
        }
        else //tower has reached max level
        {
            var newText = selectedTowerScript.GetTowerName() + "\n Level: " + selectedTowerScript.GetLevel() + "\n Damage: " + damage + "\n Attack Speed: " + selectedTowerScript.GetAttackSpeed() + "\n Range: " + range 
                + "\n Sell Value: " + (selectedTowerScript.GetUpgradeCost() / 2);
            towerDetails.text = newText;
            DisableButton(upgradeButton.gameObject);
        }

        towerScriptDisplayingInfo = selectedTowerScript;
        if (!isUpgrade)
        {
            towerScriptDisplayingInfo.ToggleRange();
        }

        

        
        EnableButton(sellButton.gameObject);
        DisableButton(buyButton.gameObject);//fixes bug 
    }

    public void DisplayTowerPurchaseDetails(Tower towerScript)
    {
        if (towerScriptDisplayingInfo != null)
        {
            towerScriptDisplayingInfo.ToggleRange();
        }

        var newText = towerScript.GetTowerName() + "\n Damage: " + towerScript.GetDamage()+ "\n Attack Speed: " + towerScript.GetAttackSpeed() +"\n Range: " + towerScript.GetRange() + "\n Cost: " + towerScript.GetCost();
        towerDetails.text = newText;

        towerScriptDisplayingInfo = null;

        EnableButton(buyButton.gameObject);
    }

    //displays default details
    public void DisplayDefaultDetails()
    {
        if(towerScriptDisplayingInfo != null)
        {
            towerScriptDisplayingInfo.ToggleRange();
            towerScriptDisplayingInfo = null;
        }

        towerDetails.text = defaultText;

        DisableButton(buyButton.gameObject);
        DisableButton(upgradeButton.gameObject);
        DisableButton(sellButton.gameObject);
    }
    
    public void StartDrag()
    {
        var towerScript = selectedTower.GetComponent<Tower>();
        //find selected tower, check if player has enough to buy it, then start dragging
        if(towerScript.GetCost() <= gameScript.ReturnCurrency())
        {
            beingDragged = true;
            DragTower(selectedTower);
        }
        else
        {
            //notify player that they dont have enough currency
            EnableErrorPanel();
            errorText.text = "Insufficient funds";
        }
        
    }


    private void DragTower(GameObject tower)
    {
        //remove tower if right mouse button is clicked
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(towerBeingDragged.gameObject);
            selectedTower = null;
            beingDragged = false;
            redOverlay.gameObject.SetActive(false);
            greenOverlay.gameObject.SetActive(false);
            DisplayDefaultDetails();
        }
        else if (Input.GetMouseButtonDown(0) == false )
        {            
            if (towerBeingDragged == null)
            {
                //instantiate on button
                towerBeingDragged = Instantiate(tower, transform.position, tower.transform.rotation).GetComponent<Transform>();
                towerBeingDragged.GetComponent<Tower>().enabled = false;
            }
            else
            {
                if(tile != null)
                {
                    towerBeingDragged.position = new Vector3(tile.position.x, tile.position.y, -0.5f);
                }
            }
        }
        else 
        {
            if (isPlaceable == true)
            {
                DropTower();
            }
                
        } 
    }

    private void DropTower()
    {
        //decide if obj can be placed on a tile,place tower centered on current map tile
        beingDragged = false;

        towerBeingDragged.position = new Vector3(tile.position.x, tile.position.y, -0.5f);

        Tower towerScript = towerBeingDragged.GetComponent<Tower>();
       towerScript.enabled = true;
       StartCoroutine(WaitForTowerToInitialize(towerBeingDragged));
        gameScript.UseCurrency(towerScript.GetCost());
       
        towerBeingDragged = null;
        greenOverlay.gameObject.SetActive(false);
        DisplayDefaultDetails();
    }

    #endregion

    #region
    IEnumerator WaitForTowerToInitialize(Transform tower)
    {
        Tower towerScript = tower.GetComponent<Tower>();
        while(towerScript.CheckIfInitialized() == false)
        {
            yield return null;
        }

        towerScript.HideRange();
    }

    IEnumerator WaitAndDisable()
    {
        yield return new WaitForSeconds(3);
        DisableErrorPanel();
    }
#endregion
}
