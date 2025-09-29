using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIView : MonoBehaviour
{
    [SerializeField] GameObject _worldUI;

    [SerializeField] GameObject _worldList;
    // [SerializeField] Button _worldListBackButton;
    [SerializeField] Transform _worldListScrollViewContent;
    // private readonly Subject<Unit> _worldListBackButtonClicked = new Subject<Unit>();
    // public IObservable<Unit> WorldListBackButtonClicked => _worldListBackButtonClicked;
    private readonly Subject<WorldID> _worldListItemClicked = new Subject<WorldID>();
    public IObservable<WorldID> WorldListItemClicked => _worldListItemClicked;

    [SerializeField] GameObject _worldDetail;
    [SerializeField] Button _worldDetailBackButton;
    [SerializeField] TextMeshProUGUI _worldDetailWorldName;
    [SerializeField] Image _worldDetailWorldImage;
    [SerializeField] Button _joinSessionButton;
    [SerializeField] Button _createSessionButton;
    [SerializeField] TextMeshProUGUI _worldDetailDescription;
    [SerializeField] Transform _worldDetailScrollViewContent;
    private readonly Subject<SessionInfo> _sessionItemClicked = new Subject<SessionInfo>();
    public IObservable<SessionInfo> SessionItemClicked => _sessionItemClicked;
    private readonly Subject<Unit> _updateSessionItemsRequested = new Subject<Unit>();
    public IObservable<Unit> UpdateSessionItemsRequested => _updateSessionItemsRequested;

    [SerializeField] GameObject _createSessionSetting;
    [SerializeField] TextMeshProUGUI _createSessionName;
    [SerializeField] TextMeshProUGUI _createSessionPassword;
    [SerializeField] Button _createSessionOKButton;
    [SerializeField] Button _createSessionCancelButton;
    private readonly Subject<CustomSessionInfo> _createSessionRequested = new Subject<CustomSessionInfo>();
    public IObservable<CustomSessionInfo> CreateSessionRequested => _createSessionRequested;

    [SerializeField] GameObject _joinSessionSetting;
    [SerializeField] TextMeshProUGUI _joinSessionPassword;
    [SerializeField] Button _joinSessionOKButton;
    [SerializeField] Button _joinSessionCancelButton;
    private readonly Subject<CustomSessionInfo> _joinSessionRequested = new Subject<CustomSessionInfo>();
    public IObservable<CustomSessionInfo> JoinSessionRequested => _joinSessionRequested;

    private WorldData[] _worldDatas;
    private List<GameObject> _sessionItems;
    private CompositeDisposable _disposable;
    private void Start()
    {
        _sessionItems = new List<GameObject>();

        _worldDetailBackButton.onClick.AddListener(BackToWorldList);
        _createSessionButton.onClick.AddListener(GoToCreateSessionSetting);
        _joinSessionButton.onClick.AddListener(GoToJoinSessionSetting);
    }

    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        // _worldListBackButton
        //     .OnClickAsObservable()
        //     .Subscribe(_ => BackToMainMenu())
        //     .AddTo(_disposable);

        _createSessionOKButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                CustomSessionInfo customSessionInfo = new CustomSessionInfo();
                customSessionInfo.SessionName = _createSessionName.text;
                customSessionInfo.Password = _createSessionPassword.text;
                _createSessionRequested.OnNext(customSessionInfo);
            })
            .AddTo(_disposable);

        _createSessionCancelButton
            .OnClickAsObservable()
            .Subscribe(_ => BackToWorldDetail())
            .AddTo(_disposable);

        _joinSessionOKButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                CustomSessionInfo customSessionInfo = new CustomSessionInfo();
                customSessionInfo.Password = _joinSessionPassword.text;
                _joinSessionRequested.OnNext(customSessionInfo);
            })
            .AddTo(_disposable);

        _joinSessionCancelButton
            .OnClickAsObservable()
            .Subscribe(_ => BackToWorldDetail())
            .AddTo(_disposable);
    }
    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void CreateWorldListItems(WorldData[] worldDatas, GameObject worldListItemPrefab)
    {
        _worldDatas = worldDatas;
        foreach (var worldData in _worldDatas)
        {
            GameObject obj = Instantiate(worldListItemPrefab, _worldListScrollViewContent);
            obj.GetComponent<Image>().sprite = worldData.WorldImage;
            obj.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = worldData.WorldName;
            obj.GetComponent<Button>().onClick.AddListener(() => OnWorldListItemClicked(worldData));
        }
    }

    private void OnWorldListItemClicked(WorldData worldData)
    {
        GoToWorldDetail();

        _worldDetailWorldName.text = worldData.WorldName;
        _worldDetailWorldImage.sprite = worldData.WorldImage;
        _worldDetailDescription.text = worldData.WorldDescription;

        _worldListItemClicked.OnNext(worldData.WorldID);
    }

    public void UpdateSessionItems(List<SessionInfo> sessionInfos, GameObject sessionItemPrefab)
    {
        // 更新時に前回のセッションアイテムは削除する
        if (_sessionItems.Count != 0)
        {
            foreach (var sessionItem in _sessionItems)
            {
                Destroy(sessionItem);
            }
            _sessionItems.Clear();
        }

        foreach (var sessionInfo in sessionInfos)
        {
            GameObject obj = Instantiate(sessionItemPrefab, _worldDetailScrollViewContent);
            obj.transform.Find("SessionName").GetComponent<TextMeshProUGUI>().text = sessionInfo.Name;
            obj.transform.Find("NumberOfPeople").GetComponent<TextMeshProUGUI>().text = $"{sessionInfo.PlayerCount} / {sessionInfo.MaxPlayers}";
            obj.GetComponent<Button>().onClick.AddListener(() => OnSessionItemClicked(sessionInfo));
            _sessionItems.Add(obj);
        }
    }

    private void OnSessionItemClicked(SessionInfo sessionInfo)
    {
        Debug.Log($"Name = {sessionInfo.Name}, WorldID = {sessionInfo.Properties["WorldID"]}, Password = {sessionInfo.Properties["Password"]}");
        _sessionItemClicked.OnNext(sessionInfo);
        _joinSessionButton.interactable = true;
    }

    private void GoToJoinSessionSetting()
    {
        _worldDetail.SetActive(false);
        _joinSessionSetting.SetActive(true);
    }

    private void GoToCreateSessionSetting()
    {
        _worldDetail.SetActive(false);
        _createSessionSetting.SetActive(true);
    }

    // public void GoToWorldUI()
    // {
    //     _worldUI.SetActive(true);
    //     _worldList.SetActive(true);
    // }

    private void GoToWorldDetail()
    {
        _worldList.SetActive(false);
        _worldDetail.SetActive(true);

        _joinSessionButton.interactable = false;
    }

    // private void BackToMainMenu()
    // {
    //     _worldList.SetActive(false);
    //     _worldListBackButtonClicked.OnNext(Unit.Default);
    // }

    private void BackToWorldList()
    {
        _worldDetail.SetActive(false);
        _worldList.SetActive(true);
    }

    /// <summary>
    /// "WorldDetail"に戻る際に各セッションを更新する
    /// </summary>
    private void BackToWorldDetail()
    {
        _createSessionSetting.SetActive(false);
        _joinSessionSetting.SetActive(false);
        _worldDetail.SetActive(true);

        _joinSessionButton.interactable = false;

        _updateSessionItemsRequested.OnNext(Unit.Default);
    }
    
    
    // todo:UIの状態をリセットする処理を作る

}
