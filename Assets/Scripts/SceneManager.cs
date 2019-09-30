using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Range(0, 5)] public int ShaderIndex = 0;
    [Range(2f, 100f)] public float Density = 30f;

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

        _ScanTransform = GameObject.FindGameObjectWithTag("ScanTransform").GetComponent<Transform>();
        _ScanObjects = _ScanTransform.GetComponentsInChildren<Renderer>();

        if (Application.Instance.EnableStencilMask)
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

                if (Application.Instance.EnableShutters)
                {
                    GameObject shutter = Instantiate(Resources.Load("Prefabs/Shutter"), ob.transform) as GameObject;
                    shutter.name = "Shutter";

                    foreach (Transform child in shutter.GetComponentsInChildren<Transform>())
                        child.gameObject.layer = ob.layer;
                }
            }
            if (Application.Instance.EnableShutters)
            {
                _Shutters = FindObjectsOfType<Shutter>();
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
    }

    private void ManipulateScanObjects()
    {
        float elapsedTime = Time.time;

        if (Application.Instance.EnableAutoEffects)
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
        Density = 1f + Mathf.Sin(elapsedTime / 10f) * 30f;
        foreach (Renderer r in _ScanObjects)
        {
            r.material.SetFloat("_Index", ShaderIndex);
            r.material.SetFloat("_Density", Density);
        }
    }

    private void ActivateSequence()
    {
        if (Application.Instance.EnableShutters)
            foreach (Shutter shutter in _Shutters) shutter.FadeTo(0f, 1f);
    }

    private void DeactivateSequence()
    {
        if (Application.Instance.EnableShutters)
            foreach (Shutter shutter in _Shutters) shutter.FadeTo(1f, 1f);
    }
}
