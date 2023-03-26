using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class MainSceneMgr : MonoBehaviourPunCallbacks
{
    private static MainSceneMgr instance;
    public static MainSceneMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainSceneMgr>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(MainSceneMgr), typeof(MainSceneMgr)).GetComponent<MainSceneMgr>();
                }
            }
            return instance;
        }
    }

    //[System.Obsolete("얘 안씀!")]
    [SerializeField] GameObject PlayerList = null;
    [SerializeField] GameObject AnswerPanel = null;
    [SerializeField] GameObject GameOverPanel = null;

    [SerializeField] TMP_InputField InputAnswer = null;

    [SerializeField] TextMeshProUGUI GameStart = null;
    [SerializeField] TextMeshProUGUI Right = null;
    [SerializeField] TextMeshProUGUI RoomName = null;
    [SerializeField] TextMeshProUGUI Theme = null;
    [SerializeField] TextMeshProUGUI DrawTime = null;

    [SerializeField] SettingCanvas DrawPicture = null;

    GameObject player = null;
    PlayerChat playerChat = null;
    WaitUntil PlayerSetting = null;
    WaitUntil Start_Game = null;

    bool reStart = false;
    public bool getAnswer = false;
    byte orderPlayer = 1;

    string PlayMode = null;
    string[] themes = null;
    string theme = null;

    WaitForSeconds three = new WaitForSeconds(0.3f);
    WaitForSeconds half = new WaitForSeconds(0.5f);


    private void Awake()
    {
        if (PlayerList == null) PlayerList = GameObject.FindGameObjectWithTag(nameof(PlayerList));
        if (DrawPicture == null) DrawPicture = GameObject.Find(nameof(DrawPicture)).GetComponent<SettingCanvas>();
        PlayerSetting = new WaitUntil(() => PlayerList.transform.childCount == DBServer.Inst.Imgindex - 1);
        Start_Game = new WaitUntil(() => PlayerList.transform.childCount == PhotonNetwork.CurrentRoom.PlayerCount);
        PlayMode = DBServer.Inst.playMode;
        SoundMgr.Inst.AddButtonListener();

        if (PlayMode.Equals("random")) DBServer.Inst.GetKeyWord();
    }

    public void SetKeyWord(string keyword)
    {
        themes = keyword.Split(',');
    }
    public void NotfoundKeyWord()
    {
        TextAsset answer = Resources.Load<TextAsset>("KeyWord");
        themes = answer.text.Split(',');
    }

    private void Start()
    {
        RoomName.text = PhotonNetwork.CurrentRoom.Name;
        Theme.text = "제시어";
        DrawTime.text = "Time";
        Right.gameObject.SetActive(false);
        AnswerPanel.SetActive(false);

        StartCoroutine(nameof(MainInst));
    }

    IEnumerator MainInst()
    {
        do
        {
            reStart = false;
            yield return PlayerSetting;
            player = PhotonNetwork.Instantiate(nameof(PlayerPro), Vector3.zero, Quaternion.identity);
            playerChat = player.GetComponent<PlayerChat>();

            yield return Start_Game;
            yield return three;
            GameStart.gameObject.SetActive(true);
            yield return three;
            GameStart.gameObject.SetActive(false);
            yield return half;
            yield return half;
            GameLogic();
            yield return null;
        } while (reStart);
    }

    void GameLogic()
    {
        StopCoroutine(nameof(MainInst));
        if (PlayMode.Equals("random")) StartCoroutine(nameof(RandomMode));
        else if (PlayMode.Equals("personal")) StartCoroutine(nameof(PersonalMode));
    }

    IEnumerator RandomMode()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (orderPlayer == DBServer.Inst.Imgindex)
            {
                if (themes == null)
                {
                    TextAsset answer = Resources.Load<TextAsset>("KeyWord");
                    themes = answer.text.Split(',');
                }
                theme = themes[Random.Range(0, themes.Length)];
                photonView.RPC(nameof(RPC_SetTheme), RpcTarget.All, theme);
                Theme.text = theme;
                photonView.RPC(nameof(RPC_SetMusic), RpcTarget.All, Random.Range(1, SoundMgr.Inst.musiclist));
                DrawPicture.Myturn();
            }
            else
            {
                Theme.text = "?";
                playerChat.Answer();
            }
            yield return StartCoroutine(nameof(OneTurn));

            Theme.text = null;
            DrawTime.text = null;
            orderPlayer++;
            if (orderPlayer > PhotonNetwork.CurrentRoom.PlayerCount) orderPlayer = 1;

            if (getAnswer)
            {
                yield return half;
                Right.gameObject.SetActive(true);
                SoundMgr.Inst.UISound_RightAnswer();
                yield return three;
                Right.gameObject.SetActive(false);
                yield return half;
            }
            getAnswer = false;
        }
        EndGame();
    }
    IEnumerator PersonalMode()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (orderPlayer == DBServer.Inst.Imgindex)
            {
                AnswerPanel.SetActive(true);
                photonView.RPC(nameof(RPC_SetMusic), RpcTarget.All, Random.Range(1, SoundMgr.Inst.musiclist));
            }
            else
            {
                Theme.text = "?";
                playerChat.Answer();
            }
            yield return StartCoroutine(nameof(OneTurn));

            Theme.text = null;
            DrawTime.text = null;
            orderPlayer++;
            if (orderPlayer > PhotonNetwork.CurrentRoom.PlayerCount) orderPlayer = 1;

            if (getAnswer)
            {
                yield return half;
                Right.gameObject.SetActive(true);
                SoundMgr.Inst.UISound_RightAnswer();
                yield return three;
                Right.gameObject.SetActive(false);
                yield return half;
            }
            getAnswer = false;
        }
        EndGame();
    }

    public void OnEndEdit(string answer)
    {
        theme = answer;
        InputAnswer.text = null;
        photonView.RPC(nameof(RPC_SetTheme), RpcTarget.All, theme);
        Theme.text = theme;
        AnswerPanel.SetActive(false);
        DrawPicture.Myturn();
    }

    IEnumerator OneTurn()
    {
        float MyDrawTime = 100f;
        if (PlayMode != "random") MyDrawTime = 120f;
        while (MyDrawTime > 0f)
        {
            if (getAnswer) break;
            yield return null;
            DrawTime.text = $"{(int)MyDrawTime / 60} Min {(int)MyDrawTime % 60} Sec";
            MyDrawTime -= Time.deltaTime;
        }
        DrawPicture.End();
        playerChat.End();
    }

    void EndGame()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        List<PlayerPro> list = new List<PlayerPro>();

        for (int i = 0; i < PlayerList.transform.childCount; i++)
        {
            PlayerPro pro = PlayerList.transform.GetChild(i).GetComponent<PlayerPro>();
            Debug.Log(pro.MyScore);
            list.Add(pro);
        }
        list.Sort((a, b) => b.MyScore.CompareTo(a.MyScore));
        for (int i = 0; i < list.Count; i++)
        {
            bool rankOne = false;
            if (i == 0 || (list[0].MyScore != 0 && list[0].MyScore == list[i].MyScore)) rankOne = true;
            list[i].Set_Rank(rankOne);
            Debug.Log($"{i} {list[i].MyScore}");
        }
        photonView.RPC(nameof(RPC_GameOverPanel), RpcTarget.All);
    }

    public void OnClick_ExitButton()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnClick_ReStartButton()
    {
        photonView.RPC(nameof(RPC_ReStart), RpcTarget.All);
    }
    public void OnClick_StopButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    void RPC_SetMusic(int index)
    {
        SoundMgr.Inst.MusicStart(index);
    }

    [PunRPC]
    void RPC_SetTheme(string an)
    {
        theme = an;
        playerChat.SetTheme(theme);
    }

    [PunRPC]
    void RPC_ReStart()
    {
        PhotonNetwork.Destroy(player);
        reStart = true;
        GameOverPanel.SetActive(false);
        StartCoroutine(nameof(MainInst));
    }

    [PunRPC]
    void RPC_GameOverPanel()
    {
        playerChat.End();
        GameOverPanel.SetActive(true);
    }
    //uiRanking.gameObject.SetActive(!uiRanking.gameObject.activeSelf);
    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    #region Callback

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString()); //현재 서버 상태
        PhotonNetwork.LoadLevel("Start");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 다른 플레이어가 방을 나감
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) PhotonNetwork.LeaveRoom(); //혼자 남으면 자동퇴장
        if (otherPlayer.ActorNumber < PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DBServer.Inst.Imgindex--;
        }
    }

    public override void OnLeftRoom() // 방에서 나가면 실행
    {
        PhotonNetwork.Destroy(player);
        PhotonNetwork.LoadLevel("Lobby");
    }
    #endregion
}

