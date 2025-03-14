using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Camera playerCamera;
    public LeftPortal[] leftportals;
    public RightPortal[] rightportals;
    public Camera leftPortalCamera;
    public Camera rightPortalCamera;
    public int portalStage = 0;
    [Header("LeftCameraÁÂÇ¥")]
    public Transform leftCameraFirstPosition;
    public Transform leftCameraSecondPosition;
    public Transform leftCameraThirdPosition;

    [Header("RightCameraÁÂÇ¥")]
    public Transform rightCameraFirstPosition;
    public Transform rightCameraSecondPosition;
    public Transform rightCameraThirdPosition;


    void LateUpdate()
    {

        foreach (LeftPortal portal in leftportals)
        {
            portal.UpdateCamera(playerCamera);
        }
        foreach (RightPortal portal in rightportals)
        {
            portal.UpdateCamera(playerCamera);
        }

        switch (portalStage)
        {
            case 0:
                rightPortalCamera.transform.position = rightCameraFirstPosition.transform.position;
                leftPortalCamera.transform.position = leftCameraFirstPosition.transform.position;
                break;
            case 1:
                rightPortalCamera.transform.position = rightCameraSecondPosition.transform.position;
                leftPortalCamera.transform.position = leftCameraSecondPosition.transform.position;
                break;
            case 2:
                rightPortalCamera.transform.position = rightCameraThirdPosition.transform.position;
                leftPortalCamera.transform.position = leftCameraThirdPosition.transform.position;
                break;
            default:
                rightPortalCamera.transform.position = rightCameraFirstPosition.transform.position;
                leftPortalCamera.transform.position = leftCameraFirstPosition.transform.position;
                break;
        }
    }


}