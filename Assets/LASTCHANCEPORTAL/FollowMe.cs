using System.Collections;
using PortalOLD;
using UnityEngine;
using UnityEngine.XR;

public class FollowMe : MonoBehaviour {
    private Camera mainCamera;
    public Material portalMaterial;

    private void Awake() {
        mainCamera = Camera.main;
        StartCoroutine(AppliedTexture());
    }

    void Update() {
        Vector3 vector3 = mainCamera.transform.position;
        vector3.x += 2;
        transform.position = vector3;

        transform.rotation = mainCamera.transform.rotation;
    }

    private IEnumerator AppliedTexture() {
        Camera[] cams = GetComponentsInChildren<Camera>();
        
        while (XRSettings.eyeTextureWidth == 0 || XRSettings.eyeTextureHeight == 0) {
            yield return null;
        }

        bool right = false;
        foreach (Camera cam in cams) {
            if (cam.targetTexture) {
                cam.targetTexture.Release();
            }
            cam.fieldOfView = mainCamera.GetComponent<Camera>().fieldOfView;
            cam.targetTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
            
            if (right) {
                portalMaterial.SetTexture("_RightTex", cam.targetTexture);
            } else {
                portalMaterial.SetTexture("_LeftTex", cam.targetTexture);
                right = true;
            }
        }
    }
}
