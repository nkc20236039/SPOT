using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{

    public static DontDestroyObject Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // �q�I�u�W�F�N�g�擾
        Transform[] parentAndChildren = transform.GetComponentsInChildren<Transform>(true);

        foreach(Transform child in parentAndChildren)
        {
            DontDestroyOnLoad(child);
        }
    }
}
