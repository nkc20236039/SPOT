using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

// ライトからxが近い順、yが遠い順に並び変える
// シャドーポイントから伸びる影の終点を求める
// カメラフレームについたらそこ同士をつなげる

public class LightCollider : MonoBehaviour
{
    [SerializeField] LayerMask m_LayerMask;     // レイヤーマスク

    void Start()
    {

    }

    /// <summary>
    /// 光に当たっている角の座標を取得する
    /// </summary>
    /// <param name="lightPosition"></param>
    /// <param name="forwardDirection"></param>
    /// <param name="spotAngle"></param>
    /// <returns></returns>
    public List<GameObject> GetLightHitPoint(Vector2 lightPosition, Vector2 forwardDirection, float spotAngle)
    {
        List<GameObject> brightPoint = new List<GameObject>();
        // shadowPointが光に当たっているかチェックする
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject shadowPoint = transform.GetChild(i).gameObject;

            Vector2 shadowCornerPosition = shadowPoint.transform.position;
            // 比較するベクトル計算
            Vector2 standardVector =
                (lightPosition -
                    (forwardDirection +
                    lightPosition))
                    .normalized;
            Vector2 targetVector =
                (lightPosition - shadowCornerPosition)
                .normalized;

            // ライトまでの角度を求める
            float angle =
                Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                Mathf.Rad2Deg;

            if (angle < spotAngle / 2)
            {
                // 妨げになるオブジェクトがないか調べる
                RaycastHit2D hit = Physics2D.Linecast(lightPosition, shadowCornerPosition, m_LayerMask);

                if (!hit)
                {
                    brightPoint.Add(shadowPoint);
                }
            }
        }

        return brightPoint;
    }

    private void SortShadowPoint(GameObject[] shadowPoint)
    {
        List<Vector2> cornerPosition = new List<Vector2>();

        for (int i = 0; i < shadowPoint.Length; i++)
        {
            // 影オブジェクトを座標に変換
            cornerPosition.Add(shadowPoint[i].transform.position);
        }

        // x座標が
        cornerPosition.Sort((a, b) =>
        {
            if (a.x != b.x)
            {
                return a.x.CompareTo(b.x);
            }
            else
            {
                return -a.y.CompareTo(b.y);
            }
        });
    }
}
