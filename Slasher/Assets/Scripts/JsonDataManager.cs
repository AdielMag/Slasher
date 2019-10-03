using System.IO;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    #region Singelton
    public static JsonDataManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    string storeJsonPath = Application.streamingAssetsPath + "/StoreData.json";
    string gamePlayJsonPath = Application.streamingAssetsPath + "/GamePlayData.json";

    public StoreData storeData;
    public GamePlayData gamePlayData;



    public void LoadData()
    {
        storeData = JsonUtility.FromJson<StoreData>(File.ReadAllText(storeJsonPath));
        gamePlayData = JsonUtility.FromJson<GamePlayData>(File.ReadAllText(gamePlayJsonPath));
    }

    public void SaveData()
    {
        File.WriteAllText(storeJsonPath, JsonUtility.ToJson(storeData));
        File.WriteAllText(gamePlayJsonPath, JsonUtility.ToJson(gamePlayData));
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
