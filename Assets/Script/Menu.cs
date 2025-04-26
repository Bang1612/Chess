using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.Netcode;

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
        SaveManager.instance.SaveDelete() ;
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
    public void OnHostClicked()
    {
        // disable AI, start host
        GameManager.aiEnable = false;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        Debug.Log("Server init");
        NetworkManager.Singleton.StartHost();
        
        SceneManager.LoadScene("Game");
    }

    public void OnJoinClicked()
    {
        GameManager.aiEnable = false;
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("Game");
    }

    // Optional: approve all connections
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req,
                               NetworkManager.ConnectionApprovalResponse res)
    {
        res.Approved = true;
        res.CreatePlayerObject = true;
    }
}
