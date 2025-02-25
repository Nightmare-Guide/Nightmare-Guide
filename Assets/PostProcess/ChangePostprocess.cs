using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ChangePostprocess : MonoBehaviour
{
    public PostProcessingProfile profile;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            profile.vignette.enabled = true;
            profile.colorGrading.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {   
            profile.vignette.enabled = false;
            profile.colorGrading.enabled = true;
        }
    }
}
