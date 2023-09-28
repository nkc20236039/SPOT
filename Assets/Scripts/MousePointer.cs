using System.Collections;
using System.Collections.Generic;
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
        Vector3 cursorScale = cursorIconRect.localScale;
        cursorScale.x = Mathf.Abs(cursorScale.x);

        // 右クリックされているとき
        if (isRightClick)
        {
            if(canSlide)
            {
                playAnimationIndex = 1;
                cursorScale.x *= direction;
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
        cursorIconRect.localScale = cursorScale;

        animator.SetInteger("CursorIcon", playAnimationIndex);
    }
}
