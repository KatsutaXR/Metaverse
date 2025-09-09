using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIView : MonoBehaviour
{
    [SerializeField] private GameObject _clientUI;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private Button _lobbyButton;
    [SerializeField] private Button _worldButton;
    [SerializeField] private Button _avatarButton;
    [SerializeField] private Button _respawnButton;
    private readonly Subject<Unit> _lobbyButtonClicked = new Subject<Unit>();
    public IObservable<Unit> LobbyButtonClicked => _lobbyButtonClicked;
    private readonly Subject<Unit> _worldButtonClicked = new Subject<Unit>();
    public IObservable<Unit> WorldButtonClicked => _worldButtonClicked;
    private readonly Subject<Unit> _avatarButtonClicked = new Subject<Unit>();
    public IObservable<Unit> AvatarButtonClicked => _avatarButtonClicked;
    private readonly Subject<Unit> _respawnButtonClicked = new Subject<Unit>();
    public IObservable<Unit> RespawnButtonClicked => _respawnButtonClicked;

    private CompositeDisposable _disposable;
    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        _lobbyButton
            .OnClickAsObservable()
            .Subscribe(_ => _lobbyButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        _worldButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                _mainMenu.SetActive(false);
                _worldButtonClicked.OnNext(Unit.Default);
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
    public void ToggleClientUI(Vector3 togglePosition, Quaternion toggleRotation)
    {
        _clientUI.transform.position = togglePosition;
        _clientUI.transform.rotation = toggleRotation;
        _clientUI.SetActive(!_clientUI.activeSelf);
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
