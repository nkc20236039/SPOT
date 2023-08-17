using UnityEngine;

public class ColliderAngle : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool debug = false;

    private GameObject[] shadowPoints;

    void Start()
    {
        UpdateShadowPoints();
    }

    void Update()
    {
        // ライトから角にあるポイントに向かってRayを飛ばす
        foreach (GameObject shadowPoint in shadowPoints)
        {
            Vector3 rayStart = transform.position;
            Vector3 rayEnd = shadowPoint.transform.position;
            float rayHitObjectPosition = Mathf.Infinity;
            float shadowPointPosition = 0;

            RaycastHit2D rayHit = Physics2D.Linecast(rayStart, rayEnd, layerMask);

            if(rayHit.collider != null)
            {
                // 何かに当たったら座標の距離を取得
                rayHitObjectPosition =
                    rayHit.point.magnitude;
                shadowPointPosition =
                    shadowPoint.transform.position.magnitude;
                // Debug.Log($"{rayHitObjectPosition}     {shadowPointPosition}");
            }

            // 当たった場所とポイントがほぼ同じ場所だったら
            if(Mathf.Approximately(rayHitObjectPosition, shadowPointPosition))
            {
                // 当たったらポイントをオンにする
                shadowPoint.SetActive(true);

                // 当たったオブジェクトの角度をライトからの角度に合わせる
                Vector3 shadowPointDir = rayStart - rayEnd;
                shadowPoint.transform.rotation = Quaternion.FromToRotation(Vector3.left, shadowPointDir);
            }
            else
            {
                // 当たらなかったらポイントをオフにする
                shadowPoint.SetActive(false);
            }

            // デバッグ
            if (debug)
            {
                if (Input.GetKey(KeyCode.LeftAlt)) { 
                Debug.Log($"ヒット座標: {rayHit.point} 対象オブジェクト: {shadowPoint.transform.position}"); 
                }

                if (Mathf.Approximately(rayHitObjectPosition, shadowPointPosition))
                {
                    Debug.DrawLine(rayStart, rayEnd, Color.green);
                }
                else
                {
                    Debug.DrawLine(rayStart, rayHit.point, Color.red);
                }
            }
        }
    }

    /// <summary>
    /// 影ポイントを更新
    /// </summary>
    public void UpdateShadowPoints()
    {
        // tag[ShadowPoint]を取得
        shadowPoints = GameObject.FindGameObjectsWithTag("ShadowPoint");
    }
}
