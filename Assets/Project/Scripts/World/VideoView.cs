using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoView : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private AudioSource _audioSource;
    // 押下時に前面UI表示のオンオフを設定する
    [SerializeField] private Button _displayButton;
    [SerializeField] private Button _frontUIBackgroundButton;
    [SerializeField] private GameObject _frontUI;

    [SerializeField] private TextMeshProUGUI _videoTitle;

    [SerializeField] private Transform _playlistScrollViewContent;

    [SerializeField] private Slider _seekBarSlider;
    [SerializeField] private SliderDragNotifier _seekBarDragNotifier;
    [SerializeField] private TextMeshProUGUI _currentVideoTime;
    [SerializeField] private TextMeshProUGUI _videoLength;

    [SerializeField] private Button _randomButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _playPuaseButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _loopButton;
    [SerializeField] private Image _randomImage;
    [SerializeField] private Image _playPauseImage;
    [SerializeField] private Image _loopImage;
    [SerializeField] private Sprite _playSprite;
    [SerializeField] private Sprite _pauseSprite;
    private readonly Subject<Unit> _randomButtonClicked = new Subject<Unit>();
    public IObservable<Unit> RnadomButtonClicked => _randomButtonClicked;
    private readonly Subject<Unit> _previousButtonClicked = new Subject<Unit>();
    public IObservable<Unit> PreviousButtonClicked => _previousButtonClicked;
    private readonly Subject<Unit> _playPauseButtonClicked = new Subject<Unit>();
    public IObservable<Unit> PlayPauseButtonClicked => _playPauseButtonClicked;
    private readonly Subject<Unit> _nextButtonClicked = new Subject<Unit>();
    public IObservable<Unit> NextButtonClicked => _nextButtonClicked;
    private readonly Subject<Unit> _loopButtonClicked = new Subject<Unit>();
    public IObservable<Unit> LoopButtonClicked => _loopButtonClicked;

    private readonly Subject<int> _playlistItemClicked = new Subject<int>();
    public IObservable<int> PlaylistItemClicked => _playlistItemClicked;

    private readonly Subject<float> _seekBarValueUpdated = new Subject<float>();
    public IObservable<float> SeekBarValueUpdated => _seekBarValueUpdated;

    private readonly Subject<Unit> _videoEnded = new Subject<Unit>();
    public IObservable<Unit> VideoEnded => _videoEnded;

    [SerializeField] private Slider _volumeSlider;
    private bool _isSeekBarValueUpdatePublishing = false;
    private CompositeDisposable _disposable;

    private void Start()
    {
        _frontUI.SetActive(false);
        _volumeSlider.value = _audioSource.volume;

        _displayButton.onClick.AddListener(() => _frontUI.SetActive(true));
        _frontUIBackgroundButton.onClick.AddListener(() => _frontUI.SetActive(false));
        _volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        _videoPlayer.loopPointReached += OnVideoEnded;
    }

    private void Update()
    {
        TimeSpan ts = TimeSpan.FromSeconds(_videoPlayer.time);

        string formatted = ts.ToString(@"hh\:mm\:ss");
        _currentVideoTime.text = formatted;

        ts = TimeSpan.FromSeconds(_videoPlayer.length);
        formatted = ts.ToString(@"hh\:mm\:ss");
        _videoLength.text = formatted;

        if (_seekBarDragNotifier.IsDragging || _isSeekBarValueUpdatePublishing) return;
        if (_videoPlayer.time == 0 || _videoPlayer.length == 0) _seekBarSlider.value = 0;
        else _seekBarSlider.value = (float)(_videoPlayer.time / _videoPlayer.length);
    }

    private void OnEnable()
    {
        _disposable = new CompositeDisposable();

        _randomButton
            .OnClickAsObservable()
            .Subscribe(_ => OnRnadomButtonClicked())
            .AddTo(_disposable);

        _previousButton
            .OnClickAsObservable()
            .Subscribe(_ => OnPreviousButtonClicked())
            .AddTo(_disposable);

        _playPuaseButton
            .OnClickAsObservable()
            .Subscribe(_ => OnPlayPauseButtonClicked())
            .AddTo(_disposable);

        _nextButton
            .OnClickAsObservable()
            .Subscribe(_ => OnNextButtonClicked())
            .AddTo(_disposable);

        _loopButton
            .OnClickAsObservable()
            .Subscribe(_ => OnLoopButtonClicked())
            .AddTo(_disposable);

        _seekBarDragNotifier
            .PointerDowned
            .Subscribe(_ => OnSeekBarPointerDowned())
            .AddTo(_disposable);

        _seekBarDragNotifier
            .PointerUpped
            .Subscribe(_ => OnSeekBarPointerUpped())
            .AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void Initialize(List<VideoData> videoDatas, GameObject playlistItemPrefab)
    {
        CreatePlaylistItems(videoDatas, playlistItemPrefab);
    }

    private void CreatePlaylistItems(List<VideoData> videoDatas, GameObject playlistItemPrefab)
    {
        for (int i = 0; i < videoDatas.Count; i++)
        {
            // ラムダ式内で参照渡しになるため、iを一時的にコピーする
            int index = i;
            GameObject obj = Instantiate(playlistItemPrefab, _playlistScrollViewContent);
            obj.transform.Find("PlaylistNumber").GetComponent<TextMeshProUGUI>().text = (index + 1).ToString();
            obj.transform.Find("VideoName").GetComponent<TextMeshProUGUI>().text = videoDatas[index].Name;
            obj.GetComponent<Button>().onClick.AddListener(() => OnPlaylistItemClicked(index));
        }
    }

    private void OnVolumeSliderValueChanged(float value)
    {
        _audioSource.volume = value;
    }

    private void OnPlaylistItemClicked(int index)
    {
        _playlistItemClicked.OnNext(index);
    }

    private void OnRnadomButtonClicked()
    {
        _randomButtonClicked.OnNext(Unit.Default);
    }

    private void OnPreviousButtonClicked()
    {
        _previousButtonClicked.OnNext(Unit.Default);
    }

    private void OnPlayPauseButtonClicked()
    {
        _playPauseButtonClicked.OnNext(Unit.Default);
    }

    private void OnNextButtonClicked()
    {
        _nextButtonClicked.OnNext(Unit.Default);
    }

    private void OnLoopButtonClicked()
    {
        _loopButtonClicked.OnNext(Unit.Default);
    }

    private void OnSeekBarPointerDowned()
    {
        
    }

    private void OnSeekBarPointerUpped()
    {
        _seekBarValueUpdated.OnNext(_seekBarSlider.value);
        // OnPointerUpのタイミングでIsDragging = falseにするとイベントが届く前にUpdateが来てしまうためここで記載
        _seekBarDragNotifier.IsDragging = false;
        _isSeekBarValueUpdatePublishing = true;
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        _videoEnded.OnNext(Unit.Default);
    }

    public void SetRandom(bool random)
    {
        if (random)
        {
            _randomImage.color = Color.cyan;
        }
        else
        {
            _randomImage.color = Color.white;
        }
    }

    public void SetVideoContent(string path, string title)
    {
        _videoTitle.text = title;
        _videoPlayer.url = path;
        _videoPlayer.Play();
    }

    public void SetPlayState(bool isPlaying)
    {
        if (isPlaying)
        {
            _playPauseImage.sprite = _pauseSprite;
            _videoPlayer.Play();
        }
        else
        {
            _playPauseImage.sprite = _playSprite;
            _videoPlayer.Pause();
        }
    }

    public void SetLoop(bool loop)
    {
        if (loop)
        {
            _loopImage.color = Color.cyan;
        }
        else
        {
            _loopImage.color = Color.white;
        }

        _videoPlayer.isLooping = loop;
    }

    public async UniTask SetVideoTime(float seekBarValue)
    {
        _videoPlayer.time = _videoPlayer.length * seekBarValue;
        // VideoPlayer.timeの反映に時間がかかるため少し待つ
        await UniTask.WaitForSeconds(1f);
        _isSeekBarValueUpdatePublishing = false;
    }

    public float GetCurrentSeekBarValue()
    {
        return _seekBarSlider.value;
    }
}
