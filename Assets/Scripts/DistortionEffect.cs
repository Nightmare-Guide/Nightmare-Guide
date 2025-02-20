using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionEffect : MonoBehaviour
{
    public Material distortionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (distortionMaterial != null)
        {
            Graphics.Blit(src, dest, distortionMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
