using UnityEngine;
using UnityEngine.UI;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    private Transform _Actor;
    private BodySourceView _KinectBody;
    private Text _DebugTextBox;

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
    }

    void Update()
    {
        if (_KinectBody != null)
        {
            Vector3 headPosition = _KinectBody.GetHeadPosition() * 2f;
            headPosition.x *= -1f;
            _Actor.position = (headPosition != Vector3.zero) ? headPosition : _Actor.position;
        }
    }

    void ActivateSequence()
    {
        Debug.Log("TRACKED");
    }

    void DeactivateSequence()
    {
        Debug.Log("UNTRACKED");
    }

    public void WriteDebug(string msg)
    {
        _DebugTextBox.text = msg;
    }
}
