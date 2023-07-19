using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class IntroHandler : MonoBehaviour
{
    public VideoPlayer MyVideoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        // assign video clip
        MyVideoPlayer.Play();
        StartCoroutine(StartIntro());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("GameScene");
    }
    // krish was here
}
