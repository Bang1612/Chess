using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class Misc : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public static SaveData SavedGameState { get; private set; }

    void Awake() {
    
    }

    public void SaveGameState() {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.SaveCreate(); // Clone the current state
        string json = JsonUtility.ToJson(SavedGameState);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("File saved");
    }
    

    public void LoadGameState() {
        string path = GetSavePath();
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SavedGameState = JsonUtility.FromJson<SaveData>(json);
        }
    }

    private string GetSavePath() {
        return Path.Combine(Application.persistentDataPath, "saved_game.json");
    }
    public void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale =0;
    }
    public void Home(){
        
        SaveGameState();
        Time.timeScale =1;
        // SceneManager.LoadScene("MenuScene");
        SceneManager.LoadSceneAsync(0);
        
;   }
    public void Restart(){
        string savePath = Path.Combine(Application.persistentDataPath, "saved_game.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        Time.timeScale =1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
    public void Resume(){ 
        pauseMenu.SetActive(false);
        Time.timeScale =1;
    }
}

[System.Serializable]
public class SaveData
{
    public bool aiIsBlack;
    public string currentPlayer;
    public bool gameOver;
    
    public AI.Difficulty aiDifficulty;
    
    // public int[,] positions= new int[8,8];
    public int[,] positions = new int[8,8];
    
    
    
}
