using UnityEngine;
/// <summary>
/// 各機能で使用したいデータをここにまとめる
/// </summary>
public class PlayerData
{
    private GameObject _player;
    public GameObject Player
    {
        get { return _player; }
        set { _player ??= value; }
    }
    
}
