using UnityEngine;

public class DrawCamera : MonoBehaviour
{
    private Camera drawCamera;

    private void Start()
    {
        drawCamera = GetComponent<Camera>();

        drawCamera.clearFlags = CameraClearFlags.Nothing;
    }
}
