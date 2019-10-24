using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldmWaterCubemap : MonoBehaviour
{
    public GameObject WaterPlane;
    private Material WaterPlaneMat;
    private Cubemap cubemap;
    private GameObject CubeMapCamera;
    // Start is called before the first frame update
    void Start()
    {
        CubeMapCamera = new GameObject("CubeMapCam");
        cubemap = new Cubemap(128, TextureFormat.RGBA32, true);

        CubeMapCamera.AddComponent<Camera>();
        CubeMapCamera.transform.position = WaterPlane.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CubeMapCamera.GetComponent<Camera>().RenderToCubemap(cubemap);
        WaterPlane.GetComponent<MeshRenderer>().material.SetTexture("_CubeMap", cubemap);
    }
}
