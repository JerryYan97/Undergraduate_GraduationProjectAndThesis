using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//As long as you use the PostEffectsBase, u need to attach the script to a camera;
[ExecuteInEditMode]
public class mWaterCubemap : MonoBehaviour
{
    //public GameObject WaterPlane;
    public float clipPlaneOffset = 2.0f;
    public int textureSize = 256;
    public LayerMask reflectLayers = -1;

    private RenderTexture m_ReflectionTexture;
    private int m_OldReflectionTextureSize;

    //Brilient! Use the Camera as both key and value; The key is the currently rendering camera; And the value is the corresponding reflection camera;
    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //OnWillRenderObject is called for each camera if the object is visible and not a UI element.
    public void OnWillRenderObject()
    {
        //remember that this script is attatched on the water panel.
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;
        Camera reflectionCamera;

        Camera cam = Camera.current;
        
        if (!cam)
        {
            return;
        }
        Debug.Log("this is:" + cam.name + " " + "the ID of it is:" + cam.GetInstanceID());

        CreateWaterObjects(cam, out reflectionCamera);//Get or create the current cam's reflectionCamera;

        UpdateCameraModes(cam, reflectionCamera);//Nothing special, just copy some sets of it.

        // Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        //initialize the pos of the camera under the water
        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        Vector3 oldpos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint(oldpos);//new pos is the reflection pos;
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;//?
        //reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

        // Set custom culling matrix from the current camera -- transform world to projection;
        reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

        reflectionCamera.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
        reflectionCamera.targetTexture = m_ReflectionTexture;
        bool oldCulling = GL.invertCulling;
        GL.invertCulling = !oldCulling;

        //+++++
        reflectionCamera.transform.position = oldpos;
        //+++++

        reflectionCamera.Render();
        //reflectionCamera.transform.position = oldpos;
        GL.invertCulling = oldCulling;
        GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
    }

    // Given position/normal of the plane, calculates plane in camera space.
    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }

    void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera)
    {
        reflectionCamera = null;
        // Reflection render texture
        if (!m_ReflectionTexture || m_OldReflectionTextureSize != textureSize)
        {
            if (m_ReflectionTexture)
            {
                //When the texture size changed;
                DestroyImmediate(m_ReflectionTexture);
            }
            m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16);
            m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();//The ID of the water plane?
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = textureSize;//Pay attention to this; we do not save the oldtexture, but we save the size of it.
        }

        // Camera for reflection
        m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);//At first the reflectionCamera is absolutely null, but then this function would output a reflection cam;
        if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        {//if there is not a matched reflectioncamera.
            GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
            //15690 is the ID of the water panel;
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            //go.hideFlags = HideFlags.HideAndDontSave;
            go.hideFlags = HideFlags.DontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;
        }
    }

    void OnDisable()
    {
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }
       
        foreach (var kvp in m_ReflectionCameras)
        {
            DestroyImmediate((kvp.Value).gameObject);
        }
        m_ReflectionCameras.Clear();
        
    }

    void UpdateCameraModes(Camera src, Camera dest)// cam, reflection
    {
        if (dest == null)
        {
            return;
        }
        // set water camera to clear the same way as current camera
        //dest.clearFlags = src.clearFlags;
        dest.clearFlags = CameraClearFlags.Skybox;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent<Skybox>();
            Skybox mysky = dest.GetComponent<Skybox>();
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }
}
