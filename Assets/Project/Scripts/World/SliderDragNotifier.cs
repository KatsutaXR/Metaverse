using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDragNotifier : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isDragging = false;
    public bool IsDragging { get => _isDragging; set => _isDragging = value; }

    private readonly Subject<Unit> _pointerDowned = new Subject<Unit>();
    public IObservable<Unit> PointerDowned => _pointerDowned;
    private readonly Subject<Unit> _pointerUpped = new Subject<Unit>();
    public IObservable<Unit> PointerUpped => _pointerUpped;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
        _pointerDowned.OnNext(Unit.Default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerUpped.OnNext(Unit.Default);
    }
}
