using System.Collections;
using UnityEngine;

/// <summary>
/// Force the use of a second display and set custom resolution
/// </summary>
[RequireComponent(typeof(Camera))]
public class ActivateDisplay : MonoBehaviour
{
    public bool UseSingleDisplay = false;
    public int Width = 1280;
    public int Height = 720;

    void Start()
    {
        Screen.SetResolution(Width, Height, true);

        if (!UseSingleDisplay)
        {
            // activate second display (projector) if available
            if (Display.displays.Length > 1)
            {
                Display.displays[1].Activate();
                foreach (Camera cam in FindObjectsOfType<Camera>())
                {
                    cam.targetDisplay = 1;
                }
                GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().targetDisplay = 1;
            }
            //StartCoroutine(WaitForApp());
        }
    }

    IEnumerator WaitForApp()
    {
        yield return new WaitForSeconds(1f);
        Application.Instance.AppendMessage("Displays connected: " + Display.displays.Length);
        yield return null;
    }
}
