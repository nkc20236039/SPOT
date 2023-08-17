using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public class LightCollider : MonoBehaviour
{
    [Header("ÉâÉCÉgê›íË")]
    [SerializeField, Range(0.0f, 180.0f)] private float m_spotAngle;
    [SerializeField, Range(0.0f, 359.9f)] private float m_spotDirection;
    [SerializeField] private float m_spotDistance;
    [Space]
    [Header("Ray")]
    [SerializeField] private bool debug = false;
    [SerializeField] private LayerMask layerMask;


    void Start()
    {
        
    }

    void Update()
    {
        

    }
}
