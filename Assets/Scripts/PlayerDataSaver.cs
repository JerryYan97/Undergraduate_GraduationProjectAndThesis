using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSaver : MonoBehaviour
{
    private string name;
    private string recordid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void InputPlayerName(string username)
    {
        name = username;
    }

    public void InputRecordID(string tempRID)
    {
        Debug.Log("InputRecordID:" + tempRID);
        recordid = tempRID;
    }

    public string GetRecordID()
    {
        Debug.Log("Get Record ID:" + recordid);
        return recordid;
    }

    public string GetPlayerName()
    {
        return name;
    }
}
