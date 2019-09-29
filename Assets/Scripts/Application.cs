using UnityEngine;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    public bool EnableStencilMask = true;
    public bool EnableShutters = true;
    public bool EnableDebugging = true;
    public bool EnableAutoEffects = true;
    public bool EnableInputManual = true;

    private Rect messageRect;
    private Rect staticMessageRect;
    private GUIStyle debugMessageStyle;

    private string message = "";
    private string staticMessage = "";
    private bool endOfFrame = false;

    void Awake()
    {
        if (Instance == null) Instance = this;

        messageRect = new Rect(16, 16, Screen.width/2, Screen.height/2);
        staticMessageRect = new Rect(16, Screen.height/2, Screen.width / 2, Screen.height / 2);

        debugMessageStyle = new GUIStyle();
        debugMessageStyle.fontSize = 20;
        debugMessageStyle.normal.textColor = Color.white;
        debugMessageStyle.font = Resources.Load("Fonts/Anonymous-Pro-B") as Font;
    }

    public void AppendMessage(string msg)
    {
        if (endOfFrame)
        {
            message = "";
            endOfFrame = false;
        }
        message = string.Format(message + "{0}\n", msg);
    }

    public void StaticMessage(string msg)
    {
        staticMessage = msg;
    }

    void OnGUI()
    {
        GUI.Label(messageRect, message, debugMessageStyle);
        GUI.Label(staticMessageRect, staticMessage, debugMessageStyle);
        endOfFrame = true;
    }
}
