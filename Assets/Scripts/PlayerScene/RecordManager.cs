using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordManager : MonoBehaviour
{
    public Text LoginTimes;
    public Text PlayingTime;
    public Text AVGDistance;
    public Text AVGHeight;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetRecord(string logintimes, string playingtime, string avgdis, string avgheight)
    {
        LoginTimes.text = logintimes;
        PlayingTime.text = playingtime + " s";
        AVGDistance.text = avgdis;
        AVGHeight.text = avgheight;
    }
}
