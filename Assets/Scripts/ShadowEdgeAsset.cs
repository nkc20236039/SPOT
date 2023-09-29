using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ShadowEdge")]
public class ShadowEdgeAsset : ScriptableObject
{
    public LayerMask indexLayerMask;    // 画面フレームのレイヤーマスク
    public LayerMask objectLayerMask;          // 
    public bool debug = true;
    public float radiusAllow;
}
