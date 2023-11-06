using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer video;
    void Awake()
    {
        video = GetComponent<VideoPlayer>();
    }
    void OnEnable()
    {
        video.loopPointReached += EndVideoTwo;
    }
    

    void EndVideoTwo(VideoPlayer video)
    {
        BasicData.levelSwitch[33] = true;
        Debug.Log("视频播放结束");
    }
    
}
