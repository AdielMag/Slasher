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

    SpawnInformation[] spawnInfo = new SpawnInformation[150];

    int maxSpawnedObj;
    public int currentSpawnedObj = 0;

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
        if (currentSpawnedObj < maxSpawnedObj && finishedThinking)
            SpawnObject(spawnInfo[currentSpawnedObj]);
    }

    bool finishedThinking;
    void SpawnBrain()
    {
        finishedThinking = false;

        // Loop for each one of the objective that going to spawn for the next (maxSpawnObj).
        // (You will be called again after that amount will be spawned)

        // Variables to store into the information.
        Vector3 position = new Vector3(0, 0, 50);
        SpawnInformation.ObjectType type = SpawnInformation.ObjectType.Regular; // Will need to be changed with type logic...
        for (int i =0; i < maxSpawnedObj; i++)
        {
            // Will need logic to check for type...

            if (type == SpawnInformation.ObjectType.Regular)
            {
                int segemtnLength = Random.Range(1, 5);
                if (i + segemtnLength > maxSpawnedObj - 1)
                    segemtnLength = maxSpawnedObj - i;

                bool randomize = Random.Range(0, 11) > 7 ? true : false;

                // Y position needs to be changed between types!!
                position = randomize ? position : new Vector3(Random.Range(1, 4) * 2 - 5, 1.25f, position.z); 

                for (int tempI = 0; tempI < segemtnLength; tempI++)
                {
                    if (randomize)
                    {
                        position = new Vector3(Random.Range(1, 4) * 2 - 5, position.y, position.z);
                    }
                    position += Vector3.forward * 4;

                    spawnInfo[i + tempI] = new SpawnInformation();
                    spawnInfo[i + tempI].objectType = type;
                    spawnInfo[i + tempI].position = position;
                }
                i += segemtnLength - 1;
            }
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
        }
        currentSpawnedObj++;
    }

    // Spawn regular objects with x diffrence of 2 and z diffrence of 4.
    // Reset game when lost.
}

class SpawnInformation
{
    public Vector3 position;
    public enum ObjectType { Regular }
    public ObjectType objectType;
}
