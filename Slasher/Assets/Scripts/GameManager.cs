using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singelton
    public static GameManager instance;
    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = 120;
    }
    #endregion

    public bool playing;

    [HideInInspector]
    public bool isTouching = true;

    SpawnInformation[] spawnInfo = new SpawnInformation[500];

    public int maxSpawnedObj, spawnCounter = 0;
    [HideInInspector]
    public int maxCurrnetObj = 50, currentSpawnedObj;

     float timeLeft;
    float newRunTime = 1;

    public Slider timeLeftIndicator;

    public Animator touchIndiactor;
    ObjectPooler objPooler;
    LoadingScreen loadScr;

    private void Start()
    {
        objPooler = ObjectPooler.instance;
        objPooler.StartLevel();

        loadScr = LoadingScreen.instance;

        maxSpawnedObj = spawnInfo.Length;
        SpawnBrain();
    }

    private void FixedUpdate()
    {
#if !UNITY_EDITOR
        isTouching = Input.touchCount > 0 ? true : false;

        Time.timeScale = Mathf.Lerp(Time.timeScale, isTouching ? 1 : !playing ? 1 : .2f, Time.fixedDeltaTime * 6);

        touchIndiactor.SetBool("On", !playing ? false : isTouching ? false : true);
#endif
        if (!playing)
            return;

        timeLeft -= .001f;

        timeLeftIndicator.value = timeLeft / newRunTime;

        if (timeLeft <= 0)
            PlayerController.instance.LostGame();

        if (!finishedThinking)
            return;

        if (spawnCounter < maxSpawnedObj && currentSpawnedObj < maxCurrnetObj)
           SpawnObject(spawnInfo[spawnCounter]);
    }

    public void StartPlayingCoroutine()
    {
        StartCoroutine(StartPlaying());
    }

    IEnumerator StartPlaying()
    {
        loadScr.Enter();

        yield return new WaitForSeconds (.3f);

        loadScr.Done();

        // Disable all objects.
        Objective[] objs = FindObjectsOfType<Objective>();
        foreach (Objective obj in objs)
        {
            obj.gameObject.SetActive(false);
        }

        spawnCounter = 0;

        StartCoroutine(OffsetStartPlaying());
        PlayerController.instance.anim.SetFloat("Forward", 1);

        ResetPoints();

        PlayerController.instance.ResetPlayer();

        inGameScoreIndicator.gameObject.SetActive(true);
        timeLeftIndicator.gameObject.SetActive(true);

        timeLeft = newRunTime;
    }

    IEnumerator OffsetStartPlaying()
    {
        yield return new WaitForSeconds(.25f);
        playing = true;
    }

    public Animator lostMenuAnimatorController;
    public void LostGame()
    {
        playing = false;
        lostMenuAnimatorController.SetTrigger("Open");

        ComboCounter.instance.LostCombo();

        ResetSpawnBrain();
        SpawnBrain();
    }

    // Spawn stuff.

    bool finishedThinking;
    public int difficulty;
    float initialValue = 1.02f, initialValuePower;
    float shieldSpawnMinPrecentage = 100;
    float shooterSpawnMinPrecentage = 100;
    float undestructableMinPrecentage = 1;
    Vector3 spawnPosition = new Vector3(0, 1.25f, 50); // Its outside the method because it needs to keep the record of all the past spawns.
    void SpawnBrain()
    {
        finishedThinking = false;

        // Calculate Difficulty by the time - excess time.


        // Loop for each one of the objective that going to spawned (until you reach maxSpawnedObj...).
        // (You will be called again after that amount will be spawned) maybe? probably...

        for (int i =0; i < maxSpawnedObj; i++)
        {
            // Will need logic to check for type...
            //Logic for Type.
            #region Shield
            if (Random.Range(0f, 101) > shieldSpawnMinPrecentage)
            {
                spawnPosition = new Vector3(Random.Range(1, 5) * 2 - 5, 1.25f, spawnPosition.z);

                AddInfoToSpawnArray(SpawnInformation.ObjectType.Shield, i);

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(0, 2) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 4, 60);
                spawnPosition += Vector3.forward * verticalDiff;

                shieldSpawnMinPrecentage = 100;
            }
            #endregion

            #region Shooter
            if (Random.Range(0f, 101) > shooterSpawnMinPrecentage)
            {
                spawnPosition = new Vector3(Random.Range(1, 5) * 2 - 5, 1.25f, spawnPosition.z);

                AddInfoToSpawnArray(SpawnInformation.ObjectType.Shooter, i);

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(0, 2) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 4, 60);
                spawnPosition += Vector3.forward * verticalDiff;

                shieldSpawnMinPrecentage *= 1.02f;
            }
            #endregion

            #region Regular
            else
            {
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

                    // Define Type:
                    SpawnInformation.ObjectType type;
                    if(!randomize && tempI > 2 && tempI == segemtnLength -1)
                    {
                        type = SpawnInformation.ObjectType.BonusPoints;
                        undestructableMinPrecentage *= 1.01f;
                    }

                    else if (Random.Range(0f, difficulty * undestructableMinPrecentage) > 2 + difficulty)
                    {
                        type = SpawnInformation.ObjectType.Undestructable;
                        undestructableMinPrecentage *= 0.94f;
                    }
                    else
                    {
                        type = SpawnInformation.ObjectType.Regular;
                        undestructableMinPrecentage *= 1.01f;
                    }

                    AddInfoToSpawnArray(type, i + tempI);
                }

                int verticalDiff = 4 + Mathf.RoundToInt(Random.Range(1 - difficulty / 2f, 4 - difficulty /1.3f ) * 4);
                verticalDiff = Mathf.Clamp(verticalDiff, 4, 60);
                spawnPosition += Vector3.forward * verticalDiff;

                i += segemtnLength - 1;

                shieldSpawnMinPrecentage *= .999f;
                shooterSpawnMinPrecentage *= .99f;
            }
            #endregion 
        }

        finishedThinking = true;
    }

    void ResetSpawnBrain()
    {
        difficulty = 0;
        initialValue = 1.02f;
        initialValuePower = 0;
        shieldSpawnMinPrecentage = 100;
        shooterSpawnMinPrecentage = 100;
        undestructableMinPrecentage = 1;
        spawnPosition = new Vector3(0, 1.25f, 50);
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
            case SpawnInformation.ObjectType.Undestructable:
                objPooler.SpawnFromPool("Undestructable", info.position, Quaternion.identity);
                break;
            case SpawnInformation.ObjectType.Shooter:
                objPooler.SpawnFromPool("Shooter", info.position, Quaternion.identity);
                break;
            case SpawnInformation.ObjectType.BonusPoints:
                objPooler.SpawnFromPool("Bonus Points", info.position, Quaternion.identity);
                break;
            case SpawnInformation.ObjectType.Shield:
                objPooler.SpawnFromPool("Shield", info.position, Quaternion.identity);
                break;
        }
        currentSpawnedObj++;
        spawnCounter++;
    }

    public void AddTime(float amount)
    {
        timeLeft = timeLeft + amount > newRunTime ? newRunTime : timeLeft + amount;
    }

    #region Points 
    [Header("Points Variables")]
    public Text inGameScoreIndicator;

    public int points;

    public void AddPoints(int amount)
    {
        points += amount;

        inGameScoreIndicator.text = points.ToString();
    }

    public void ResetPoints()
    {
        points = points = 0;
    }
    #endregion
}

public class SpawnInformation
{
    public Vector3 position;
    public enum ObjectType { Regular, Shield, Undestructable,BonusPoints,Shooter }
    public ObjectType objectType;
}
