using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
        Server.activePlayers.Add(Server.clients[_fromClient]);
    }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    public static void PlayerKill(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();
        Server.clients[_fromClient].player.Kill(_shootDirection);
    }

    public static void PlayerSelectedColour(int _fromClient, Packet _packet)
    {
        float _colourId = _packet.ReadFloat();
        ServerManager.instance.isTaken[(int)_colourId] = true;
        ServerSend.PlayerAppliedColour(_fromClient, (int)_colourId);
        Server.clients[_fromClient].player.colourId = (int)_colourId;
        Debug.Log($"{_fromClient} has selected colour {_colourId}");
    }

    public static void StartGame(int _fromClient, Packet _packet) 
    {
        
        Debug.Log($"Host: [{Server.clients[_fromClient].player.username}] has started game");
        for (int i = 1; i <= Server.activePlayers.Count; i++)
        {
            Debug.Log($"Moving Player {Server.clients[i].player.username}.");

            Server.clients[i].player.controller.enabled = false;

            Server.clients[i].player.gameObject.transform.position = new Vector3(1.5f, 0.5f, 0f);
            Debug.Log($"Player {Server.clients[i].player.username} is now at {Server.clients[i].player.transform.position}");
            Server.clients[i].player.gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            ServerSend.PlayerPosition(Server.clients[i].player);
            ServerSend.PlayerRotation(Server.clients[i].player);
            Server.clients[i].player.controller.enabled = true;

        }
        Debug.Log("Spawned all players to start location.");
    }

    public static void StartMeeting(int _fromClient, Packet _packet)
    {
        Debug.Log($"Player {Server.clients[_fromClient].player.username} has ordered a meeting.");
        ServerSend.MeetingStarted(_fromClient);
    }

    public static void PlayerVoted(int _fromClient, Packet _packet)
    {
        int _voteTarget = _packet.ReadInt();
        if (_voteTarget != 0)
        {
            if (ServerManager.instance.votingResults.ContainsKey(Server.clients[_voteTarget].player.username))
            {
                ServerManager.instance.votingResults[Server.clients[_voteTarget].player.username] += 1;
            }
            else
            {
                ServerManager.instance.votingResults.Add(Server.clients[_voteTarget].player.username, 1);   
            }
            Debug.Log($"Player {Server.clients[_fromClient].player.username}:{_fromClient} has voted for {Server.clients[_voteTarget].player.username}");
        }
        else
        {
            ServerManager.instance.votingResults["Skip"] += 1;
            Debug.Log($"Player {Server.clients[_fromClient].player.username}:{_fromClient} has skipped voting.");
        }

        Server.clients[_fromClient].player.hasVoted = true;

        Debug.Log("Current Voting Results Update: ");
        foreach (string username in ServerManager.instance.votingResults.Keys)
        {
            Debug.Log ($"{username}: {ServerManager.instance.votingResults[username]}");
        }

        ServerManager.instance.voteCount += 1;
        Debug.Log("Vote count: " + ServerManager.instance.voteCount);

        if (ServerManager.instance.voteCount == Server.activePlayers.Count)
        {
            ServerSend.MeetingEnded();
        }
    }
}