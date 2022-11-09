using UnityEngine;

public class MonoSingleton<DerivedType> : MonoBehaviour where DerivedType : MonoSingleton<DerivedType>
{
    public static DerivedType Instance
    {
        get
        {
            return s_Instance;
        }
    }
    protected static DerivedType s_Instance = null;

    private static readonly string k_DerivedName = typeof(DerivedType).Name;
    private static readonly string k_LogPrefix = "[MonoSingleton<" + k_DerivedName + ">]";

    protected virtual void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_Instance = (DerivedType)this;
    }
}