using DG.Tweening;
using EasyTransition;
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SystemButton : MonoBehaviour
{
    [SerializeField] TransitionSettings reloadTransition;
    private Player playerScript;

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
        SEManager.Instance.Play(SEPath.RELOAD, 1, 0, 0.8f);
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

