using UnityEngine;
using System.IO;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;
    public GameManager SavedGameState { get; private set; }

    void Awake() {
        // if (Instance == null) {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        //     LoadGameState();
        // } else {
        //     Destroy(gameObject);
        // }
    }

    public void SaveGameState() {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        SavedGameState = gm.Clone(); // Clone the current state
        string json = JsonUtility.ToJson(SavedGameState);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("File saved");
    }
    

    // public void LoadGameState() {
    //     string path = GetSavePath();
    //     if (File.Exists(path)) {
    //         string json = File.ReadAllText(path);
    //         SavedGameState = JsonUtility.FromJson<GameManager>(json);
    //     }
    // }

    private string GetSavePath() {
        return Path.Combine(Application.persistentDataPath, "saved_game.json");
    }
}
