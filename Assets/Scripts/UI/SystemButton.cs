using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SystemButton : MonoBehaviour
{


    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

