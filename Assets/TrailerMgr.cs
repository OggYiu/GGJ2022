using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class ClipInfo
{
    public GameObject clipObject;
    public bool isVideo;
    public float from;
    public float to;
    public float duration => to - from;
}

public class TrailerMgr : MonoBehaviour
{
    [SerializeField]
    private List<ClipInfo> clipInfos = new List<ClipInfo>();

    [SerializeField]
    private ClipInfo currentClipInfo;

    [SerializeField]
    private int currentClipIndex = -1;

    [SerializeField]
    private float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var clipInfo in clipInfos)
        {
            clipInfo.clipObject.gameObject.SetActive(false);    
        }
        currentClipIndex = -1;
        NextClip();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentClipInfo != null)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if(timeLeft <= 0)
                {
                    timeLeft = 0;
                    NextClip();
                }
            }
            else if(currentClipInfo.isVideo)
            {
                var videoPlayer = currentClipInfo.clipObject.GetComponent<VideoPlayer>();
                if(!videoPlayer.isPlaying)
                {
                    timeLeft = 0;
                    NextClip();
                }
            }
        }
    }

    void NextClip()
    {
        if(currentClipInfo != null)
        {
            currentClipInfo.clipObject.SetActive(false);
        }

        ++currentClipIndex;

        if(currentClipIndex >= clipInfos.Count)
        {
            return;
        }

        currentClipInfo = clipInfos[currentClipIndex];

        currentClipInfo.clipObject.SetActive(true);
        if (currentClipInfo.isVideo)
        {
            var videoPlayer = currentClipInfo.clipObject.GetComponent<VideoPlayer>();
            videoPlayer.time += currentClipInfo.from;
        }

        timeLeft = currentClipInfo.duration;
    }
}
