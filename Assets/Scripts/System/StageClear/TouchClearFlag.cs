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

        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���Ƀv���C���[���G�ꂽ��
        if (collision.gameObject.tag == "Player")
        {
            playerScript.isPlayed = true;
            // �A�j���[�V�����\���L�����o�X��Image��L����
            animatedImage.SetActive(true);
            playerScript.canPlayerControl = false;
            // ���Đ�
            SEManager.Instance.Play(SEPath.CREAR_AUDIO, delay: 0.2f);
            BGMManager.Instance.Pause();
            // ���݂̃V�[�����擾
            Stage++;
            string sceneName = "Stage" + Stage.ToString();
            // �g�����W�V����
            TransitionManager.Instance().Transition(sceneName, transition, transitionDelay);
        }
    }
}
