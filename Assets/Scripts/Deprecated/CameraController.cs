using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private GameObject MainCamera;
    private string CacheUsername;
    //public GameObject RegisterCamera;
    public GameObject MainUI;
    public GameObject RegisterUI;
    public GameObject RecordUI;
    public GameObject PlayerUI;
    public NetworkController netcontrol;
    public Text username;
    public Text authresult;
    public InputField password;

    // Start is called before the first frame update
    void Start()
    {
       MainCamera = GameObject.Find("Main Camera");
        //MainUI = GameObject.Find("Main Panel");
        //RegisterUI = GameObject.Find("Register Panel");

        MainUI.SetActive(true);
        RegisterUI.SetActive(false);
        RecordUI.SetActive(false);
        PlayerUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetAuthResult(bool res)
    {
        if (res)
        {
            authresult.text = "You have successed login";
            CacheUsername = username.text;
            MainToPlayer();
        }
        else
        {
            authresult.text = "You have input a false username or password or both";
        }
    }

    public void OnLogin_Click()
    {
        Debug.Log("The user name is:" + username.text);
        Debug.Log("The password is:" + password.text);
        netcontrol.PasswordAuth(username.text, password.text);
    }

    public void GetCreateResult(bool res)
    {
        if (res)
        {
            authresult.text = "You have successed create one user";
        }
        else
        {
            authresult.text = "Your username has been used";
        }
    }

    public string GetUserName()
    {
        return CacheUsername;
    }

    #region Between UI

    public void GoRegister_Click()
    {
        RegisterUI.SetActive(true);
        MainUI.SetActive(false);
    }

    public void GoMain_Click()
    {
        MainUI.SetActive(true);

        RegisterUI.SetActive(false);
    }

    public void PlayerToRecord()
    {
        PlayerUI.SetActive(false);
        RecordUI.SetActive(true);
    }

    public void RecordToPlayer()
    {
        RecordUI.SetActive(false);
        PlayerUI.SetActive(true);
    }

    private void MainToPlayer()
    {
        MainUI.SetActive(false);
        PlayerUI.SetActive(true);
    }
    #endregion

    public void DoSomething(int a)
    {
        Debug.Log("Hello this is Cross Script Function:" + a);
    }
}
