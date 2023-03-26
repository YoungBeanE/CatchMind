using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class UserPro : MonoBehaviourPun
{
    [SerializeField] ResourcesObj ResDataObj = null;
    [SerializeField] Image PlayerImg = null;

    [SerializeField] TextMeshProUGUI ID = null;
    [SerializeField] TextMeshProUGUI NickName = null;
    [SerializeField] TextMeshProUGUI Level = null;

    [SerializeField] Button ready = null;
    [SerializeField] Image buttonColor = null;
    GameObject InRoomPanel_PlayerList = null;
    public bool IsReady { get; set; } = false;

    private void Awake()
    {
        InRoomPanel_PlayerList = GameObject.FindGameObjectWithTag("PlayerList");

        if (PlayerImg == null) PlayerImg = this.transform.GetChild(0).GetComponent<Image>();
        if (ready == null) ready = gameObject.transform.GetComponentInChildren<Button>();
        if (buttonColor == null) buttonColor = ready.gameObject.GetComponent<Image>();
        ready.onClick.AddListener(delegate { OnClick_ReadyButton(); });
    }
    private void Start()
    {
        this.transform.SetParent(InRoomPanel_PlayerList.transform, false);
        //if(photonView.IsMine) photonView.ObservedComponents.Add(this); //OnPhotonSerializeView사용시 필요 (+IPunObservable 상속)
    }

    #region UserInfo
    public void Set_UserInfo()
    {
        photonView.RPC(nameof(RPC_UserInfo), RpcTarget.AllBuffered, DBServer.Inst.GetUserInfo_id(), PhotonNetwork.LocalPlayer.NickName, DBServer.Inst.GetUserInfo_level());
    }

    [PunRPC]
    void RPC_UserInfo(string id, string nickname, string level)
    {
        ID.text = id;
        NickName.text = nickname;
        Level.text = "Level : " + level;
    }
    #endregion

    #region UserImg
    public void Set_UserImg(byte index)
    {
        DBServer.Inst.Imgindex = index;
        photonView.RPC(nameof(RPC_UserImg), RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    void RPC_UserImg(byte index)
    {
        PlayerImg.sprite = ResDataObj.PlayerImg[index];
    }
    #endregion

    #region Button
    void OnClick_ReadyButton()
    {
        if (photonView.IsMine == false) return;
        if (IsReady) IsReady = false;
        else IsReady = true;
        SoundMgr.Inst.UISound_OnClick();
        photonView.RPC(nameof(RPC_Ready), RpcTarget.AllBuffered, IsReady);
    }

    [PunRPC]
    void RPC_Ready(bool ready)
    {
        IsReady = ready;
        if (IsReady) buttonColor.color = Color.cyan;
        else buttonColor.color = Color.white;
    }

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
    #endregion

}
