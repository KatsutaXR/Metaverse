using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StrokeController : NetworkBehaviour
{
    [Networked]
    public int StrokeCount { get; set; } = 0;
    [SerializeField]
    private GameObject _strokePrefab;
    private Dictionary<int, LineRenderer> _activeStrokes = new();
    private Dictionary<int, List<Vector3>> _strokePoints = new();

    // 座標の範囲設定（±50m）
    private const float RANGE = 100f;
    private const float HALF_RANGE = RANGE / 2f;
    // 1回で送る点の数
    private const int CHUNK_SIZE = 50;

    /// <summary>
    /// クライアントが参加したときに呼ばれる
    /// 状態権限を持つクライアントが線を同期する
    /// 状態権限を持つクライアントがいない場合はマスタークライアントが同期する
    /// Rpcは一度に512byteまでしか送れないため、50点ずつ送る
    /// </summary>
    /// <param name="targetPlayer"></param>
    public void SyncStrokes(PlayerRef targetPlayer)
    {
        if (Object.StateAuthority == PlayerRef.None)
        {
            if (!Runner.IsSharedModeMasterClient) return;
        }
        else
        {
            if (!Object.HasStateAuthority) return;
        }

        foreach (var stroke in _strokePoints)
        {
            var points = stroke.Value;
            for (int i = 0; i < points.Count; i += CHUNK_SIZE)
            {
                int count = Mathf.Min(CHUNK_SIZE, points.Count - i);
                var chunk = points.GetRange(i, count);
                byte[] compressedPoints = CompressPoints(chunk);
                RpcSyncStrokes(targetPlayer, stroke.Key, compressedPoints);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcSyncStrokes([RpcTarget] PlayerRef targetPlayer, int strokeId, byte[] compressedPoints)
    {
        // var decompressedPoints = DecompressPoints(compressedPoints);
        // _strokePoints[strokeId] = decompressedPoints;
        // _activeStrokes[strokeId] = CreateStroke(decompressedPoints);

        var decompressedPoints = DecompressPoints(compressedPoints);

        if (_strokePoints.ContainsKey(strokeId)) _strokePoints[strokeId].AddRange(decompressedPoints);
        else _strokePoints[strokeId] = decompressedPoints;

        if (_activeStrokes.ContainsKey(strokeId)) AddPointsToLineRenderer(_activeStrokes[strokeId], decompressedPoints);
        else _activeStrokes[strokeId] = CreateStroke(decompressedPoints);
    }

    public void AddPoint(bool isNewStroke, Vector3 point)
    {
        if (isNewStroke) StrokeCount++;

        if (!_strokePoints.ContainsKey(StrokeCount)) _strokePoints[StrokeCount] = new List<Vector3>();
        _strokePoints[StrokeCount].Add(point);

        if (!_activeStrokes.ContainsKey(StrokeCount)) _activeStrokes[StrokeCount] = CreateStroke();
        var lineRenderer = _activeStrokes[StrokeCount];
        lineRenderer.positionCount = _strokePoints[StrokeCount].Count;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

        RpcAddPoint(StrokeCount, point);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcAddPoint(int strokeId, Vector3 point)
    {
        // 二重の処理を防ぐ
        if (Object.HasStateAuthority) return;

        if (!_strokePoints.ContainsKey(strokeId)) _strokePoints[strokeId] = new List<Vector3>();
        _strokePoints[strokeId].Add(point);

        if (!_activeStrokes.ContainsKey(strokeId)) _activeStrokes[strokeId] = CreateStroke();
        var lineRenderer = _activeStrokes[strokeId];
        lineRenderer.positionCount = _strokePoints[strokeId].Count;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);
    }

    private LineRenderer CreateStroke(List<Vector3> points = null)
    {
        var stroke = Instantiate(_strokePrefab);
        var lineRenderer = stroke.GetComponent<LineRenderer>();
        
        if (points != null)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        return lineRenderer;
    }

    private void AddPointsToLineRenderer(LineRenderer targetLineRenderer, List<Vector3> points)
    {
        if (points != null)
        {
            foreach (var point in points)
            {
                targetLineRenderer.positionCount++;
                targetLineRenderer.SetPosition(targetLineRenderer.positionCount - 1, point);
            }
        }
    }

    public void DeleteStrokes()
    {
        foreach (var stroke in _activeStrokes)
        {
            Destroy(stroke.Value);
        }

        StrokeCount = 0;
        _strokePoints.Clear();
        _activeStrokes.Clear();
        RpcDeleteStrokes(Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcDeleteStrokes(PlayerRef player)
    {
        // 二重の処理を防ぐ
        if (player == Runner.LocalPlayer) return;

        foreach (var stroke in _activeStrokes)
        {
            Destroy(stroke.Value);
        }

        _strokePoints.Clear();
        _activeStrokes.Clear();
    }

    /// <summary>
    /// List<Vector3> を byte[] に圧縮
    /// </summary>
    public static byte[] CompressPoints(List<Vector3> points)
    {
        // short は 2バイト * 3成分
        byte[] data = new byte[points.Count * sizeof(short) * 3];
        int offset = 0;

        foreach (var p in points)
        {
            short sx = FloatToShort(p.x);
            short sy = FloatToShort(p.y);
            short sz = FloatToShort(p.z);

            Buffer.BlockCopy(BitConverter.GetBytes(sx), 0, data, offset, sizeof(short));
            offset += sizeof(short);
            Buffer.BlockCopy(BitConverter.GetBytes(sy), 0, data, offset, sizeof(short));
            offset += sizeof(short);
            Buffer.BlockCopy(BitConverter.GetBytes(sz), 0, data, offset, sizeof(short));
            offset += sizeof(short);
        }

        return data;
    }

    /// <summary>
    /// byte[] を List<Vector3> に復元
    /// </summary>
    public static List<Vector3> DecompressPoints(byte[] data)
    {
        List<Vector3> points = new();
        int offset = 0;

        while (offset < data.Length)
        {
            short sx = BitConverter.ToInt16(data, offset);
            offset += sizeof(short);
            short sy = BitConverter.ToInt16(data, offset);
            offset += sizeof(short);
            short sz = BitConverter.ToInt16(data, offset);
            offset += sizeof(short);

            float x = ShortToFloat(sx);
            float y = ShortToFloat(sy);
            float z = ShortToFloat(sz);

            points.Add(new Vector3(x, y, z));
        }

        return points;
    }

    // float → short (−50m～+50m を −32768～32767 にマッピング)
    private static short FloatToShort(float value)
    {
        float clamped = Mathf.Clamp(value, -HALF_RANGE, HALF_RANGE);
        return (short)(clamped / HALF_RANGE * short.MaxValue);
    }

    // short → float
    private static float ShortToFloat(short value)
    {
        return value / (float)short.MaxValue * HALF_RANGE;
    }
    
}
