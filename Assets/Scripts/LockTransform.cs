using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransform : MonoBehaviour
{
    public bool lockPosition = true;
    public bool lockRotation = true;
    public bool lockScale = true;

    private GameObject dummyObject;
    private Transform originalTranform;

    void Start()
    {
        dummyObject = new GameObject();
        originalTranform = dummyObject.transform;
    }

    void Update()
    {
        if (lockPosition) transform.position = originalTranform.position;
        if (lockRotation) transform.rotation = originalTranform.rotation;
        if (lockScale) transform.localScale = originalTranform.localScale;
    }
}
