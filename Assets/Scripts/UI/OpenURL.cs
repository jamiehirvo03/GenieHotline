using UnityEngine;

public class OpenURL : MonoBehaviour
{
    [SerializeField] private string targetURL;

    public void OpenLink()
    {
        Application.OpenURL(targetURL);
    }
}
