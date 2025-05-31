using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer video;
    void Start()
    {
        video = GetComponent<VideoPlayer>();
        video.loopPointReached += InvokeFaroTest;
    }

    private void Update()
    {

    }
    private void InvokeFaroTest(VideoPlayer source)
    {
        Invoke("FaroTest", 2.0f);
    }

    void FaroTest()
    {
        SceneManager.LoadSceneAsync("FaroTest");
    }
}
