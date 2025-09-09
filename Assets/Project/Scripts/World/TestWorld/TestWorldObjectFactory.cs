using UnityEngine;
using VContainer;

public class TestWorldObjectFactory : WorldObjectFactory
{
    private Vector3 _spawnPlayerPosition = Vector3.zero; // プレイヤーの初期位置
    private Quaternion _spawnPlayerRotation = Quaternion.Euler(0, 0, 0); // プレイヤーの初期回転角度
    public override Vector3 SpawnPlayerPosition => _spawnPlayerPosition;
    public override Quaternion SpawnPlayerRotation => _spawnPlayerRotation;
    [Inject]
    public TestWorldObjectFactory(PrefabDatabase prefabDatabase)
    {
        _prefabDatabase = prefabDatabase;
    }
}
