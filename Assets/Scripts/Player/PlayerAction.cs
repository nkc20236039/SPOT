using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class Player
{
    private Vector2 velocity;
    
    // パラメーター
    [Header("プレイヤーに影響する力")]
    [SerializeField] private float m_speed;         // プレイヤー測度
    [SerializeField] private float m_jumpForce;     // ジャンプ力
    [SerializeField] private float m_gravityScale;  //　重力の大きさ

    [Header("ライトに関わる変数")]
    [SerializeField] private float pickReach;       // 拾える範囲
    [SerializeField] private Vector3 parentPos;     // 持っている時のプレイヤーからの距離

    /// <summary>
    /// プレイヤーの動く
    /// </summary>
    private void PlayerMove()
    {
        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // 左右移動
        velocity.x = moveInput * m_speed * Time.deltaTime;

        // 斜面だった場合にベクトルを変更する
        velocity = groundStateScript.Slope(velocity);

        // ジャンプ
        if (isJump)
        {
            velocity.y += m_jumpForce;
            isJump = false;
        }

        // 地面にいるとき/いないときの処理
        if (groundStateScript.isGround())
        {
            
        }
        else
        {
            //重力を付ける
            velocity.y -= m_gravityScale * Time.deltaTime;
        }

        // 向きを合わせる
        if(moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;
    }

    /// <summary>
    /// ライトを拾う/置く動作の切り替え
    /// </summary>
    private void SwitchSpotLight()
    {
        // ライトを持っているかチェック
        bool haveLight = GameObject.Find("Light").transform.IsChildOf(transform);

        if (haveLight)
        {
            // ライトを持っている時ライトを置く
            this.gameObject.transform.DetachChildren();
            haveLight = false;
        }
        else
        {
            // ライトを持っていない時の処理
            // 全てのライトをアレイに入れる
            GameObject[] light = GameObject.FindGameObjectsWithTag("EnableLight");
            GameObject nearestObject = null;
            float nearestDistance = pickReach;

            // ライトとプレイヤーの位置の距離を求める
            foreach(GameObject thisObject in light)
            {
                Vector3 distance = thisObject.transform.position - transform.position;
                // 拾える範囲内にライトがあるか調べる
                if(distance.magnitude <= pickReach && distance.magnitude < nearestDistance)
                {
                    nearestObject = thisObject;
                    nearestDistance = distance.magnitude;
                }
            }

            // 拾える範囲内にライトがあったか
            if(nearestObject != null)
            {
                // 親子関係に登録する
                nearestObject.transform.parent = this.transform;
                // 進行方向になるように補正
                parentPos.x = Mathf.Abs(parentPos.x) * Mathf.Sign(transform.localScale.x) * -1;
                nearestObject.transform.localScale = Vector3.one;
                // 座標を変更
                nearestObject.transform.position = transform.position + parentPos;


                haveLight = true;
            }
            else
            {
                // 無かった場合のメッセージを送る
                Debug.Log("近くにライトが無い\n--UI作成時に画面に表示するようにする");
            }
        }
    }
}
