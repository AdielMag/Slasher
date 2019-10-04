using System.IO;
using System.Collections;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

public class JsonDataManager : MonoBehaviour
{
    public static JsonDataManager instance;
    private void Awake()
    {
        instance = this;

        storeDataPath = Path.Combine(Application.persistentDataPath, "StoreData.json");
        gameplayDataPath = Path.Combine(Application.persistentDataPath, "GameplayData.json");
    }

    string storeDataPath, gameplayDataPath;

    public StoreData storeData;
    public GamePlayData gamePlayData;

    public void LoadData()
    {
        if (File.Exists(storeDataPath))
        {
            storeData = JsonUtility.FromJson<StoreData>(File.ReadAllText(storeDataPath));
        }
        else
        {
            Debug.Log("Store Data Dosent Exist");
        }
        if (File.Exists(gameplayDataPath))
        {
            gamePlayData = JsonUtility.FromJson<GamePlayData>(File.ReadAllText(gameplayDataPath));
        }
        else
        {
            Debug.Log("Store Data Dosent Exist");
        }

    }

    public void SaveData()
    {
        File.WriteAllText(storeDataPath, JsonUtility.ToJson(storeData));
        File.WriteAllText(gameplayDataPath, JsonUtility.ToJson(gamePlayData));
    }

}

[System.Serializable]
public class StoreData
{
    public int Coins;
    public int[] CharactersBought;
    public int EquippedCharacter;

    public int[] WeaponsBought;
    public int EquippedWeapon;
}

[System.Serializable]
public class GamePlayData
{
    public int[] HighestScores;
    public int TotalBallSliced;
    public int TotalGamePlayed;
}
