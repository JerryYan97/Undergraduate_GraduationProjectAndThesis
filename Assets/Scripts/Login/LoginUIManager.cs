using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginUIManager : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject MainUI;
    public GameObject RegisterUI;
    public LoginNetController netcontrol;
    public PlayerDataSaver datasaver;

    public Text username;
    public Text authresult;
    public InputField password;
    private string CacheUsername;
    // Update is called once per frame
    void Start()
    {
        MainUI.SetActive(true);
        RegisterUI.SetActive(false);
    }
    void Update()
    {
        
    }
    #region UI间切换
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
    #endregion


    public void GetAuthResult(bool res)
    {
        if (res)
        {
            authresult.text = "You have successed login";
            CacheUsername = username.text;
            datasaver.InputPlayerName(CacheUsername);
            SceneManager.LoadScene("Player GUI");
            //MainToPlayer();
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

}
