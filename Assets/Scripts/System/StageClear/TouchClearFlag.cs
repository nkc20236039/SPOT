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
        // ���Ƀv���C���[���G�ꂽ��
        if (collision.gameObject.tag == "Player")
        {
            // �A�j���[�V�����\���L�����o�X��Image��L����
            animatedImageScript.playGif = true;
            playerScript.canPlayerControl = false;
            // ���Đ�
            SEManager.Instance.Play(SEPath.CREAR_AUDIO);

            // ���݂̃V�[�����擾
            string sceneName = SceneManager.GetActiveScene().name;
            // �g�����W�V����
            TransitionManager.Instance().Transition(sceneName, transition, transitionDelay);
        }
    }

    IEnumerator PlayClearAnimation()
    {
        // 

        yield return null;
    }
}
