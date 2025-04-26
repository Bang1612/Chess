using UnityEngine;
using Unity.Netcode;

public class NetworkManagerPersist : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Keep alive across scenes
    }
}