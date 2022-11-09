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
    [SerializeField] private ViveColliderEventCaster m_RightGrabber;
    [SerializeField] private GameObject m_RightLaserPointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_RightLaserRaycaster;
    [SerializeField] private GameObject m_RightCurvePointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_RightCurveRaycaster;


    [Header("Left Controller")]
    [SerializeField] private ViveColliderEventCaster m_LeftGrabber;
    [SerializeField] private GameObject m_LeftLaserPointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_LeftLaserRaycaster;
    [SerializeField] private GameObject m_LeftCurvePointer;
    [SerializeField] private HTC.UnityPlugin.Pointer3D.Pointer3DRaycaster m_LeftCurveRaycaster;

    [Header("Constants")]
    [SerializeField] private float m_HeightOffset = 5f;

    private ShapeObject m_LeftDraggingShape = null;
    private ShapeObject m_RightDraggingShape = null;


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

        // Move with teleport raycast on left trigger
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

        // Open Menu
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Menu) || ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Menu))
        {
            bool menuActive = UIController.Instance.MenuActive;
            UIController.Instance.SetMenuActive(!menuActive);
        }

        // Menu Interaction with right controller
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Pad) && UIController.Instance.MenuActive)
        {
            var hits = m_RightLaserRaycaster.SortedRaycastResults;

            foreach(var hit in hits)
            {
                //Spawn shape based on button tag
                ShapeController.ShapeButton shapeType;
                if (Enum.TryParse(hit.gameObject.tag, out shapeType))
                {
                    ShapeController.Instance.SpawnShape(shapeType);
                    break;
                }
                else if (hit.gameObject.tag == "SaveButton")
                {
                    ShapeController.Instance.SaveLevel();
                    break;
                }
                else if (hit.gameObject.tag == "LoadButton")
                {

                    ShapeController.Instance.LoadLevel();
                    break;
                }
            }
        }

        // Delete objects with Left controller
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Pad))
        {
            var hits = m_LeftLaserRaycaster.SortedRaycastResults;

            foreach(var hit in hits)
            {
                if (hit.gameObject.tag == "ShapeObject")
                {
                    ShapeController.Instance.DespawnShape(hit.gameObject);
                    break;
                }
            }
        }


        // Left hand drag and rotate logic
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Grip))
        {
            if (m_LeftDraggingShape == null)
            {
                var colliders = m_LeftGrabber.enteredColliders;
                foreach(var collider in colliders)
                {
                    // Find first collider that is a shape
                    if (collider.gameObject.tag == "ShapeObject")
                    {
                        var shapeObject = collider.GetComponent<ShapeObject>();
                        //Prevent grabbing shape with both hands
                        if (m_RightDraggingShape == null || shapeObject != m_RightDraggingShape)
                        {
                            shapeObject.StartDragging(m_LeftGrabber.transform);
                            m_LeftDraggingShape = shapeObject;
                            break;
                        }
                    }   
                }
            }

        }
        //Release Object
        else if (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Grip))
        {
            if (m_LeftDraggingShape != null)
            {
                m_LeftDraggingShape.StopDragging();
                m_LeftDraggingShape = null;
            }
        }
        
        //Right hand dragging logic redundant code
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Grip))
        {      
            if (m_RightDraggingShape == null)
            {
                var colliders = m_RightGrabber.enteredColliders;
                foreach(var collider in colliders)
                {
                    if (collider.gameObject.tag == "ShapeObject")
                    {
                        var shapeObject = collider.GetComponent<ShapeObject>();
                        if (m_LeftDraggingShape == null || shapeObject != m_LeftDraggingShape)
                        {
                            shapeObject.StartDragging(m_LeftGrabber.transform);
                            m_RightDraggingShape = shapeObject;
                            break;
                        }
                    }
                }
            }
        }
        else if (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Grip))
        {
            if (m_RightDraggingShape != null)
            {
                m_RightDraggingShape.StopDragging();
                m_RightDraggingShape = null;
            }
        }


    }

    public Transform GetCameraTransform()
    {
        return m_CameraObject.gameObject.transform;
    }
}
