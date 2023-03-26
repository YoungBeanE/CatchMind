using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class PlayerPro : MonoBehaviourPun
{
    GameObject PlayerList = null;
    GameObject Ranking = null;
    
    [SerializeField] ResourcesObj ResDataObj = null;
    [SerializeField] GameObject Crown = null;
    [SerializeField] Image PlayerImg = null;

    [SerializeField] TextMeshProUGUI NickName = null;
    [SerializeField] TextMeshProUGUI Level = null;
    [SerializeField] TextMeshProUGUI Score = null;

    int MyLevel;
    public int MyScore;

    private void Awake()
    {
        PlayerList = GameObject.FindGameObjectWithTag("PlayerList");
        Ranking = GameObject.Find("RefObj").GetComponent<RefObject>().targetObject;
        if (PlayerImg == null) PlayerImg = this.transform.GetChild(0).GetComponent<Image>();
        if (Crown == null) Crown = this.transform.GetChild(1).gameObject;
    }
    private void Start()
    {
        this.transform.SetParent(PlayerList.transform, false);
        Crown.SetActive(false);
        if (photonView.IsMine == false) return;
        Set_UserInfo();
        Set_UserImg(DBServer.Inst.Imgindex);
    }

    #region PlayerInfo
    void Set_UserInfo()
    {
        MyLevel = int.Parse(DBServer.Inst.GetUserInfo_level());
        MyScore = 0;
        photonView.RPC(nameof(RPC_UserInfo), RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, MyLevel, MyScore);
    }

    [PunRPC]
    void RPC_UserInfo(string nickname, int level, int score)
    {
        NickName.text = nickname;
        Level.text = $"Level : {level}";
        Score.text = $"Score : {score}";
    }
    #endregion

    #region PlayerImg
    public void Set_UserImg(byte index)
    {
        photonView.RPC(nameof(RPC_UserImg), RpcTarget.All, index);
    }

    [PunRPC]
    void RPC_UserImg(byte index)
    {
        PlayerImg.sprite = ResDataObj.PlayerImg[index];
    }
    #endregion

    #region PlayerScore
    public void Set_UserScore(int score)
    {
        MyScore += score;
        if ((MyScore % 100 == 0) && MyScore > 0)
        {
            MyLevel++;
            DBServer.Inst.LevelUp(MyLevel);
        }
        photonView.RPC(nameof(RPC_UserScore), RpcTarget.All, MyLevel, MyScore);
    }

    [PunRPC]
    void RPC_UserScore(int level, int score)
    {
        MyLevel = level;
        MyScore = score;
        MainSceneMgr.Inst.getAnswer = true;
        Level.text = $"Level : {MyLevel}";
        Score.text = $"Score : {MyScore}";
    }
    #endregion

    #region PlayerRanking
    public void Set_Rank(bool rankone)
    {
        photonView.RPC(nameof(RPC_Rank), RpcTarget.All, rankone);
    }

    [PunRPC]
    void RPC_Rank(bool setCrown)
    {
        this.transform.SetParent(Ranking.transform, false);
        Crown.SetActive(setCrown);
    }
    #endregion

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //동기화 시점에 호출되는 함수
    {
        Debug.Log("동기화");
        if (stream.IsWriting)// 상대방의 나에게 데이터를 보낼때
        {
            Debug.Log("보낸다 : " + IsReady);
            stream.SendNext(IsReady);
        }
        else // 상대방의 나로부터 데이터를 받을 때
        {
            IsReady = (bool)stream.ReceiveNext();
            Debug.Log("받는다 : " + IsReady); //받는게 안됨
        }
    }*/

}
