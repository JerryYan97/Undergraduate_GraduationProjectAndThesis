using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneManager : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject RecordUI;
    public GameObject PlayerUI;
    public PlayerSceneNetController net;

    // Start is called before the first frame update
    void Start()
    {
        RecordUI.SetActive(false);
        PlayerUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToScene1()
    {
        SceneManager.LoadScene("Guide");
    }

    public void ToScene2()
    {
        SceneManager.LoadScene("SpecialEffect1");
    }
    #region UI间切换
    public void OnScene1Click()
    {
        net.CreateUserRecord(1);
    }

    public void OnScene2Click()
    {
        net.CreateUserRecord(2);
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
    #endregion


}
