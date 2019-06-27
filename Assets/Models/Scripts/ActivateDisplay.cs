using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ActivateDisplay : MonoBehaviour
{
    public bool UseSingleDisplay = false;

    void Start()
    {
        Screen.fullScreen = true;
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
        Application.Instance.WriteDebug("Displays connected: " + Display.displays.Length);
        yield return null;
    }
}
