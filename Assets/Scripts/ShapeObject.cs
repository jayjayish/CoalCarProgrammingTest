using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShapeObject : MonoBehaviour
{
    [SerializeField]
    private ShapeController.ShapeType m_ShapeType = ShapeController.ShapeType.Cylinder;
    public ShapeController.ShapeType ShapeType => m_ShapeType;
    private bool m_IsDragging = false;
    private Transform m_HandTransform;
    private Vector3 m_InitialHandPosition;
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialHandRotation;
    private Quaternion m_InitialRotation;

    // Update is called once per frame
    void Update()
    {
        if (m_IsDragging)
        {
            // Calculate new position based on difference in hand's initial transform and current transform
            gameObject.transform.position = m_InitialPosition + m_HandTransform.position - m_InitialHandPosition;
            gameObject.transform.rotation = m_InitialRotation * m_HandTransform.rotation * m_InitialHandRotation;
        }
    }


    public void StartDragging(Transform parentHand)
    {
        // Keep track of which hand's transform
        m_HandTransform = parentHand;
        m_IsDragging = true;

        // Save quaternion as inverse since we want to undo its effects
        m_InitialHandPosition = m_HandTransform.position;
        m_InitialHandRotation = Quaternion.Inverse(m_HandTransform.rotation);
        
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;

    }

    public void StopDragging()
    {
        m_HandTransform = null;
        m_IsDragging = false;
    }
}
