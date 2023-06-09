using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundMgr : MonoBehaviour
{
    private static SoundMgr instance;
    public static SoundMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundMgr>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(SoundMgr), typeof(SoundMgr)).GetComponent<SoundMgr>();
                }
                //DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    [SerializeField] AudioClip[] buttonList = null; //HighLighted - Click
    [SerializeField] AudioClip[] answerList = null; //Right - Wrong
    [SerializeField] AudioClip[] musicList = null; //lobby - ingame
    AudioSource Audio = null;
    public int musiclist = 0;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        Audio = this.GetComponent<AudioSource>();
        musiclist = musicList.Length;
    }

    #region Button
    public void AddButtonListener() //각 씬매니저에서 호출
    {
        GameObject[] rootObj = GetSceneRootObject();

        for (int i = 0; i < rootObj.Length; i++)
        {
            GameObject go = (GameObject)rootObj[i] as GameObject;
            Component[] buttons = go.transform.GetComponentsInChildren(typeof(Button), true);
            foreach (Button button in buttons)
            {
                button.onClick.AddListener(delegate { UISound_OnClick(); });
            }
        }
    }
    GameObject[] GetSceneRootObject()
    {
        Scene curscene = SceneManager.GetActiveScene();
        return curscene.GetRootGameObjects();
    }
    public void UISound_HighLighted()
    {
        Audio.PlayOneShot(buttonList[0], 1);
    }
    public void UISound_OnClick()
    {
        Audio.PlayOneShot(buttonList[1], 1);
    }
    #endregion

    public void MusicStart(int musicindex)
    {
        if (Audio.isPlaying) Audio.Stop();
        Audio.clip = musicList[musicindex];
        Audio.Play();
    }
    public void UISound_RightAnswer()
    {
        Audio.PlayOneShot(answerList[0], 1);
    }
    public void UISound_WrongAnswer()
    {
        Debug.Log("틀렸다고 재생해");
        Audio.PlayOneShot(answerList[1], 1);
    }
}
