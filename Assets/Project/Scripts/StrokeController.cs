using Fusion;
using UnityEngine;

public class StrokeController : NetworkBehaviour
{
    private LineRenderer _lineRenderer;
    [Networked] public string PenTag { get; set; }

    [Networked, Capacity(1024)]
    private NetworkLinkedList<Vector3> Points { get; }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Spawned()
    {
        base.Spawned();
        // 後から参加した人も線を同期させる
        _lineRenderer.positionCount = Points.Count;

        // 既存のポイントをLineRendererに反映
        for (int i = 0; i < Points.Count; i++)
        {
            _lineRenderer.SetPosition(i, Points[i]);
        }
    }

    public void AddPoint(Vector3 point, float segmentLength)
    {
        if (Points.Count == 0 || Vector3.Distance(Points[Points.Count - 1], point) > segmentLength)
        {
            Points.Add(point);
            _lineRenderer.positionCount = Points.Count;
            _lineRenderer.SetPosition(Points.Count - 1, point);
            RpcSetStrokePosition(Points.Count - 1, point); // 同期
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetStrokePosition(int count, Vector3 point)
    {
        _lineRenderer.positionCount = Points.Count;
        _lineRenderer.SetPosition(count, point);
    }
}
