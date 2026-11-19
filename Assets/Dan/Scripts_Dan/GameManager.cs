using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ItemTable itemTable;

    private string saveFilePath;
    private GameData gameData;

    private void Awake()
    {
       if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
       else if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }

       saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to " + saveFilePath);
    }

    private void LoadGame()
    {
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded from " + saveFilePath);
        }
        else
        {
           gameData = new GameData();
            SaveGame();
        }

        Debug.Log("Game Loaded");
    }

    public void SetGameData(GameData data)
    {
        gameData = data;
    }

    //public void UpdateGameData(GameData data)
    //{
    //    gameData = data;
    //    SaveGame();
    //}

    public GameData GetGameData()
    {
        return gameData;
    }

    public void SaveXP(int xp)
    {
        PlayerPrefs.SetInt("PLayerXP", xp);
        PlayerPrefs.Save();
    }

    public int LoadXP()
    {
        return PlayerPrefs.GetInt("PlayerXP", 0);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("Death");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class GameData
{
    public float currentLevel;
    public float currentXP;
    public int availableAttributePoints;
    public float healthPoints;
    public float manaPoints;
    public float speedPoints;
}
