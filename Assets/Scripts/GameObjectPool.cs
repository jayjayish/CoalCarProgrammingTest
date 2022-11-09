using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectPool: MonoSingleton<GameObjectPool>
{
    [SerializeField]
    private List<GameObject> m_Prefabs = null;
    [SerializeField]
    private int m_Capacity = 0;
    private List<List<bool>> m_GameObjectLifetimes;
    protected List<List<ShapeObject>> m_GameObjects;
    
    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) 
        {
            return;
        }
        
        if(m_Prefabs.Count == 0)
        {
            Debug.LogError(gameObject.name + " does not have an attached prefab!");
            return;
        }

        //Initialize prefab lists
        Instance.m_GameObjectLifetimes = new List<List<bool>>();
        Instance.m_GameObjects = new List<List<ShapeObject>>();
        for (int i = 0; i < m_Prefabs.Count; i += 1)
        {
            var lifeList = new List<bool>(m_Capacity);
            lifeList.AddRange(Enumerable.Repeat(false, m_Capacity));
            Instance.m_GameObjectLifetimes.Add(lifeList);
            
            var objList = new List<ShapeObject>(m_Capacity);
            for(int j = 0; j < m_Capacity; j += 1)
            {
                GameObject clone = Instantiate(m_Prefabs[i]);
                clone.SetActive(false);
                clone.transform.SetParent(this.transform);
                objList.Add(clone.GetComponent<ShapeObject>());
            }
            Instance.m_GameObjects.Add(objList);
        }
    }

    public ShapeObject Allocate(ShapeController.ShapeType shapeType)
    {
        int index = m_GameObjectLifetimes[(int)shapeType].FindIndex(value => value == false);
        if (index == -1)
        {
            Debug.LogError(gameObject.name + " is full!");
            return null;
        }

        m_GameObjectLifetimes[(int)shapeType][index] = true;
        m_GameObjects[(int)shapeType][index].gameObject.SetActive(true);

        return m_GameObjects[(int)shapeType][index];
    }

    public void Deallocate(ShapeObject value, bool worldPositionStays = true)
    {
        var shapeType = value.ShapeType;

        int index = m_GameObjects[(int)shapeType].FindIndex(listValue => GameObject.ReferenceEquals(value, listValue));
        if (index == -1)
        {
            Debug.LogError(gameObject.name + " double free!");
            return;
        }

        m_GameObjects[(int)shapeType][index].transform.SetParent(this.transform, worldPositionStays);
        m_GameObjects[(int)shapeType][index].gameObject.SetActive(false);
        m_GameObjectLifetimes[(int)shapeType][index] = false;
    }
}
