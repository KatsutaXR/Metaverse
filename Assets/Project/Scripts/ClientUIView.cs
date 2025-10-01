using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIView : MonoBehaviour
{
    [SerializeField] private GameObject _clientUI;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private Button _profileButton;
    [SerializeField] private Button _avatarButton;
    [SerializeField] private Button _respawnButton;
    private readonly Subject<Unit> _lobbyButtonClicked = new Subject<Unit>();
    public IObservable<Unit> LobbyButtonClicked => _lobbyButtonClicked;
    private readonly Subject<Unit> _profileButtonClicked = new Subject<Unit>();
    public IObservable<Unit> ProfileButtonClicked => _profileButtonClicked;
    private readonly Subject<Unit> _avatarButtonClicked = new Subject<Unit>();
    public IObservable<Unit> AvatarButtonClicked => _avatarButtonClicked;
    private readonly Subject<Unit> _respawnButtonClicked = new Subject<Unit>();
    public IObservable<Unit> RespawnButtonClicked => _respawnButtonClicked;
    private readonly Subject<Unit> _updateTransformRequested = new Subject<Unit>();
    public IObservable<Unit> UpdateTransformRequested => _updateTransformRequested;

    private CompositeDisposable _disposable;
    private void Update()
    {
        if (!_clientUI.activeSelf) return;

        _updateTransformRequested.OnNext(Unit.Default);
    }
    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        _lobbyButton
            .OnClickAsObservable()
            .Subscribe(_ => _lobbyButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        _profileButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                _mainMenu.SetActive(false);
                _profileButtonClicked.OnNext(Unit.Default);
            })
            .AddTo(_disposable);

        _avatarButton
            .OnClickAsObservable()
            .Subscribe(_ => _avatarButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        _respawnButton
            .OnClickAsObservable()
            .Subscribe(_ => _respawnButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);
    }

    // todo:再度UIを開いたときに各UIの状態を初期に戻したい
    public void ToggleClientUI(Vector3 position, Quaternion rotation)
    {
        _clientUI.transform.SetPositionAndRotation(position, rotation);
        _clientUI.SetActive(!_clientUI.activeSelf);
    }

    public void UpdateClientUiTransform(Vector3 position, Quaternion rotation)
    {
        _clientUI.transform.SetPositionAndRotation(position, rotation);
    }


    public void BackToMainMenu()
    {
        _mainMenu.SetActive(true);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
