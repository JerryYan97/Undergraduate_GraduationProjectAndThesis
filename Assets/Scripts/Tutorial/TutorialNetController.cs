using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class TutorialNetController : MonoBehaviour
{
    SocketIOComponent m_socket;
    public DataCollector datacollector;
    private GameObject UserData;
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

    public void UploadRecordData()
    {
        float tempHeight = datacollector.GetAVGHeight();
        float tempDistance = datacollector.GetDistance();
        string tempID = UserData.GetComponent<PlayerDataSaver>().GetRecordID();
        Debug.Log("upload record id:" + tempID);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["AVGHeight"] = tempHeight.ToString();
        data["distance"] = tempDistance.ToString();
        data["recordid"] = tempID;
        m_socket.Emit("UploadRecord", new JSONObject(data));
    }


}
