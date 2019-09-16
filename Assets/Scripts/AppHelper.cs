using UnityEngine;

public static class AppHelper
{
    public static string TransformToString(Transform t)
    {
        string result =
            t.localPosition.ToString("F3") + "\n" +
            t.localRotation.ToString("F3") + "\n" +
            t.localScale.ToString("F3") + "\n";
        return result;
    }
}
