using UnityEngine;
using System.Collections;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

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




    //Gameobjects that need to start unenabled


    public NetworkPlayBehaviour networkedPlayInstance;

    public Text statusText;
    MatchInfo currentMatchInfo;
    NetworkClient localClient;
    NetworkConnection remoteConnection;
    [SerializeField]
    GameObject findMatchButton;
     [SerializeField]
    GameObject createMatchButton;
     [SerializeField]
    GameObject backButton;

    int matchDomain = 0;
    //if isServer true then the networkplaybehaviour gets this set
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
        NetworkManager.singleton.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, matchDomain, OnInternetMatchCreate);
        ToggleButtons();

    }
    void ToggleButtons(){
        
        findMatchButton.SetActive(!findMatchButton.activeInHierarchy);
        createMatchButton.SetActive(!createMatchButton.activeInHierarchy);
    }

    //this method is called when your request for creating a match is returned
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Create match succeeded");
            statusText.text = "Match created";
            MatchInfo hostInfo = matchInfo;
            currentMatchInfo = matchInfo;
            
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
        NetworkManager.singleton.matchMaker.ListMatches(0, 10, matchName, true, 0, matchDomain, OnInternetMatchList);
        ToggleButtons();
    }

    //this method is called when a list of matches is returned
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                Debug.Log("A list of matches was returned");
                statusText.text = "Matches found!";

                if (matches[matches.Count - 1].currentSize < 2 &&matches[matches.Count - 1].currentSize > 0){
                    NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, matchDomain, OnJoinInternetMatch);
                }
                else{
                    Debug.Log("no suitable match found, try again");
                }
                    
            }
            else
            {
                Debug.Log("No matches in requested room!");
                statusText.text = "No matches found, create one!";
                ToggleButtons();
            }
        }
        else
        {
            statusText.text = "Couldn't connect to server";
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            
            Debug.Log("Connecting...");
            statusText.text = "Connecting...";
            MatchInfo hostInfo = matchInfo;

            localClient = NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            statusText.text = "Connection failed, try again";
            Debug.LogError("Join match failed");
            ToggleButtons();
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        //statusText.text = "client connected!";
        Debug.Log("client connected!");
        remoteConnection = conn;
        //if we are a client connecting to a host then create the networkplayobject
        if (isServer == false)
        {
            networkedPlayInstance.gameObject.SetActive(true);
            networkedPlayInstance.customIsServer = isServer;
            networkedPlayInstance.server = this;
            conn.RegisterHandler(CommandMsg.msgType, networkedPlayInstance.OnRecieveCommands);
            conn.RegisterHandler(UnpauseMsg.msgType, networkedPlayInstance.OnRecieveUnpause);
        }
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (localClient != null)
        {
            if (conn != localClient.connection)
            {
                Debug.Log("another player joined! now create local networkplaybehaviour");
                networkedPlayInstance.gameObject.SetActive(true);
                networkedPlayInstance.customIsServer = isServer;
                networkedPlayInstance.server = this;

                conn.RegisterHandler(MsgType.Highest + 1, networkedPlayInstance.OnRecieveCommands);
                remoteConnection = conn;
            }
            else
            {
                Debug.Log("same as localclient connected to server");
            }
        }
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
         
        base.OnClientDisconnect(conn);
       
        if(NetworkManager.singleton.matchMaker != null){
        NetworkManager.singleton.matchMaker.DropConnection(currentMatchInfo.networkId, currentMatchInfo.nodeId, matchDomain, null);
        }
        
       
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        NetworkManager.singleton.matchMaker.DropConnection(currentMatchInfo.networkId,currentMatchInfo.nodeId, matchDomain, null);
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    //functions called by the networkplaybehaviour
    public void SendCommands(SerializableCommandList commands)
    {
        CommandMsg msg = new CommandMsg();
        msg.commands = new SerializableCommandList();
        msg.commands = commands;
        //Debug.Log(commands[0].targetPosition.x + " y: " + commands[0].targetPosition.y);

        byte[] result;
        

        BinaryFormatter bf = new BinaryFormatter();
        System.IO.MemoryStream ms = new System.IO.MemoryStream();

        bf.Serialize(ms, commands);
        result = ms.ToArray();
        CommandMsg cmdMsg = new CommandMsg();

        cmdMsg.serializedCommands = result;
        //f(cmdMsg.serializedCommands);
        if (remoteConnection != null)
        {
            remoteConnection.Send(CommandMsg.msgType, cmdMsg);
        }
        else
        {
            Debug.Log("no remote connection to send to");
        }
    }
      public void f(byte[] input)
    {
        List<SerializableCommand> deserializedCommands = new List<SerializableCommand>();
        BinaryFormatter bf = new BinaryFormatter();
        Byte[] buffer = input;
        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
        deserializedCommands = bf.Deserialize(ms) as List<SerializableCommand>;
        //Debug.Log(deserializedCommands[0].targetPosition.x + " y: " + deserializedCommands[0]//.targetPosition.y);
        

        
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