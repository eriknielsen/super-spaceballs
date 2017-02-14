using UnityEngine;
using System.Collections;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class ServerBehaviour : NetworkManager {
    [Serializable]
    public class SerializableCommandList : List<SerializableCommand> { }
    public class CommandMsg : MessageBase {
        public static short msgType = MsgType.Highest + 1;
        public SerializableCommandList commands;

        public byte[] serializedCommands;
    }
    public class UnpauseMsg : MessageBase {
        public static short msgType = MsgType.Highest + 2;
    }

    public GameObject networkedPlayPrefab;

    public Text statusText;

    NetworkClient localClient;
    NetworkConnection remoteConnection;
    //if isServer then the networkplaybehaviour gets this set
    bool isServer;

    void Start()
    {
        NetworkManager.singleton.StartMatchMaker();


    }
    //call this method to request a match to be created on the server
    public void CreateInternetMatch(string matchName)
    {
        Debug.Log("creating match");
        matchName = "";
        NetworkManager.singleton.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnInternetMatchCreate);
    }

    //this method is called when your request for creating a match is returned
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            statusText.text = "Create match succeeded";
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            localClient = NetworkManager.singleton.StartHost(hostInfo);
            isServer = true;
        }
        else
        {
            statusText.text = "Create match failed";
            Debug.LogError("Create match failed");
        }
    }

    //call this method to find a match through the matchmaker
    public void FindInternetMatch(string matchName)
    {
        NetworkManager.singleton.matchMaker.ListMatches(0, 10, matchName, true, 0, 0, OnInternetMatchList);
    }

    //this method is called when a list of matches is returned
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                Debug.Log("A list of matches was returned");
                statusText.text = "A list of matches was returned, trying to join one";

                if (matches[matches.Count - 1].currentSize < 2)
                    NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                Debug.Log("No matches in requested room!");
                statusText.text = "No matches in requested room!";
            }
        }
        else
        {
            statusText.text = "Couldn't connect to match maker";
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Able to join a match");
            statusText.text = "Able to join a match";
            MatchInfo hostInfo = matchInfo;

            localClient = NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        statusText.text = "client connected!";
        Debug.Log("client connected!");
        remoteConnection = conn;
        //if we are a client connecting to a host then create the networkplayobject
        if (isServer == false)
        {
            NetworkPlayBehaviour npb = Instantiate(networkedPlayPrefab).GetComponent<NetworkPlayBehaviour>();
            npb.customIsServer = isServer;
            npb.server = this;
            conn.RegisterHandler(CommandMsg.msgType, npb.OnRecieveCommands);
            conn.RegisterHandler(UnpauseMsg.msgType, npb.OnRecieveUnpause);
        }
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (localClient != null)
        {
            if (conn != localClient.connection)
            {
                Debug.Log("another player joined! now create local networkplaybehaviour");
                NetworkPlayBehaviour npb = Instantiate(networkedPlayPrefab).GetComponent<NetworkPlayBehaviour>();
                npb.customIsServer = isServer;
                npb.server = this;

                conn.RegisterHandler(MsgType.Highest + 1, npb.OnRecieveCommands);
                remoteConnection = conn;
            }
            else
            {
                Debug.Log("same as localclient connected to server");
            }
        }
    }
    //functions called by the networkplaybehaviour
    public void SendCommands(SerializableCommandList commands)
    {
        CommandMsg msg = new CommandMsg();
        msg.commands = new SerializableCommandList();
        msg.commands = commands;
        Debug.Log("msgcommands is " + msg.commands.Count + " long and the argument was " + commands.Count);

        byte[] result;

        BinaryFormatter bf = new BinaryFormatter();
        System.IO.MemoryStream ms = new System.IO.MemoryStream();


        bf.Serialize(ms, commands);
        result = ms.ToArray();
        CommandMsg cmdMsg = new CommandMsg();

        cmdMsg.serializedCommands = result;

        if (remoteConnection != null)
        {
            remoteConnection.Send(CommandMsg.msgType, cmdMsg);
        }
        else
        {
            Debug.Log("no remote connection to send to");
        }
    }
    public void SendUnpauseGame()
    {
        UnpauseMsg unpMsg = new UnpauseMsg();
        if (isServer && remoteConnection != null)
        {
            remoteConnection.Send(UnpauseMsg.msgType, unpMsg);
        }
    }
}