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
    /// オブジェクトの角が光を浴びているか
    /// </summary>
    /// <returns>bool</returns>
    public bool IsLightHit()
    {
        lightPos = shadowAngleScript.spotLight.transform.position;
        // RayのStartを設定
        Vector3 lightDir = Vector3.ClampMagnitude(lightPos, 0.01f);
        rayStart = transform.position + lightDir;

        // Ray発射
        for(int i = 0;i < lMask.Length; i++)
        {
            result = Physics2D.Linecast(rayStart, lightPos, lMask[i]);
            if(result) break;
        }

        // Ray表示
        if (result) Debug.DrawLine(rayStart, result.point, Color.red);
        else Debug.DrawLine(rayStart, lightPos, Color.magenta);

        return result;
    }
}
