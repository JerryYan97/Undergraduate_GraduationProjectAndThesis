using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUIControl : MonoBehaviour
{
    public GameObject PopOutWindow;
    public GameObject EndCanvas;
    public TutorialNetController SocketIO;
    // Start is called before the first frame update
    void Start()
    {
        PopOutWindow.SetActive(false);
        EndCanvas.SetActive(false);
    }

    public void OnFinishedTurtuial()
    {
        EndCanvas.SetActive(true);
    }

    #region Btn Actions
    public void OnGearBtnClick()
    {
        PopOutWindow.SetActive(true);
    }
    
    public void OnQuitBtnClick()
    {
        SocketIO.UploadRecordData();
        SceneManager.LoadScene("Player GUI");
    }

    public void OnResumeBtnClick()
    {
        if (PopOutWindow.activeSelf)
        {
            PopOutWindow.SetActive(false);
        }
        else
        {
            EndCanvas.SetActive(false);
        }
    }
    #endregion
}
