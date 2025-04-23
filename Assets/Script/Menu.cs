using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // public  AI.Difficulty aiType ;
    public void PlayGame(){
        SceneManager.LoadSceneAsync(1); 
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void SelectEzMode(){
        AI.aiType = AI.Difficulty.Dumb;
        SceneManager.LoadSceneAsync(1);
    }
    public void SelectNorMode(){
        AI.aiType = AI.Difficulty.Average;
        SceneManager.LoadSceneAsync(1);
    }
    public void SelectHardMode(){
        AI.aiType = AI.Difficulty.Smart;
        SceneManager.LoadSceneAsync(1);
    }
}
