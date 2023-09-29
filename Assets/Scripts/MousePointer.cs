using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MousePointer : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] RectTransform cursorIconRect;
    [SerializeField] Animator animator;
    private Vector2 MousePos;

    void Start()
    {

    }

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
                Input.mousePosition, canvas.worldCamera, out MousePos);
        cursorIconRect.anchoredPosition
             = new Vector2(MousePos.x, MousePos.y);
    }

    public void SetCursorIcon(bool isRightClick,int direction, bool canSlide)
    {

        int playAnimationIndex = 0; // 再生するアニメーション

        // スケール値取得
        Vector3 cursorRotate = cursorIconRect.eulerAngles;
        cursorRotate.z = 90;

        // 右クリックされているとき
        if (isRightClick)
        {
            if(canSlide)
            {
                playAnimationIndex = 1;
                cursorRotate.z *= direction;
                Debug.Log(direction);
            }
            else
            {
                playAnimationIndex = 2;
            }
        }
        else
        {
            playAnimationIndex = 0;
        }

        // スケール適用
        cursorIconRect.eulerAngles = cursorRotate;

        animator.SetInteger("CursorIcon", playAnimationIndex);
    }
}
