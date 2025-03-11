using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Camera playerCamera;
    public Portal[] portals;

    void LateUpdate()
    {
        foreach (Portal portal in portals)
        {
            portal.UpdateCamera(playerCamera);
        }
    }
}