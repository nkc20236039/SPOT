using DG.Tweening;
using EasyTransition;
using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SystemButton : MonoBehaviour
{
    [Header("リロードボタン")]
    [SerializeField] TransitionSettings reloadTransition;
    [Space]
    [Header("メニューボタン")]
    [SerializeField] Image[] displayImage;

    private Player playerScript;
    private bool isMenuOpen;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void Reload(float transitionTime)
    {
        // 現在のシーンを取得
        string sceneName = SceneManager.GetActiveScene().name;
        // シーンリロード
        TransitionManager.Instance().Transition(sceneName, reloadTransition, transitionTime);
        // 操作をできないようにする
        playerScript.canPlayerControl = false;
        SEManager.Instance.Play(SEPath.RELOAD, delay: transitionTime, pitch: 0.8f);
    }

    public void Menu()
    {
        if (isMenuOpen)
        {

        }
    }

    public void MouseEnter()
    {
        // マウスがフォーカスされたとき拡大する
        SEManager.Instance.Play(SEPath.MOUSE_OVER, 0.5f);
        transform.DOScale(1.25f, 0.5f);
    }

    public void MouseExit()
    {
        // マウスがフォーカスされたとき拡大する
        transform.DOScale(1f, 0.5f);
    }
}

