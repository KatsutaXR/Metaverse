using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AvatarUIView : MonoBehaviour
{
    [SerializeField] GameObject _avatarListUI;
    [SerializeField] Button _avatarListBackButton;
    [SerializeField] Button _avatarListSaveButton;
    [SerializeField] Transform _avatarListScrollViewContent;

    private readonly Subject<AvatarID> _avatarListItemClicked = new Subject<AvatarID>();
    public IObservable<AvatarID> AvatarListItemClicked => _avatarListItemClicked;
    private readonly Subject<Unit> _avatarListBackButtonClicked = new Subject<Unit>();
    public IObservable<Unit> AvatarListBackButtonClicked => _avatarListBackButtonClicked;
    private readonly Subject<Unit> _avatarListSaveButtonClicked = new Subject<Unit>();
    public IObservable<Unit> AvatarListSaveButtonClicked => _avatarListSaveButtonClicked;

    private CompositeDisposable _disposable;

    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        _avatarListBackButton
            .OnClickAsObservable()
            .Subscribe(_ => BackToMainMenu())
            .AddTo(_disposable);

        _avatarListSaveButton
            .OnClickAsObservable()
            .Subscribe(_ => _avatarListSaveButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void Initialize(AvatarData[] avatarDatas, GameObject avatarListItemPrefab)
    {
        CreateAvatarListItems(avatarDatas, avatarListItemPrefab);
    }

    private void CreateAvatarListItems(AvatarData[] avatarDatas, GameObject avatarListItemPrefab)
    {
        // todo:現在のアバターをハイライトする
        foreach (var avatarData in avatarDatas)
        {
            GameObject obj = Instantiate(avatarListItemPrefab, _avatarListScrollViewContent);
            obj.GetComponent<Image>().sprite = avatarData.AvatarImage;
            obj.transform.GetComponentInChildren<TextMeshProUGUI>(true).text = avatarData.AvatarName;
            obj.GetComponent<Button>().onClick.AddListener(() => OnAvatarListItemClicked(avatarData));
        }
    }

    private void OnAvatarListItemClicked(AvatarData avatarData)
    {
        // todo:選択中のアバターをハイライトする
        _avatarListItemClicked.OnNext(avatarData.AvatarID);
    }

    public void GoToAvatarList()
    {
        _avatarListUI.SetActive(true);
    }

    private void BackToMainMenu()
    {
        _avatarListUI.SetActive(false);
        _avatarListBackButtonClicked.OnNext(Unit.Default);
    }
}
