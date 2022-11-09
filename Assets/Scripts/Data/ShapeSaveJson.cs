using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
public class ShapeSaveJson
{
    public ShapeController.ShapeType ShapeType;
    public Vector3 Position;
    public Quaternion Rotation;

    public ShapeSaveJson(ShapeController.ShapeType shapeType, Vector3 pos, Quaternion quat)
    {
        ShapeType = shapeType;
        Position = pos;
        Rotation = quat;
    }
}

[System.Serializable]
public class ShapeSaveJsonList
{
    public List<ShapeSaveJson> SaveList;
}
