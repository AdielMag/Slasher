using System.IO;
using System.Collections;
using System.Collections.Generic;using UnityEngine;
using UnityEngine.Networking;

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

    //string storeDataPath, gamePlayDataPath;

    public StoreData storeData;
    public GamePlayData gamePlayData;
   

    public IEnumerator LoadData()
    {
#if UNITY_ANDROID
        using (UnityWebRequest webRequest = UnityWebRequest.Get(storeJsonPath))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            storeData = JsonUtility.FromJson<StoreData>(webRequest.downloadHandler.text);

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error");
            }
            else
            {
                Debug.Log("Loaded: " + webRequest.downloadHandler.text + "/n" + webRequest.downloadHandler.text);
            }
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(gamePlayJsonPath))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            gamePlayData = JsonUtility.FromJson<GamePlayData>(webRequest.downloadHandler.text);

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error");
            }
            else
            {
                Debug.Log("Loaded: " + webRequest.downloadHandler.text + "/n" + webRequest.downloadHandler.text);
            }
        }

        /*
        UnityWebRequest www = UnityWebRequest.Get(storeJsonPath);
        yield return www.SendWebRequest();
        storeData = JsonUtility.FromJson<StoreData>(www.downloadHandler.text);

        UnityWebRequest www2 = UnityWebRequest.Get(gamePlayJsonPath);
        yield return www2.SendWebRequest();
        gamePlayData = JsonUtility.FromJson<GamePlayData>(www2.downloadHandler.text);
        */
#else
        storeData = JsonUtility.FromJson<StoreData>(File.ReadAllText(storeJsonPath));
        gamePlayData = JsonUtility.FromJson<GamePlayData>(File.ReadAllText(gamePlayJsonPath));
#endif
    }

    public IEnumerator SaveData()
    {
#if UNITY_ANDROID
        using (UnityWebRequest www = UnityWebRequest.Post(storeJsonPath, JsonUtility.ToJson(storeData)))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            //Debug.Log(storeJsonPath);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Save complete!");
            }
        }
        using (UnityWebRequest www = UnityWebRequest.Put(gamePlayJsonPath, JsonUtility.ToJson(gamePlayData)))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Save complete!");
            }
        }
#else
        File.WriteAllText(storeJsonPath, JsonUtility.ToJson(storeData));
        File.WriteAllText(gamePlayJsonPath, JsonUtility.ToJson(gamePlayData));
#endif
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
