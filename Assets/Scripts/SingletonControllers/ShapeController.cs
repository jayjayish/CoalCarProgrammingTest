using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Linq;

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

    // Take shape type and spawn shape a distance away from face
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

    public void SaveLevel()
    {
        string path = EditorUtility.SaveFilePanel(
            "Save level to json",
            "",
            "save.json",
            "json");
        // Get copy of which shapes are active from pool
        var gameObjectLifetimes = GameObjectPool.Instance.GameObjectLifetimes;
        var gameObjects = GameObjectPool.Instance.GameObjects;

        ShapeSaveJsonList saveJson = new ShapeSaveJsonList();
        saveJson.SaveList = new List<ShapeSaveJson>();

        foreach (var shapeType in Enum.GetValues(typeof(ShapeType)))
        {
            var lifetimeList = gameObjectLifetimes[(int) shapeType];
            // Get all index of active shapes
            var indicies = Enumerable.Range(0, lifetimeList.Count)
             .Where(i => lifetimeList[i] == true)
             .ToList();
            foreach(var index in indicies)
            {
                ShapeObject shapeObject = gameObjects[(int) shapeType][index];
                ShapeSaveJson saveShape = new ShapeSaveJson(shapeObject.ShapeType, shapeObject.transform.position, shapeObject.transform.rotation);
                saveJson.SaveList.Add(saveShape);
            }
        }

        string jsonString = JsonUtility.ToJson(saveJson);
        System.IO.File.WriteAllText(path, jsonString);
    }

    public void LoadLevel()
    {
        string path = EditorUtility.OpenFilePanel("Select save file", "", "json");

        //Reset game level
        GameObjectPool.Instance.DeallocateAll();

        string jsonStr = File.ReadAllText(path);
        ShapeSaveJsonList saveJson = JsonUtility.FromJson<ShapeSaveJsonList>(jsonStr);

        foreach(var saveObject in saveJson.SaveList)
        {
            ShapeObject shape = GameObjectPool.Instance.Allocate(saveObject.ShapeType);
            shape.transform.position = saveObject.Position;
            shape.transform.rotation = saveObject.Rotation;
        }
    }

}
