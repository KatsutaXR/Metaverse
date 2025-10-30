using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class LightingData
{
    public Material Skybox;                          // 空のマテリアル
    public AmbientMode AmbientSource = AmbientMode.Skybox; // 環境光のソース
    public Color AmbientColor = Color.gray;          // 環境光カラー（Flat時に使用）
    [Range(0f, 1f)] public float EnvironmentLightIntensity = 1f; // 明るさ倍率
    [Range(0f, 1f)] public float MainLightIntensity = 1f;
    public Vector3 MainLightRotation = Vector3.zero;
    public LightingDataAsset LightingDataAsset;
}
