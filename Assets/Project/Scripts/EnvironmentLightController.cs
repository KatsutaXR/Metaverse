using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using VContainer;

public class EnvironmentLightController : NetworkBehaviour
{
    [SerializeField] private Light _mainLight;
    [SerializeField] private List<LightingData> _lightingDataList = new List<LightingData>();
    [Networked, OnChangedRender(nameof(ApplyLightingData))] public int CurrentIndex { get; set; }
    [Inject] private WorldNetworkController _worldNetworkController;

    public override void Spawned()
    {
        if (Object.HasStateAuthority) CurrentIndex = 0;

        var scope = FindAnyObjectByType<WorldLifetimeScope>();
        scope.Container.Inject(this);

        WaitWorldInitialize().Forget();
    }

    // ワールドの初期化完了を待ってからライトを同期する
    private async UniTask WaitWorldInitialize()
    {
        await UniTask.WaitUntil(() => _worldNetworkController.IsInitializeCompleted);
        ApplyLightingData();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcNextLighting()
    {
        if (_lightingDataList == null || _lightingDataList.Count == 0)
        {
            Debug.LogWarning("_lightingDataList are empty!");
            return;
        }

        CurrentIndex = (CurrentIndex + 1) % _lightingDataList.Count;
    }

    private void ApplyLightingData()
    {
        var lightingData = _lightingDataList[CurrentIndex];
        if (lightingData.Skybox != null) RenderSettings.skybox = lightingData.Skybox;

        RenderSettings.ambientMode = lightingData.AmbientSource;
        RenderSettings.ambientLight = lightingData.AmbientColor;
        RenderSettings.ambientIntensity = lightingData.EnvironmentLightIntensity;

        if (_mainLight != null)
        {
            _mainLight.intensity = lightingData.MainLightIntensity;
            _mainLight.transform.rotation = Quaternion.Euler(lightingData.MainLightRotation);
        }

        // Lightmapping.lightingDataAsset = lightingData.LightingDataAsset;
        if (lightingData.BakedLightingSet != null)
        {
            ApplyBakedLight(lightingData.BakedLightingSet);
        }
        else
        {
            LightmapSettings.lightmaps = new LightmapData[0];
        }
        
        DynamicGI.UpdateEnvironment();
    }

    private void ApplyBakedLight(BakedLightingSet lightingSet)
    {
        int count = lightingSet.lightmapColor.Length;
        LightmapData[] newLightmaps = new LightmapData[count];

        for (int i = 0; i < count; i++)
        {
            var data = new LightmapData();
            data.lightmapColor = lightingSet.lightmapColor[i];
            if (lightingSet.lightmapDir != null && i < lightingSet.lightmapDir.Length)
                data.lightmapDir = lightingSet.lightmapDir[i];

            newLightmaps[i] = data;
        }

        LightmapSettings.lightmaps = newLightmaps;

        Debug.Log($"Switched to lighting set: {lightingSet.lightmapColor.Length} lightmaps");
    }
    
}
