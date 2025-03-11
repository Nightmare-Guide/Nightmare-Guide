using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Camera_Rt : MonoBehaviour
{
    public static Camera_Rt instance; 
    
    private Camera m_Camera;
    private MouseLook m_MouseLook = new MouseLook(); // 마우스 컨트롤용 객체
    private Camera_Rt rt_Camera;
    public bool lockerCamera=true;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스 제거
        }
        lockerCamera = true;
        m_Camera = Camera.main;
        m_MouseLook.Init(transform, m_Camera.transform); // 마우스 컨트롤 초기화
        rt_Camera = GetComponent<Camera_Rt>();
    }

    private void Update()
    {
        if (lockerCamera)
        {
            RotateView(); // 마우스 회전 실행 
        }
      
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }
    public void Open_Camera()
    {
        m_MouseLook.Init(transform, m_Camera.transform);
        lockerCamera = true;
    }
    public void Close_Camera()
        {
            lockerCamera = false;
        }

    }
