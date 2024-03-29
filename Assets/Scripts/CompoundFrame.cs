﻿using UnityEngine;

public class CompoundFrame : MonoBehaviour
{
    public Camera[] _LayerCams = new Camera[3];
    private Material _MixMaterial;

    void Start()
    {
        for (int i = 0; i < _LayerCams.Length; i++)
        {
            if (i == 0) continue; // skip this camera (L2)
            _LayerCams[i].targetTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        }
        _MixMaterial = new Material(Resources.Load("Shaders/Mix") as Shader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (_LayerCams.Length > 2)
        {
            _MixMaterial.SetTexture("_Layer0", _LayerCams[2].targetTexture);
            _MixMaterial.SetTexture("_Layer1", _LayerCams[1].targetTexture);
            Graphics.Blit(src, dst, _MixMaterial);
        }
    }
}
