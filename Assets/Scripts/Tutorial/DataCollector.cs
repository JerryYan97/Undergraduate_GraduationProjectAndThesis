using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public GameObject Helicopter;
    private Transform HeliTran;
    private AVGH AVGHeight;
    private float Distance = 0f;
    private float tempX;
    private float StartY;
    private float tempZ;

    class AVGH
    {
        private float TotalHeight;
        private uint ExeTimes;

        public AVGH()
        {
            TotalHeight = 0f;
        }

        public void CollectHeight(float tempHeight)
        {
            TotalHeight += tempHeight;
            ExeTimes++;
        }

        public float GetAVGHeight()
        {
            return TotalHeight / ExeTimes;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        HeliTran = Helicopter.GetComponent<Transform>();
        AVGHeight = new AVGH();
        tempX = HeliTran.position.x;
        StartY = HeliTran.position.y;
        tempZ = HeliTran.position.z;

        InvokeRepeating("CalculateAVGHeight", 0.5f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the distance;
        float tempA = HeliTran.position.x;
        float tempAbsX = Mathf.Abs(tempA - tempX);
        float tempB = HeliTran.position.z;
        float tempAbsZ = Mathf.Abs(tempB - tempZ);
        float tempPow = Mathf.Pow(tempAbsX, 2.0f) + Mathf.Pow(tempAbsZ, 2.0f);
        float tempSqrt = Mathf.Sqrt(tempPow);
        tempX = tempA;
        tempZ = tempB;
        Distance += tempSqrt;
    }

    private void CalculateAVGHeight()
    {
        AVGHeight.CollectHeight(HeliTran.position.y - StartY);
    }

    public float GetAVGHeight()
    {
        return AVGHeight.GetAVGHeight();
    }

    public float GetDistance()
    {
        return Distance;
    }
}
