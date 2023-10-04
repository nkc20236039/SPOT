using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{

    public static DontDestroyObject Instance
    {
        get; private set;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        TouchClearFlag.animatedImage = transform.Find("Image").gameObject;
        if (!BGMManager.Instance.IsPlaying())
        {
            BGMManager.Instance.Play(BGMPath.PEEKABOO, 0.25f);
        }

        // 子オブジェクト取得
        Transform[] parentAndChildren = transform.GetComponentsInChildren<Transform>(true);

        foreach(Transform child in parentAndChildren)
        {
            DontDestroyOnLoad(child);
        }
    }
}
