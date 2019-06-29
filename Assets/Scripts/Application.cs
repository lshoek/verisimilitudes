using UnityEngine;
using UnityEngine.UI;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    public bool EnableStencilMask = true;
    public bool EnableShutters = true;
    public bool EnableDebugging = true;
    public bool EnableAutoEffects = true;

    [Range(0, 5)] public int ShaderIndex = 0;
    [Range(2f, 100f)] public float Density = 30f;

    public Transform CalibrationSubject;

    private Transform _Actor;
    private Transform _ScanTransform;

    private BodySourceView _KinectBody;
    private Shutter[] _Shutters;
    private Renderer[] _ScanObjects;

    private float _ShaderIndexInterval = 3f;
    private float _NextIndexTime = 3f;
    private float _ShaderEffectLifeTime = 5f;
    private int _TempShaderIndex = 0;

    private bool _JitterMode = false;
    private bool _Jittering = false;
    private float _JitterLifeTime = 5f;
    private float _JitterInterval = 0.125f;

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

        _Shutters = FindObjectsOfType<Shutter>();

        _DebugTextBox = GameObject.FindGameObjectWithTag("DebugTextBox").GetComponent<Text>();
        _ScanTransform = GameObject.FindGameObjectWithTag("ScanTransform").GetComponent<Transform>();
        _ScanObjects = _ScanTransform.GetComponentsInChildren<Renderer>();

        if (EnableStencilMask)
        {
            foreach (Renderer r in _ScanObjects)
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
            Vector3 headPosition = _KinectBody.GetHeadPosition();
            _Actor.position = (headPosition != Vector3.zero) ? headPosition : _Actor.position;
        }

        ManipulateScanObjects();

        // CALIBRATION STUFF
        if (EnableDebugging)
        {
            if (Input.GetKeyDown(KeyCode.X)) { _DebugToggle = !_DebugToggle; }
            _DebugDelta = _DebugToggle ? 0.01f : 0.001f;

            if (Input.GetKey(KeyCode.UpArrow)) { CalibrationSubject.position += new Vector3(0, _DebugDelta, 0); }
            if (Input.GetKey(KeyCode.DownArrow)) { CalibrationSubject.position -= new Vector3(0, _DebugDelta, 0); }
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
            WriteDebug(TransformToString(CalibrationSubject) + 
                "delta:" + _DebugDelta + 
                "\n elapsedTime:" + Time.time +
                "\n shaderIndex:" + ShaderIndex);
        }
    }

    void ManipulateScanObjects()
    {
        float elapsedTime = Time.time;

        if (EnableAutoEffects)
        {
            if (elapsedTime > _NextIndexTime)
            {
                _TempShaderIndex = Random.Range(1, 5 + 1);
                _ShaderEffectLifeTime = elapsedTime + Random.Range(1.0f, 5f);
                _ShaderIndexInterval = Random.Range(10f, 15f); //15f, 35f
                _NextIndexTime = elapsedTime + _ShaderIndexInterval;
                if (Random.Range(0f, 1f) > 0.0f) _JitterMode = true; //feuibfghuebfg
            }
            if (_JitterMode)
            {
                _JitterLifeTime = elapsedTime + Random.Range(0.5f, 2.5f);
                _Jittering = true;
                _JitterMode = false;
            }
            if (_Jittering)
            {
                ShaderIndex = ((elapsedTime % _JitterInterval) > _JitterInterval / 2) ? 0 : _TempShaderIndex;
                if (elapsedTime > _JitterLifeTime)
                {
                    _Jittering = false;
                }
            }
            else ShaderIndex = _TempShaderIndex;
            if (elapsedTime > _ShaderEffectLifeTime)
            {
                ShaderIndex = 0;
            }
        }
        Density = 1f + Mathf.Sin(elapsedTime/10f) * 30f;
        foreach (Renderer r in _ScanObjects)
        {
            r.material.SetFloat("_Index", ShaderIndex);
            r.material.SetFloat("_Density", Density);
        }
    }

    void ActivateSequence()
    {
        foreach (Shutter shutter in _Shutters) shutter.FadeTo(0f, 1f);
    }

    void DeactivateSequence()
    {
        Debug.Log("DEAC");
        foreach (Shutter shutter in _Shutters) shutter.FadeTo(1f, 1f);
    }

    static string TransformToString(Transform t)
    {
        string result =
            t.localPosition.ToString("F3") + "\n" +
            t.localRotation.ToString("F3") + "\n" +
            t.localScale.ToString("F3") + "\n";
        return result;
    }

    public void WriteDebug(string msg)
    {
        _DebugTextBox.text = msg;
    }
}
