using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ShapeController : MonoSingleton<ShapeController>
{

    [SerializeField]
    private float m_ShapeSpawnOffset = 3f;

    [Serializable]
    public enum ShapeButton
    {
        CylinderButton,
        CubeButton,
        PyramidButton
    }

    public enum ShapeType
    {
        Cylinder,
        Cube,
        Pyramid
    }
    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnShape(ShapeButton shapeButton)
    {
        ShapeType shapeType = (ShapeType) shapeButton;
        ShapeObject shape = GameObjectPool.Instance.Allocate(shapeType);
        Transform cameraTransform = ControllerManager.Instance.GetCameraTransform();
        shape.transform.position = cameraTransform.position + cameraTransform.forward * m_ShapeSpawnOffset;
        shape.transform.rotation = cameraTransform.rotation * shape.transform.rotation;
    }

    public void DespawnShape(GameObject shapeObj)
    {
       var shape = shapeObj.GetComponent<ShapeObject>();
       GameObjectPool.Instance.Deallocate(shape);
    }

}
