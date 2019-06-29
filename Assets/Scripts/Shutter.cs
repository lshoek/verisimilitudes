using System.Collections;
using UnityEngine;

public class Shutter : MonoBehaviour
{
    private Renderer _ShutterRenderer;

    void Start()
    {
        _ShutterRenderer = GetComponentInChildren<Renderer>();
        SetAlpha(1f);

        _ShutterRenderer.enabled = !Application.Instance.EnableShutters;
    }

    public void SetAlpha(float alpha)
    {
        Color col = Color.white;
        col.a = alpha;

        _ShutterRenderer.material.SetColor("_Color", col);
    }

    public void Fade(float src, float dst, float time)
    {
        StartCoroutine(FadeRoutine(src, dst, time));
    }

    private IEnumerator FadeRoutine(float src, float dst, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            SetAlpha(Mathf.Lerp(src, dst, elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
