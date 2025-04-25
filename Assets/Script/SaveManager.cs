using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager  instance;
    // This holds your most recent SaveData until the app quits
    public SaveData CurrentSave { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Store a SaveData so that any GameManager in any scene can load it.
    /// </summary>
    public void StoreSave(SaveData data)
    {
        CurrentSave = data;
    }
}
