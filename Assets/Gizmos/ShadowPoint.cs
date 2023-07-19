using UnityEngine;
using System.Collections;
using System;

public class GizmosTest : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0, 0.8f);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 1f);
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}