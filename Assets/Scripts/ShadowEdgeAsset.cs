using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ShadowEdge")]
public class ShadowEdgeAsset : ScriptableObject
{
    public LayerMask indexLayerMask;    // ��ʃt���[���̃��C���[�}�X�N
    public LayerMask objectLayerMask;          // 
    public bool debug = true;
    public float radiusAllow;
}
