using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class VideoModel
{
    public List<VideoData> Playlist { get; private set; } = new();

    public void Initialize()
    {
        LoadPlaylist();
    }

    private void LoadPlaylist()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "Videos");
        if (!Directory.Exists(path)) return;

        var files = Directory.GetFiles(path, "*.mp4")
            .OrderBy(Path.GetFileNameWithoutExtension)
            .Select(f => new VideoData
            {
                Name = Path.GetFileNameWithoutExtension(f),
                Url = f
            });

        Playlist = files.ToList();
    }

    public int GetPreviousVideoIndex(int index)
    {
        int prevIndex;

        if (index == 0) prevIndex = Playlist.Count - 1;
        else prevIndex = --index;

        return prevIndex;
    }

    public int GetNextVideoIndex(int index)
    {
        int nextIndex;

        if (index == Playlist.Count - 1) nextIndex = 0;
        else nextIndex = ++index;

        return nextIndex;
    }

    /// <summary>
    /// 動画終了時の次の動画を決める関数
    /// Loop > Randomの優先順位とする
    /// </summary>
    public int DecideNextVideoByState(int currentIndex, bool random, bool loop, List<int> videoHistory)
    {
        if (loop) return currentIndex;

        if (random)
        {
            List<int> unplayed = new List<int>();
            for (int i = 0; i < Playlist.Count; i++)
            {
                if (!videoHistory.Contains(i)) unplayed.Add(i);
            }
            return unplayed[Random.Range(0, unplayed.Count)];
        }

        return GetNextVideoIndex(currentIndex);
    }
}
