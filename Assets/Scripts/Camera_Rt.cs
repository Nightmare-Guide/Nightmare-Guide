using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Camera_Rt : MonoBehaviour
{
    public static Camera_Rt instance; 
    
    private Camera m_Camera;
    private MouseLook m_MouseLook = new MouseLook(); // ���콺 ��Ʈ�ѿ� ��ü
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
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
        }
        lockerCamera = true;
        m_Camera = Camera.main;
        m_MouseLook.Init(transform, m_Camera.transform); // ���콺 ��Ʈ�� �ʱ�ȭ
        rt_Camera = GetComponent<Camera_Rt>();
    }

    private void Update()
    {
        if (lockerCamera)
        {
            RotateView(); // ���콺 ȸ�� ���� 
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
