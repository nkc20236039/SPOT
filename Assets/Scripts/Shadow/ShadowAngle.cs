using UnityEngine;

public class ShadowAngle : MonoBehaviour
{
    private LightCheck lightCheckScript;
    private Collider2D shadowCollider;
    public GameObject spotLight { get; private set; }

    void Start()
    {
        lightCheckScript = GetComponent<LightCheck>();
        shadowCollider = GetComponent<Collider2D>();
        spotLight = GameObject.FindWithTag("Light");
    }

    void Update()
    {
        // ライトの光が当たらなければコライダーをオフにする
        shadowCollider.enabled = !lightCheckScript.IsLightHit();

        // 影が有効なら影の淵にコライダーをつける
        if (shadowCollider.enabled )
        {
            Vector3 lightDir = spotLight.transform.position - transform.position;
            // ライトのある方向を向く
            transform.rotation = Quaternion.FromToRotation(Vector3.left, lightDir);
        }
    }
}
