using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCheck : MonoBehaviour
{
    private Vector3 rayStart;
    private Vector3 lightPos;
    private ShadowAngle shadowAngleScript;
    [SerializeField] private LayerMask[] lMask;
    RaycastHit2D result;

    void Start()
    {
        shadowAngleScript = GetComponent<ShadowAngle>();
    }

    /// <summary>
    /// �I�u�W�F�N�g�̊p�����𗁂тĂ��邩
    /// </summary>
    /// <returns>bool</returns>
    public bool IsLightHit()
    {
        lightPos = shadowAngleScript.spotLight.transform.position;
        // Ray��Start��ݒ�
        Vector3 lightDir = Vector3.ClampMagnitude(lightPos, 0.01f);
        rayStart = transform.position + lightDir;

        // Ray����
        for(int i = 0;i < lMask.Length; i++)
        {
            result = Physics2D.Linecast(rayStart, lightPos, lMask[i]);
            if(result) break;
        }

        // Ray�\��
        if (result) Debug.DrawLine(rayStart, result.point, Color.red);
        else Debug.DrawLine(rayStart, lightPos, Color.magenta);

        return result;
    }
}
