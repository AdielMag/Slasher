using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singelton
    public static GameManager instace;
    private void Awake()
    {
        instace = this;

    }
    #endregion

    SpawnInformation[] spawnInfo = new SpawnInformation[25];

    int maxSpawnedObj, spawnedObjectCounter = 0;

    float excessRunTime;

    ObjectPooler objPooler;

    private void Start()
    {
        objPooler = ObjectPooler.instance;
        objPooler.StartLevel();

        maxSpawnedObj = spawnInfo.Length;
        SpawnBrain();
    }

    private void FixedUpdate()
    {
        if (spawnedObjectCounter < maxSpawnedObj && finishedThinking)
            SpawnObject(spawnInfo[spawnedObjectCounter]);
        else
        {

        }


        difficulty = 1 + Mathf.RoundToInt (0.07f * (Time.time - excessRunTime) * 2f);
    }

    bool finishedThinking;
    public int difficulty;
    // Its outside the method because it needs to keep the record of all the past spawns.
    Vector3 spawnPosition = new Vector3(0, 1.25f, 50); 
    void SpawnBrain()
    {
        finishedThinking = false;

        // Calculate Difficulty by the time - excess time.


        // Loop for each one of the objective that going to spawn for the next (maxSpawnObj).
        // (You will be called again after that amount will be spawned)
        
        SpawnInformation.ObjectType type = SpawnInformation.ObjectType.Regular; // Will need to be changed with type logic...
        for (int i =0; i < maxSpawnedObj; i++)
        {
            // Will need logic to check for type...
            //Logic for Type.
            type = Random.Range(0f, 101) > 96 ? SpawnInformation.ObjectType.Shield : SpawnInformation.ObjectType.Regular;
            #region Regular
            if (type == SpawnInformation.ObjectType.Regular)
            {
                int maxSegementLength = Random.Range(1, difficulty / 3); // Used by difficulty.
                maxSegementLength = Mathf.Clamp(maxSegementLength, 1, 7);
                int segemtnLength = Random.Range(1, maxSegementLength);
                if (i + segemtnLength > maxSpawnedObj - 1)
                    segemtnLength = maxSpawnedObj - i;

                bool randomize = Random.Range(0, 11) > 7 ? true : false;

                // Y position needs to be changed between types!!
                spawnPosition = randomize ? spawnPosition : new Vector3(Random.Range(1, 4) * 2 - 5, 1.25f, spawnPosition.z);

                for (int tempI = 0; tempI < segemtnLength; tempI++)
                {
                    if (randomize)
                    {
                        spawnPosition = new Vector3(Random.Range(1, 4) * 2 - 5, spawnPosition.y, spawnPosition.z);

                        int verticalDiffrence = 4 + Mathf.RoundToInt(Random.Range(0, 2 - difficulty / 14) * 4);
                        verticalDiffrence = Mathf.Clamp(verticalDiffrence, 0, 8);
                        spawnPosition += Vector3.forward * verticalDiffrence;
                    }
                    else
                    {
                        spawnPosition += Vector3.forward * 4;
                    }
                    spawnInfo[i + tempI] = new SpawnInformation();
                    spawnInfo[i + tempI].objectType = type;
                    spawnInfo[i + tempI].position = spawnPosition;
                }

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(0 + 3 - difficulty / 5, 12 - difficulty / 5) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 0, 60);
                spawnPosition += Vector3.forward * verticalDiff;

                i += segemtnLength - 1;
            }
            #endregion

            #region Shield
            else if (type == SpawnInformation.ObjectType.Shield)
            {
                spawnPosition = new Vector3(Random.Range(1, 4) * 2 - 5, 1.25f, spawnPosition.z);

                spawnInfo[i] = new SpawnInformation();
                spawnInfo[i].objectType = type;
                spawnInfo[i].position = spawnPosition;

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(0 + 3 - difficulty / 5, 12 - difficulty / 5) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 0, 60);
                spawnPosition += Vector3.forward * verticalDiff;
            }
            #endregion
        }

        finishedThinking = true;
    }

    void SpawnObject(SpawnInformation info) 
    {
        switch(info.objectType)
        {
            case SpawnInformation.ObjectType.Regular:
                objPooler.SpawnFromPool("Regular", info.position, Quaternion.identity);
                break;
            case SpawnInformation.ObjectType.Shield:
                objPooler.SpawnFromPool("Shield", info.position, Quaternion.identity);
                break;
        }
        spawnedObjectCounter++;
    }
}

class SpawnInformation
{
    public Vector3 position;
    public enum ObjectType { Regular,Shield }
    public ObjectType objectType;
}
