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
    public float MaxRecognitionDistance = 4f;

    private Material jointMaterial, boneMaterial;

    private Dictionary<ulong, Body> _Bodies;
    private GameObject _ActiveBodyObject;
    private ulong _ActiveBodyIndex;

    private BodySourceManager _BodyManager;
    private CameraSpacePoint[] _FilteredJoints;

    private Vector3 _HeadPosition = new Vector3();

    private const string JOINT_ID_FORMAT = "joint:{0}";
    private const string BONE_ID_FORMAT = "bone:{0}-{1}";

    public event Action OnBodyFound;
    public event Action OnBodyLost;

    KinectJointFilter filter;

    void Start()
    {
        _BodyManager = FindObjectOfType<BodySourceManager>();

        _Bodies = new Dictionary<ulong, Body>();

        jointMaterial = Resources.Load("Materials/JointMaterial") as Material;
        boneMaterial = Resources.Load("Materials/BoneMaterial") as Material;

        filter = new KinectJointFilter();
        filter.Init();
    }

    void Update() 
    {
        if (_BodyManager == null)
            return;

        Body activeBody = null;
        Body[] data = _BodyManager.GetData();
        if (data == null)
            return;
        
        // get all bodies in latest frame
        List<ulong> trackedIds = new List<ulong>();
        foreach (Body b in data)
        {
            if (b == null)
                continue;

            if(b.IsTracked)
                trackedIds.Add(b.TrackingId);
        }

        // remove all untracked bodies
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        foreach (ulong id in knownIds)
        {
            if (!trackedIds.Contains(id))
            {
                // tracked body lost
                _Bodies.Remove(id);

                if (id == _ActiveBodyIndex)
                {
                    Destroy(_ActiveBodyObject);
                    _ActiveBodyIndex = 0;
                    OnBodyLost?.Invoke();
                }
            }
        }

        // add new bodies
        foreach (Body b in data)
        {
            if (b == null)
                continue;

            if (b.IsTracked)
            {
                if (!_Bodies.ContainsKey(b.TrackingId))
                    _Bodies[b.TrackingId] = b;
            }
        }

        // find the body that is closest to the camera
        if (_ActiveBodyIndex == 0)
        {
            bool newBodyFound = false;

            // first take a look in the list of bodies that remained in sight
            if (_Bodies.Count > 0)
            {
                float min_z = MaxRecognitionDistance;
                ulong min_id = 0;
                foreach (ulong id in _Bodies.Keys)
                {
                    _Bodies.TryGetValue(id, out Body b);
                    float z = b.Joints[JointType.Head].Position.Z;

                    if (z < MaxRecognitionDistance && z < min_z)
                    {
                        min_z = z;
                        min_id = id;
                    }
                }
                if (min_z < MaxRecognitionDistance)
                {
                    newBodyFound = true;
                    foreach (Body b in data)
                    {
                        if (b.TrackingId == min_id)
                        {
                            RegisterNewActiveBody(min_id, b);
                            break;
                        }
                    }
                }
            }

            // then take a look in the list of all bodies that are new in the latest frame
            if (!newBodyFound)
            {
                float min_z = MaxRecognitionDistance;
                ulong min_id = 0;
                foreach (Body b in data)
                {
                    if (b == null)
                        continue;

                    if (b.IsTracked)
                    {
                        float z = b.Joints[JointType.Head].Position.Z;

                        if (z < MaxRecognitionDistance && z < min_z)
                        {
                            min_z = z;
                            min_id = b.TrackingId;
                        }
                    }
                }
                if (min_z < MaxRecognitionDistance)
                {
                    newBodyFound = true;
                    foreach (Body b in data)
                    {
                        if (b.TrackingId == min_id)
                        {
                            RegisterNewActiveBody(min_id, b);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (Body b in data)
            {
                if (_ActiveBodyIndex != 0 && b.TrackingId == _ActiveBodyIndex)
                {
                    activeBody = b;
                    break;
                }
            }
        }

        if (activeBody != null)
        {
            RefreshBodyObject(activeBody, _ActiveBodyObject);
            filter.UpdateFilter(activeBody);
            CameraSpacePoint c = filter.GetFilteredJoint(JointType.Head);
        }
    }

    private void RegisterNewActiveBody(ulong id, Body b)
    {
        _ActiveBodyIndex = id;
        _ActiveBodyObject = CreateBodyObject(b, b.TrackingId);
        OnBodyFound?.Invoke();
    }

    private GameObject ConstructJoint()
    {
        GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (!RenderKinectBody) ob.layer = LayerMask.NameToLayer("DontRender");
        ob.GetComponent<Renderer>().material = jointMaterial;

        return ob;
    }

    private GameObject CreateBodyObject(Body body, ulong id)
    {
        GameObject bodyObj = new GameObject("body:" + id);
        bodyObj.transform.SetParent(ParentTransform, false);

        JointType jt = JointType.Head;
        GameObject jointObj = ConstructJoint();

        jointObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        jointObj.name = string.Format(JOINT_ID_FORMAT, jt.ToString());
        jointObj.transform.SetParent(bodyObj.transform);
        
        return bodyObj;
    }
    
    private void RefreshBodyObject(Body body, GameObject bodyObject)
    {
        JointType jt = JointType.Head;

        Kinect.Joint sourceJoint = body.Joints[jt];

        Transform jointObj = bodyObject.transform.Find(string.Format(JOINT_ID_FORMAT, jt.ToString()));
        Vector3 jointPosition = GetVector3FromJoint(sourceJoint);
        jointObj.localPosition = jointPosition;

        if (jt == JointType.Head) _HeadPosition = jointPosition;
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
