using EasyTransition;
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class TouchClearFlag : MonoBehaviour
{
    [SerializeField] private AnimatedImage animatedImageScript; 
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float transitionDelay;

    private Player playerScript;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 旗にプレイヤーが触れたら
        if (collision.gameObject.tag == "Player")
        {
            // アニメーション表示キャンバスのImageを有効化
            animatedImageScript.playGif = true;
            playerScript.canPlayerControl = false;
            // 音再生
            SEManager.Instance.Play(SEPath.CREAR_AUDIO);

            // 現在のシーンを取得
            string sceneName = SceneManager.GetActiveScene().name;
            // トランジション
            TransitionManager.Instance().Transition(sceneName, transition, transitionDelay);
        }
    }

    IEnumerator PlayClearAnimation()
    {
        // 

        yield return null;
    }
}
