using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager instance;

    [SerializeField] private GameObject DebugWindow;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one DebugManager in the scene.");
        }

        instance = this;
    }

    private void Start()
    {
        DebugWindow.SetActive(false);
    }

    public static DebugManager GetInstance()
    {
        return instance;
    }


}
