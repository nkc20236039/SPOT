using DG.Tweening;
using EasyTransition;
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
    private AudioSource reloadAudio;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
        reloadAudio = GetComponent<AudioSource>();
    }

    public void Reload(float transitionTime)
    {
        // 現在のシーンを取得
        string sceneName = SceneManager.GetActiveScene().name;
        // シーンリロード
        TransitionManager.Instance().Transition(sceneName, reloadTransition, transitionTime);
        // 操作をできないようにする
        playerScript.canPlayerControl = false;
        reloadAudio.Play();
    }

    public void MouseEnter()
    {
        // マウスがフォーカスされたとき拡大する
        transform.DOScale(1.25f, 0.5f);
    }

    public void MouseExit()
    {
        // マウスがフォーカスされたとき拡大する
        transform.DOScale(1f, 0.5f);
    }
}

