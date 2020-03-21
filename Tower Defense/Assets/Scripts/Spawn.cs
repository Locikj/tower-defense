using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    #region Public Variables
    public Transform EnemyPath;

    public GameObject[] Wave0;
    public GameObject[] Wave1;
    public GameObject[] Wave2;
    public GameObject[] Wave3;
    public GameObject[] Wave4;
    #endregion

    #region Private Variables
    private int currWave;
    private int finalWave;
    private List<GameObject[]> Waves;


    private Game gameScript;
    #endregion

    #region Unity Methods

    // Use this for initialization
    void Start()
    {
        gameScript = Camera.main.GetComponent<Game>();
        currWave = -1;

        Waves = new List<GameObject[]>();
        Waves.Add(Wave0);
        Waves.Add(Wave1);
        Waves.Add(Wave2);
        Waves.Add(Wave3);
        Waves.Add(Wave4);

        finalWave = Waves.Count;

        gameScript.AddSpawnToList(transform.GetComponent<Spawn>());
    }

    #endregion

    #region Utility Methods

    public int GetCurrentWave()
    {
        return currWave;
    }

    public bool CheckForNextWave()
    {
        if (currWave + 1 < finalWave)
        {
            return true;
        }
        else
            return false;
    }

    public void StartNextWave()
    {
        currWave += 1;
        
        if (currWave < finalWave)
        {
            StartCoroutine(SpawnWave(currWave));
        }
        
    }

    private IEnumerator SpawnWave(int waveNum)
    {
        gameScript.SetEnemiesLeft(Waves[waveNum].Length);

        var currWaveArray = Waves[waveNum];
        for (int i = 0; i < currWaveArray.Length; i++)
        {
            if(currWaveArray[i] != null)
            {
                GameObject enemy = Instantiate(currWaveArray[i], transform.position, transform.rotation);
                enemy.GetComponent<Enemy>().SetPath(EnemyPath);
                //timer so enemies dont all spawn at once
                yield return new WaitForSeconds(1);
            }
            else
            {
                gameScript.ReduceEnemiesLeft(); //removes the null object from enemy count
                yield return new WaitForSeconds(2); //delays next enemy longer than noraml
            }            
        }
    }

    #endregion

}
