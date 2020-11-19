using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;
    public bool[] isTaken = new bool[10];
    public Dictionary<string, int> baseVotingResults = new Dictionary<string, int>()
    {
        {"Skip", 0},
    };
    public Dictionary<string, int> votingResults = new Dictionary<string, int>()
    {
        {"Skip", 0},
    };

    public int voteCount;

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

        for (int i = 0; i < isTaken.Length; i++)
        {
           instance.isTaken[i] = false;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
