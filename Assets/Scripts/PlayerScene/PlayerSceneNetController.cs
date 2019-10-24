using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

public class PlayerSceneNetController : MonoBehaviour
{
    SocketIOComponent m_socket;
    public RecordManager recordmanager;
    public PlaySceneManager UI;
    private GameObject UserData;
    private int mSceneNum;
    // Start is called before the first frame update
    void Start()
    {
        m_socket = GetComponent<SocketIOComponent>();
        UserData = GameObject.Find("PlayerData");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region 获取用户record接口
    public void GetUserRecord()
    {
        //string temp = cameracontrolscript.GetUserName();
        
        string tempName = UserData.GetComponent<PlayerDataSaver>().GetPlayerName();
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["Username"] = tempName;
        m_socket.Emit("GetRecord", new JSONObject(data), OnGetUserRecord);
    }

    public void OnGetUserRecord(JSONObject json)
    {
        Dictionary<string, string> data = json[0].ToDictionary();
        recordmanager.GetRecord(data["logintimes"], data["playingtime"], data["avgdis"], data["avgheight"]);
    }
    #endregion

    #region 创建record
    public void CreateUserRecord(int sceneNo)
    {
        mSceneNum = sceneNo;
        string tempName = UserData.GetComponent<PlayerDataSaver>().GetPlayerName();
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["Username"] = tempName;
        Debug.Log("Emit Create User Record Event");
        m_socket.Emit("CreateRecord", new JSONObject(data), OnCreatedRecord);
    }

    public void OnCreatedRecord(JSONObject json)
    {
        Dictionary<string, string> data = json[0].ToDictionary();
        Debug.Log("Get the feedback record id:" + data["_id"]);
        UserData.GetComponent<PlayerDataSaver>().InputRecordID(data["_id"]);
        if(mSceneNum == 1)
        {
            UI.ToScene1();
        }
        else if(mSceneNum == 2)
        {
            UI.ToScene2();
        }
    }
    #endregion

}
