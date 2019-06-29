using System.Collections;
using UnityEngine;

public class Shutter : MonoBehaviour
{
    private Renderer _ShutterRenderer;
    private Coroutine currentRoutine;
    [HideInInspector] public bool IsFading = false;

    void Start()
    {
        _ShutterRenderer = GetComponentInChildren<Renderer>();
        SetAlpha(1f);

        _ShutterRenderer.enabled = Application.Instance.EnableShutters;
    }

    public void SetAlpha(float alpha)
    {
        Color col = Color.white;
        col.a = alpha;

        _ShutterRenderer.material.SetColor("_Color", col);
    }

    public void FadeTo(float dst, float time)
    {
        float src = _ShutterRenderer.material.GetColor("_Color").a;

        if (IsFading) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeRoutine(src, dst, time));
    }

    public void Fade(float src, float dst, float time)
    {
        if (IsFading) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeRoutine(src, dst, time));
    }

    private IEnumerator FadeRoutine(float src, float dst, float time)
    {
        IsFading = true;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            SetAlpha(Mathf.Lerp(src, dst, elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(dst);
        IsFading = false;
    }
}
