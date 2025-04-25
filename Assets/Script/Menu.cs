using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Menu : MonoBehaviour
{
    // public  AI.Difficulty aiType ;
    public Button continueButton;
    void Start()
    {
        RefreshButtonState();
    }
    public void SaveDeleted()
    {
        // Delete the save file (example path)
        string savePath = Path.Combine(Application.persistentDataPath, "saved_game.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        // Update the Continue button state
        RefreshButtonState();
    }
    public bool CheckForSaveFile()
    {
        // Use your existing save file path logic (e.g., from SettingsManager)
        
        return SaveManager.instance.CurrentSave == null ? false:true;
    }
    public void RefreshButtonState()
    {
        continueButton.interactable = CheckForSaveFile();
    }

    public void ContinueGame(){
        SaveData temp = SaveManager.instance.CurrentSave;
        AI.aiType = temp.aiDifficulty;
        SceneManager.LoadSceneAsync(1); 
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void SelectEzMode(){
        AI.aiType = AI.Difficulty.Dumb;
        SaveDeleted();
        SceneManager.LoadSceneAsync(1);
    }
    public void SelectNorMode(){
        AI.aiType = AI.Difficulty.Average;
        SaveDeleted();
        SceneManager.LoadSceneAsync(1);
    }
    public void SelectHardMode(){
        AI.aiType = AI.Difficulty.Smart;
        SaveDeleted();
        SceneManager.LoadSceneAsync(1);
    }
    public void EnableAI(){
        GameManager.aiEnable= true;
    }
    
    public void DisableAI(){
        GameManager.aiEnable= false;
    }
}
