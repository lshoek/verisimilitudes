using UnityEngine;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    private Transform _Actor;
    private BodySourceView _KinectBody;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _Actor = GameObject.FindGameObjectWithTag("Actor").transform;
        _KinectBody = FindObjectOfType<BodySourceView>();

        if (_KinectBody == null)
        {
            Debug.Log("Could not find BodySourceView object in scene");
        }
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
}
