using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIView : MonoBehaviour
{
    [SerializeField] GameObject _profileUI;

    [SerializeField] GameObject _myProfileUI;
    [SerializeField] TMP_InputField _myName;
    [SerializeField] TMP_InputField _mySelfIntroduction;
    [SerializeField] Button _myProfileSaveButton;
    [SerializeField] Button _myProfileBackButton;

    [SerializeField] GameObject _targetProfileUI;
    [SerializeField] TextMeshProUGUI _targetName;
    [SerializeField] TextMeshProUGUI _targetSelfIntroduction;
    [SerializeField] Button _targetProfileBackButton;

    private readonly Subject<ProfileData> _myProfileSaveButtonClicked = new Subject<ProfileData>();
    public IObservable<ProfileData> MyProfileSaveButtonClicked => _myProfileSaveButtonClicked;
    private readonly Subject<Unit> _myProfileBackButtonClicked = new Subject<Unit>();
    public IObservable<Unit> MyProfileBackButtonClicked => _myProfileBackButtonClicked;

    private readonly Subject<Unit> _targetProfileBackButtonClicked = new Subject<Unit>();
    public IObservable<Unit> TargetProfileBackButtonClicked => _targetProfileBackButtonClicked;
    private CompositeDisposable _disposable;

    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        _myProfileSaveButton
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                var profileData = new ProfileData();
                profileData.Name = _myName.text;
                profileData.SelfIntroduction = _mySelfIntroduction.text;
                _myProfileSaveButtonClicked.OnNext(profileData);
            })
            .AddTo(_disposable);

        _myProfileBackButton
            .OnClickAsObservable()
            .Subscribe(_ => BackToMainMenu())
            .AddTo(_disposable);

        _targetProfileBackButton
            .OnClickAsObservable()
            .Subscribe(_ => CloseTargetProfileUI()) // todo:処理の記載
            .AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void GoToMyProfileUI(ProfileData profileData)
    {
        _profileUI.SetActive(true);
        _myProfileUI.SetActive(true);

        _myName.text = profileData.Name;
        _mySelfIntroduction.text = profileData.SelfIntroduction;
    }

    public void GoToTargetProfileUI(ProfileData profileData)
    {
        _profileUI.SetActive(true);
        _targetProfileUI.SetActive(true);

        _targetName.text = profileData.Name;
        _targetSelfIntroduction.text = profileData.SelfIntroduction;
    }

    private void BackToMainMenu()
    {
        _profileUI.SetActive(false);
        _myProfileUI.SetActive(false);

        _myName.text = "";
        _mySelfIntroduction.text = "";
        _myProfileBackButtonClicked.OnNext(Unit.Default);
    }

    /// <summary>
    /// _targetProfileBackButton押下時にTargetProfileUIを閉じる
    /// UI表示中はSortOrderの優先順位により、他の画面の操作を行えないようにしている
    /// これにより閉じたときに前の画面が表示されるように見える
    /// </summary>
    private void CloseTargetProfileUI()
    {
        // 前回の画面がMyProfileUIならprofileUIはそのままにしておく
        if (!_myProfileUI.activeSelf) _profileUI.SetActive(false);

        _targetProfileUI.SetActive(false);
        _targetName.text = "";
        _targetSelfIntroduction.text = "";
    }
}
