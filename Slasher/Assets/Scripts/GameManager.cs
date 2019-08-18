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

    SpawnInformation[] spawnInfo = new SpawnInformation[100];

    int maxSpawnedObj, spawnedObjectCounter = 0;

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
    }


    bool finishedThinking;
    public int difficulty;
    float initialValue = 1.02f, initialValuePower;
    float shieldSpawnMinPrecentage = 100;
    Vector3 spawnPosition = new Vector3(0, 1.25f, 50); // Its outside the method because it needs to keep the record of all the past spawns.
    void SpawnBrain()
    {
        finishedThinking = false;

        // Calculate Difficulty by the time - excess time.


        // Loop for each one of the objective that going to spawned (until you reach maxSpawnedObj...).
        // (You will be called again after that amount will be spawned) maybe? probably...

        SpawnInformation.ObjectType type;
        for (int i =0; i < maxSpawnedObj; i++)
        {
            // Will need logic to check for type...
            //Logic for Type.
            type = Random.Range(0f, 101) > shieldSpawnMinPrecentage ? SpawnInformation.ObjectType.Shield : SpawnInformation.ObjectType.Regular;
            #region Regular
            if (type == SpawnInformation.ObjectType.Regular)
            {
                //int maxSegementLength = Random.Range(1, 1+ Mathf.RoundToInt(difficulty / 2f)); // Used by difficulty.
                //maxSegementLength = Mathf.Clamp(maxSegementLength, 1, 7);
                int segemtnLength = Random.Range(1, 7);
                if (i + segemtnLength > maxSpawnedObj - 1)
                    segemtnLength = maxSpawnedObj - i;

                bool randomize = Random.Range(0, 11) > 7 ? true : false;

                // Y position needs to be changed between types!!
                spawnPosition = randomize ? spawnPosition : new Vector3(Random.Range(1, 5) * 2 - 5, 1.25f, spawnPosition.z);

                for (int tempI = 0; tempI < segemtnLength; tempI++)
                {
                    if (randomize)
                    {
                        spawnPosition = new Vector3(Random.Range(1, 5) * 2 - 5, spawnPosition.y, spawnPosition.z);

                        int verticalDiffrence = 4 + Mathf.RoundToInt(Random.Range(0, 2 - difficulty / 14) * 4);
                        verticalDiffrence = Mathf.Clamp(verticalDiffrence, 0, 8);
                        spawnPosition += Vector3.forward * verticalDiffrence;
                    }
                    else
                    {
                        spawnPosition += Vector3.forward * 4;
                    }
                    AddInfoToSpawnArray(type, i + tempI);
                }

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(1 - difficulty / 2f, 4 - difficulty /1.3f ) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 0, 60);
                spawnPosition += Vector3.forward * verticalDiff;

                i += segemtnLength - 1;

                shieldSpawnMinPrecentage *= .999f;
            }
            #endregion

            #region Shield
            else if (type == SpawnInformation.ObjectType.Shield)
            {
                spawnPosition = new Vector3(Random.Range(1, 5) * 2 - 5, 1.25f, spawnPosition.z);

                AddInfoToSpawnArray(type, i);

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(0, 2) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 0, 60);
                spawnPosition += Vector3.forward * verticalDiff;
            }
            #endregion
        }

        finishedThinking = true;
    }

    private void AddInfoToSpawnArray(SpawnInformation.ObjectType type, int i)
    {
        spawnInfo[i] = new SpawnInformation();
        spawnInfo[i].objectType = type;
        spawnInfo[i].position = spawnPosition;

        difficulty = Mathf.RoundToInt(Mathf.Pow(initialValue, initialValuePower++));
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
