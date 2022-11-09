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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
