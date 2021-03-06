using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
/// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
        ColourList();
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.colourId);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerAnimation(Player _player, bool _isMoving)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerAnimation))
        {
            _packet.Write(_player.id);
            _packet.Write(_isMoving);

            SendTCPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerDied(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDied))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
        _player.gameObject.layer = 6;
    }

    public static void PlayerAppliedColour(int _fromClient, float _colourId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerAppliedColour))
        {
            _packet.Write(_colourId);
            _packet.Write(_fromClient);
            SendTCPDataToAll(_packet);
        }
        ColourList();
    }

    public static void ColourList()
    {
        int _boolsLength = ServerManager.instance.isTaken.Length;
        using (Packet _packet = new Packet((int)ServerPackets.colourList))
        {
            _packet.Write(_boolsLength);
            for (int i = 0; i < _boolsLength; i++)
            {
                _packet.Write(ServerManager.instance.isTaken[i]);
            }
            SendTCPDataToAll(_packet);
        }

        Debug.Log("Sending new Color List.");
        for (int i = 0; i < _boolsLength; i++)
        {
            Debug.Log(ServerManager.instance.isTaken[i]);
        }
    }
    
    public static void MeetingStarted(int _meetingHost)
    {
        using (Packet _packet = new Packet((int)ServerPackets.meetingStarted))
        {
            _packet.Write(_meetingHost);
            SendTCPDataToAll(_packet);
        }
    }

    public static void MeetingEnded()
    {
        KeyValuePair<string, int> max = new KeyValuePair<string, int>("Placeholder", -1);
        foreach (KeyValuePair<string, int> pair in ServerManager.instance.votingResults)
        {
            if (pair.Value > max.Value)
            {
                max = pair;
            }
        }

        foreach (KeyValuePair<string, int> pair in ServerManager.instance.votingResults)
        {
            if (pair != max)
            {
                if (pair.Value == max.Value)
                {
                    max.Key = "Skip"
                }
            }
        }
        Debug.Log($"Voting Result is: {max.Key}");

        Debug.Log("All players have voted, ending meeting.");
        using (Packet _packet = new Packet((int)ServerPackets.meetingEnded))
        {
            _packet.Write(max.Key);
            SendTCPDataToAll(_packet);
        }

        ServerManager.instance.voteCount = 0;

        ServerManager.instance.votingResults.Clear();
        foreach (KeyValuePair<string, int> pair in ServerManager.instance.baseVotingResults)
        {
            ServerManager.instance.votingResults.Add(pair.Key, pair.Value);
        }

        Debug.Log("Printing current voting results");
        foreach (KeyValuePair<string, int> pair in ServerManager.instance.votingResults)
        {
            Debug.Log(pair.Key + " : " + pair.Value);
        }
    }

    public static void PlayerRoles(int _id, string _role)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRoles))
        {
            _packet.Write(_id);
            _packet.Write(_role);
            SendTCPDataToAll(_packet);
        }
    }
}

    #endregion  
