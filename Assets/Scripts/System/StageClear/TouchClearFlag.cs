using EasyTransition;
using KanKikuchi.AudioManager;
using System.Collections;
using UnityEngine;

public class TouchClearFlag : MonoBehaviour
{
    [SerializeField] private GameObject animationCanvas;
    public static GameObject animatedImage;
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float transitionDelay;

    private Player playerScript;
    public static int Stage = 1;

    private void Start()
    {
        if (animatedImage == null)
        {
            animatedImage = animationCanvas.transform.Find("Image").gameObject;
        }
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 旗にプレイヤーが触れたら
        if (collision.gameObject.tag == "Player")
        {
            // アニメーション表示キャンバスのImageを有効化
            animatedImage.SetActive(true);
            playerScript.canPlayerControl = false;
            // 音再生
            SEManager.Instance.Play(SEPath.CREAR_AUDIO, delay: 0.1f);

            // 現在のシーンを取得
            Stage++;
            string sceneName = "Stage" + Stage.ToString();
            // トランジション
            TransitionManager.Instance().Transition(sceneName, transition, transitionDelay);
        }
    }
}
