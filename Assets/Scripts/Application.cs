using UnityEngine;
using Kinect = Windows.Kinect;

public class Application : MonoBehaviour
{
    public static Application Instance { get; private set; }

    private Transform _Actor;
    private BodySourceView _KinectBody;

    void Start()
    {
        _Actor = GameObject.FindGameObjectWithTag("Actor").transform;
        _KinectBody = FindObjectOfType<BodySourceView>();
    }

    void Awake()
    {
        if (Instance == null)  Instance = this;
    }

    void Update()
    {
        Vector3 headPosition = _KinectBody.GetHeadPosition() * 2f;
        headPosition.x *= -1f;
        _Actor.position = (headPosition != Vector3.zero) ? headPosition : _Actor.position;
    }
}
