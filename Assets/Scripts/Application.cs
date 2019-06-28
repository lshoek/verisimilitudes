using UnityEngine;
using UnityEngine.UI;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    public bool EnableStencilMask = true;

    public Transform CalibrationSubject;
    public Vector3 CameraBias = new Vector3();

    private Transform _Actor;
    private Transform _ScanTransform;

    private BodySourceView _KinectBody;
    private Text _DebugTextBox;

    private float _DebugDelta = 0.001f;
    private bool _DebugToggle = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _Actor = GameObject.FindGameObjectWithTag("Actor").transform;
        _KinectBody = FindObjectOfType<BodySourceView>();
        _KinectBody.OnBodyFound += ActivateSequence;
        _KinectBody.OnBodyLost += DeactivateSequence;

        if (_KinectBody == null)
        {
            Debug.Log("Could not find BodySourceView object in scene");
        }

        _DebugTextBox = GameObject.FindGameObjectWithTag("DebugTextBox").GetComponent<Text>();
        _ScanTransform = GameObject.FindGameObjectWithTag("ScanTransform").GetComponent<Transform>();

        if (EnableStencilMask)
        {
            foreach (Renderer r in _ScanTransform.GetComponentsInChildren<Renderer>())
            {
                Texture tempTex = r.material.mainTexture;
                r.material = new Material(Resources.Load("Shaders/StencilRead") as Shader);
                r.material.mainTexture = tempTex;
                r.material.SetColor("_Color", Color.white);
                r.material.SetInt("_StencilRef", 1);
            }
            foreach (GameObject ob in GameObject.FindGameObjectsWithTag("StencilMask"))
            {
                Renderer r = ob.GetComponent<Renderer>();
                r.material = new Material(Resources.Load("Shaders/StencilWrite") as Shader);
                r.material.SetInt("_StencilRef", 1);
            }
        }
    }

    void Update()
    {
        if (_KinectBody != null)
        {
            Vector3 headPosition = _KinectBody.GetHeadPosition() + CameraBias;
            _Actor.position = (headPosition != Vector3.zero) ? headPosition : _Actor.position;
        }

        // CALIBRATION STUFF
        if (Input.GetKeyDown(KeyCode.X)) { _DebugToggle = !_DebugToggle; }
        _DebugDelta = _DebugToggle ? 0.01f : 0.001f;

        if (Input.GetKey(KeyCode.UpArrow)) { CalibrationSubject.position += new Vector3(0, _DebugDelta, 0); }
        if (Input.GetKey(KeyCode.DownArrow)) { CalibrationSubject.position -= new Vector3(0, _DebugDelta, 0);  }
        if (Input.GetKey(KeyCode.LeftArrow)) { CalibrationSubject.position += new Vector3(_DebugDelta, 0, 0); }
        if (Input.GetKey(KeyCode.RightArrow)) { CalibrationSubject.position -= new Vector3(_DebugDelta, 0, 0); }
        if (Input.GetKey(KeyCode.Alpha1)) { CalibrationSubject.position += new Vector3(0, 0, _DebugDelta); }
        if (Input.GetKey(KeyCode.Alpha2)) { CalibrationSubject.position -= new Vector3(0, 0, _DebugDelta); }

        //if (Input.GetKey(KeyCode.W)) { CalibrationSubject.localScale += new Vector3(_DebugDelta, _DebugDelta, _DebugDelta); }
        //if (Input.GetKey(KeyCode.S)) { CalibrationSubject.localScale -= new Vector3(_DebugDelta, _DebugDelta, _DebugDelta); }

        if (Input.GetKey(KeyCode.D)) { CalibrationSubject.localScale += new Vector3(_DebugDelta, 0, 0); }
        if (Input.GetKey(KeyCode.A)) { CalibrationSubject.localScale -= new Vector3(_DebugDelta, 0, 0); }
        if (Input.GetKey(KeyCode.W)) { CalibrationSubject.localScale += new Vector3(0, _DebugDelta, 0); }
        if (Input.GetKey(KeyCode.S)) { CalibrationSubject.localScale -= new Vector3(0, _DebugDelta, 0); }

        if (Input.GetMouseButtonDown(0))
        {
            float scale = Input.mousePosition.x / Screen.width * 100.0f;
        }
        WriteDebug(TransformToString(CalibrationSubject) + "delta:" + _DebugDelta);
    }

    void ActivateSequence()
    {
        Debug.Log("TRACKED");
    }

    void DeactivateSequence()
    {
        Debug.Log("UNTRACKED");
    }

    static string TransformToString(Transform t)
    {
        string result =
            t.localPosition.ToString("F6") + "\n" +
            t.localRotation.ToString("F6") + "\n" +
            t.localScale.ToString("F6") + "\n";
        return result;
    }

    public void WriteDebug(string msg)
    {
        _DebugTextBox.text = msg;
    }
}
