using UnityEngine;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;
using Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Transform ParentTransform;
    public bool RenderKinectBody = true;
    public bool InvertX = false;

    private Material jointMaterial, boneMaterial;

    private Body trackedBody;
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    private CameraSpacePoint[] _FilteredJoints;

    private Vector3 _HeadPosition = new Vector3();
    private int _NumBodies = 1;

    private const string JOINT_ID_FORMAT = "joint:{0}";
    private const string BONE_ID_FORMAT = "bone:{0}-{1}";

    public event Action OnBodyFound;
    public event Action OnBodyLost;

    KinectJointFilter filter;

    void Start()
    {
        _BodyManager = FindObjectOfType<BodySourceManager>();

        jointMaterial = Resources.Load("Materials/JointMaterial") as Material;
        boneMaterial = Resources.Load("Materials/BoneMaterial") as Material;

        filter = new KinectJointFilter();
        filter.Init();
    }

    void Update() 
    {
        if (_BodyManager == null) return;
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
                
            if(body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
                if (trackedBody == null)
                    trackedBody = body;
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                // tracked body lost
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                OnBodyLost?.Invoke();
            }
        }

        //check if the trackedbody is still in sight
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (body.TrackingId == trackedBody.TrackingId)
                {
                    if (_Bodies.ContainsKey(body.TrackingId))
                    {
                        RefreshBodyObject(body, _Bodies[body.TrackingId]);

                        filter.UpdateFilter(body);
                        CameraSpacePoint c = filter.GetFilteredJoint(JointType.Head);
                        return;
                    }
                }
            }
        }

        // find a new body
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                float zHead = body.Joints[JointType.Head].Position.Z;
                Debug.Log(zHead);
                if (zHead > 4.5f) return;

                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    // new body found
                    _Bodies[body.TrackingId] = CreateBodyObject(body, body.TrackingId);
                    OnBodyFound?.Invoke();
                }
                RefreshBodyObject(body, _Bodies[body.TrackingId]);

                filter.UpdateFilter(body);
                CameraSpacePoint c = filter.GetFilteredJoint(JointType.Head);
                //_HeadPosition = new Vector3(c.X, c.Y, c.Z);

                return;
            }
        }
        //Debug.Log("No body found");
    }

    private GameObject ConstructJoint()
    {
        GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (!RenderKinectBody) ob.layer = LayerMask.NameToLayer("DontRender");
        ob.GetComponent<Renderer>().material = jointMaterial;

        return ob;
    }

    private GameObject CreateBodyObject(Kinect.Body body, ulong id)
    {
        GameObject bodyObj = new GameObject("body:" + id);
        bodyObj.transform.SetParent(ParentTransform, false);

        Kinect.JointType jt = Kinect.JointType.Head;
        GameObject jointObj = ConstructJoint();

        jointObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        jointObj.name = string.Format(JOINT_ID_FORMAT, jt.ToString());
        jointObj.transform.SetParent(bodyObj.transform);
        
        return bodyObj;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        Kinect.JointType jt = Kinect.JointType.Head;

        Kinect.Joint sourceJoint = body.Joints[jt];

        Transform jointObj = bodyObject.transform.Find(string.Format(JOINT_ID_FORMAT, jt.ToString()));
        Vector3 jointPosition = GetVector3FromJoint(sourceJoint);
        jointObj.localPosition = jointPosition;

        if (jt == Kinect.JointType.Head) _HeadPosition = jointPosition;
    }
    
    public Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X*(InvertX?-1f:1f), joint.Position.Y, joint.Position.Z);
    }

    public Vector3 GetHeadPosition()
    {
        return _HeadPosition;
    }
}
