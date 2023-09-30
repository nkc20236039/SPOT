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
        // ���݂̃V�[�����擾
        string sceneName = SceneManager.GetActiveScene().name;
        // �V�[�������[�h
        TransitionManager.Instance().Transition(sceneName, reloadTransition, transitionTime);
        // ������ł��Ȃ��悤�ɂ���
        playerScript.canPlayerControl = false;
        SEManager.Instance.Play(SEPath.RELOAD, 1, 0, 0.8f);
    }

    public void MouseEnter()
    {
        // �}�E�X���t�H�[�J�X���ꂽ�Ƃ��g�傷��
        SEManager.Instance.Play(SEPath.MOUSE_OVER, 0.5f);
        transform.DOScale(1.25f, 0.5f);
    }

    public void MouseExit()
    {
        // �}�E�X���t�H�[�J�X���ꂽ�Ƃ��g�傷��
        transform.DOScale(1f, 0.5f);
    }
}

