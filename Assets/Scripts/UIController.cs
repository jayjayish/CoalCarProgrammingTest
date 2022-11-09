using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoSingleton<UIController>
{
    [SerializeField] 
    private GameObject m_MenuObject;

    [SerializeField]
    private float k_MenuSpawnOffset = 5f;

    private bool m_MenuActive = false;
    public bool MenuActive => m_MenuActive;
    // Start is called before the first frame update
    private void Start()
    {
        m_MenuObject.SetActive(false);
    }

/*     // Update is called once per frame
    void Update()
    {
        
    } */


    public void SetMenuActive(bool active)
    {
        m_MenuActive = active;
        m_MenuObject.SetActive(m_MenuActive);

        if (m_MenuActive)
        {
            Transform cameraTransform = ControllerManager.Instance.GetCameraTransform();
            m_MenuObject.transform.position = cameraTransform.position + cameraTransform.forward * k_MenuSpawnOffset;
            m_MenuObject.transform.rotation = cameraTransform.rotation;
        }
    }
}
