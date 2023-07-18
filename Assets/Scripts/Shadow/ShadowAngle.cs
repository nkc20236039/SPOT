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
            Vector3 lightDir = transform.position - transform.position;
            // Quaternion(回転値)を取得
            Quaternion quaternion = Quaternion.LookRotation(lightDir);
            // 算出した回転値をこのゲームオブジェクトのrotationに代入
            transform.rotation = quaternion;
        }
    }
}
