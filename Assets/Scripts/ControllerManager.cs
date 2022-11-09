using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using UnityEngine.EventSystems;
public class ControllerManager : MonoSingleton<ControllerManager>
{
    [SerializeField] private GameObject m_CharacterObject;
    [SerializeField] private GameObject m_CameraObject;

    [Header("Right Controller")]
    [SerializeField] private GameObject m_RightGrabber;
    [SerializeField] private GameObject m_RightLaserPointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_RightLaserRaycaster;
    [SerializeField] private GameObject m_RightCurvePointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_RightCurveRaycaster;


    [Header("Left Controller")]
    [SerializeField] private GameObject m_LeftGrabber;
    [SerializeField] private GameObject m_LeftLaserPointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_LeftLaserRaycaster;
    [SerializeField] private GameObject m_LeftCurvePointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_LeftCurveRaycaster;

    [Header("Constants")]
    [SerializeField] private float m_HeightOffset = 5f;


    // Start is called before the first frame update
    void Start()
    {
        m_RightLaserPointer.SetActive(true);
        m_LeftLaserPointer.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        // Move with teleport raycast on right trigger
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger))
        {
            m_RightCurvePointer.SetActive(true);
            m_RightLaserPointer.SetActive(false);
        }
        else if (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Trigger))
        {
            m_CharacterObject.transform.position = m_RightCurveRaycaster.FirstRaycastResult().worldPosition + new Vector3(0f, m_HeightOffset, 0f);
            m_RightCurvePointer.SetActive(false);
            m_RightLaserPointer.SetActive(true);
        }

        // Move with teleport raycast on right trigger
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Trigger))
        {
            m_LeftCurvePointer.SetActive(true);
            m_LeftLaserPointer.SetActive(false);
        }
        else if (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Trigger))
        {
            m_CharacterObject.transform.position = m_LeftCurveRaycaster.FirstRaycastResult().worldPosition + new Vector3(0f, m_HeightOffset, 0f);
            m_LeftCurvePointer.SetActive(false);
            m_LeftLaserPointer.SetActive(true);
        }

        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Menu) || ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Menu))
        {
            bool menuActive = UIController.Instance.MenuActive;
            UIController.Instance.SetMenuActive(!menuActive);
        }


        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Pad) && UIController.Instance.MenuActive)
        {
            var hits = m_RightLaserRaycaster.SortedRaycastResults;

            foreach(var hit in hits)
            {
                ShapeController.ShapeButton shapeType;
                if (Enum.TryParse(hit.gameObject.tag, out shapeType))
                {
                    ShapeController.Instance.SpawnShape(shapeType);
                }
            }
        }

        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Pad))
        {
            var hits = m_LeftLaserRaycaster.SortedRaycastResults;

            foreach(var hit in hits)
            {
                Debug.Log(hit.gameObject);
                if (hit.gameObject.tag == "ShapeObject")
                {
                    ShapeController.Instance.DespawnShape(hit.gameObject);
                }
            }
        }

    }

    public Transform GetCameraTransform()
    {
        return m_CameraObject.gameObject.transform;
    }
}
