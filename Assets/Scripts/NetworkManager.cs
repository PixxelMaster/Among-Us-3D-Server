using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

    }

    private void Start()
    {
        Physics.IgnoreLayerCollision(0, 6); 
        Physics.IgnoreLayerCollision(3, 6);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 18450);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(-3.75f, 1.1f, 30f), Quaternion.identity).GetComponent<Player>();
    }
}   