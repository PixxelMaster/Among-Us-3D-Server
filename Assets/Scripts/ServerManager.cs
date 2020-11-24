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

    private int impostorCount = 1;

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

    public void GenerateRoles()
    {
        Debug.Log($"Active player count: {Server.activePlayers.Count}");
        //Initializing a list of players with the possible choices
        List<Client> _impostorChoices = new List<Client>();
        foreach (Client _client in Server.activePlayers)
        {
            _impostorChoices.Add(_client);
        }

        for (int i = 0; i < impostorCount; i++)
        {
            int _impostorId = Random.Range(0, _impostorChoices.Count);
            Client _impostor = _impostorChoices[_impostorId];
            Debug.Log($"Set {_impostor.player.username} as impostor.");
            Server.clients[_impostor.id].player.SetImpostor();

            _impostorChoices.Remove(_impostor);
        }

        foreach(Client _client in Server.activePlayers)
        {
            if (_client.player.isImpostor)
            {
                ServerSend.PlayerRoles(_client.id, "Impostor");
            }
            else
            {
                ServerSend.PlayerRoles(_client.id, "Crewmate");
            }
        }
    }
}
