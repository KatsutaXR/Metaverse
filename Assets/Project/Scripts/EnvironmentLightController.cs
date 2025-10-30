using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEditor;
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

        Lightmapping.lightingDataAsset = lightingData.LightingDataAsset;

        DynamicGI.UpdateEnvironment();
    }
    
}
