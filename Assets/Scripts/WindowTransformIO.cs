using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WindowTransformIO : MonoBehaviour
{
    private List<SerializableTransformList> WindowTransformLists;

    private GameObject[] windowSizes;

    private string FolderName = "SaveData";
    private string FileName = "WindowTransforms";
    private string Extension = ".json";

    void Start()
    {
        WindowTransformLists = new List<SerializableTransformList>();

        windowSizes = GameObject.FindGameObjectsWithTag("WindowSize");
        List<SerializableTransform> windowSizeTransforms = new List<SerializableTransform>();

        for (int i = 0; i < windowSizes.Length; i++)
        {
            WindowTransformLists.Add(new SerializableTransformList());
            WindowTransformLists[i].TransformList = new List<SerializableTransform>();
        }
    }

    public void Load()
    {
        Debug.Log(UnityEngine.Application.persistentDataPath);
        for (int i = 0; i < windowSizes.Length; i++)
        {
            string fullPath = Path.Combine(UnityEngine.Application.persistentDataPath, FolderName, $"{FileName}_L{i + 1}{Extension}");
            if (File.Exists(fullPath))
            {
                string jsonString = File.ReadAllText(fullPath);
                SerializableTransformList transformList = JsonUtility.FromJson<SerializableTransformList>(jsonString);

                // first element is always the root transform (WindowSize_LX)
                SerializeUtilities.DeserializeTransform(transformList.TransformList[0], windowSizes[i].transform);
                Transform[] transforms = windowSizes[i].GetComponentsInChildren<Transform>();

                int transformIndex = 1; // skip the first entry
                for (int j = 0; j < transforms.Length; j++)
                {
                    if (transforms[j].tag.Equals("StencilMask"))
                    {
                        SerializeUtilities.DeserializeTransform(transformList.TransformList[transformIndex], transforms[j]);
                        transformIndex++;
                    }
                }
            }
        }
    }

    public void Save()
    {
        for (int i = 0; i < windowSizes.Length; i++)
        {
            WindowTransformLists[i].TransformList.Add(SerializeUtilities.SerializeTransform(windowSizes[i].transform));
           
            Transform[] transforms = windowSizes[i].GetComponentsInChildren<Transform>();
            foreach (Transform t in transforms)
            {
                if (t.tag.Equals("StencilMask"))
                    WindowTransformLists[i].TransformList.Add(SerializeUtilities.SerializeTransform(t));
            }
            string jsonString = JsonUtility.ToJson(WindowTransformLists[i], true);

            string fullPath = Path.Combine(UnityEngine.Application.persistentDataPath, FolderName, $"{FileName}_L{i + 1}{Extension}");
            if (File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, jsonString);
                Debug.Log($"Saved transforms to {fullPath}");
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(UnityEngine.Application.persistentDataPath, FolderName));
                File.Create(fullPath);
                Debug.Log($"Savedata file was created: {fullPath}");
            }
        }
    }
}

[Serializable]
public class SerializableTransformList
{
    public List<SerializableTransform> TransformList;
}

[Serializable]
public class SerializableTransform
{
    public float[] position;
    public float[] scale;
}

public static class SerializeUtilities
{
    public static SerializableTransform SerializeTransform(Transform t)
    {
        float[] _position = new float[3];
        float[] _scale = new float[3];

        _position[0] = t.localPosition.x;
        _position[1] = t.localPosition.y;
        _position[2] = t.localPosition.z;

        _scale[0] = t.localScale.x;
        _scale[1] = t.localScale.y;
        _scale[2] = t.localScale.z;

        return new SerializableTransform { position = _position, scale = _scale };
    }

    public static Transform DeserializeTransform(SerializableTransform src, Transform dst)
    {
        dst.localPosition = new Vector3(src.position[0], src.position[1], src.position[2]);
        dst.localScale = new Vector3(src.scale[0], src.scale[1], src.scale[2]);
        return dst;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
