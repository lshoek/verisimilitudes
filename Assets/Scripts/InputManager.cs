using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Transform[] CalibrationSubjects;
    private int subjectIndex = 0;

    private WindowTransformIO windowTransformIO;

    private float _DebugDelta = 0.001f;
    private bool _DebugToggle = false;

    private class KeyActionPair
    {
        public KeyCode KeyCode { get; private set; }
        public Action KeyAction { get; private set; }
        public string Description { get; private set; }
        public bool KeyDown { get; private set; }

        public KeyActionPair(KeyCode keyCode, Action keyAction, string description, bool keyDown = false)
        {
            KeyCode = keyCode;
            KeyAction = keyAction;
            Description = description;
            KeyDown = keyDown;
        }
    }
    private List<KeyActionPair> keyActions;

    void Start()
    {
        windowTransformIO = FindObjectOfType<WindowTransformIO>();
        keyActions = new List<KeyActionPair>();

        keyActions.Add(new KeyActionPair(KeyCode.X, () =>
        {
            _DebugToggle = !_DebugToggle;
            _DebugDelta = _DebugToggle ? 0.01f : 0.001f;
        }, 
        "x: toggle stepsize", true));

        keyActions.Add(new KeyActionPair(KeyCode.I, () => subjectIndex = (subjectIndex+1)%CalibrationSubjects.Length, "i: toggle obj index", true));
        keyActions.Add(new KeyActionPair(KeyCode.UpArrow, () => CalibrationSubjects[subjectIndex].position += new Vector3(0, _DebugDelta, 0), "up: move obj up"));
        keyActions.Add(new KeyActionPair(KeyCode.DownArrow, () => CalibrationSubjects[subjectIndex].position -= new Vector3(0, _DebugDelta, 0), "down: move obj down"));
        keyActions.Add(new KeyActionPair(KeyCode.LeftArrow, () => CalibrationSubjects[subjectIndex].position += new Vector3(_DebugDelta, 0, 0), "left: move obj left"));
        keyActions.Add(new KeyActionPair(KeyCode.RightArrow, () => CalibrationSubjects[subjectIndex].position -= new Vector3(_DebugDelta, 0, 0), "right: move obj right"));
        keyActions.Add(new KeyActionPair(KeyCode.Alpha1, () => CalibrationSubjects[subjectIndex].position += new Vector3(0, 0, _DebugDelta), "1: move obj forward"));
        keyActions.Add(new KeyActionPair(KeyCode.Alpha2, () => CalibrationSubjects[subjectIndex].position -= new Vector3(0, 0, _DebugDelta), "2: move obj back"));
        keyActions.Add(new KeyActionPair(KeyCode.D, () => CalibrationSubjects[subjectIndex].localScale += new Vector3(_DebugDelta, 0, 0), "d: scale obj x up"));
        keyActions.Add(new KeyActionPair(KeyCode.A, () => CalibrationSubjects[subjectIndex].localScale -= new Vector3(_DebugDelta, 0, 0), "a: scale obj x down"));
        keyActions.Add(new KeyActionPair(KeyCode.W, () => CalibrationSubjects[subjectIndex].localScale += new Vector3(0, _DebugDelta, 0), "w: scale obj y up"));
        keyActions.Add(new KeyActionPair(KeyCode.S, () => CalibrationSubjects[subjectIndex].localScale -= new Vector3(0, _DebugDelta, 0), "s: scale obj y down"));
        keyActions.Add(new KeyActionPair(KeyCode.R, () => CalibrationSubjects[subjectIndex].localScale += new Vector3(0, 0, _DebugDelta), "r: scale obj z up"));
        keyActions.Add(new KeyActionPair(KeyCode.F, () => CalibrationSubjects[subjectIndex].localScale -= new Vector3(0, 0, _DebugDelta), "f: scale obj z down"));
        keyActions.Add(new KeyActionPair(KeyCode.F1, () => windowTransformIO.Save(), "f1: save window transforms", true));
        keyActions.Add(new KeyActionPair(KeyCode.F2, () => windowTransformIO.Load(), "f2: load window transforms", true));

        if (Application.Instance.EnableInputManual)
        {
            string msg = "\n";
            foreach (KeyActionPair k in keyActions)
            {
                msg = $"{msg}\n{k.Description}";
            }
            Application.Instance.StaticMessage(msg);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Instance.EnableDebugging = !Application.Instance.EnableDebugging;

        if (Application.Instance.EnableDebugging)
        {
            foreach (KeyActionPair k in keyActions)
            {
                if (k.KeyDown)
                {
                    if (Input.GetKeyDown(k.KeyCode))
                        k.KeyAction();
                }
                else
                {
                    if (Input.GetKey(k.KeyCode))
                        k.KeyAction();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                float scale = Input.mousePosition.x / Screen.width * 100.0f;
            }
            if (CalibrationSubjects != null)
            {
                Application.Instance.AppendMessage($"selected: {CalibrationSubjects[subjectIndex].name}\n" +
                    $"{AppHelper.TransformToString(CalibrationSubjects[subjectIndex])}\ndelta: {_DebugDelta}\nt: {Time.time}");
            }
        }
    }
}
