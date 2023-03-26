using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbySceneMgr : MonoBehaviourPunCallbacks
{
    private static LobbySceneMgr instance;
    public static LobbySceneMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LobbySceneMgr>();
                if (instance == null)
                {
                    instance = new GameObject("LobbySceneMgr", typeof(LobbySceneMgr)).GetComponent<LobbySceneMgr>();
                }
            }
            return instance;
        }
    }

    public bool isProcessing = false;
    WaitUntil waitSetting = null;

    private void Awake()
    {
        RoomPref = Resources.Load<Room>("Room");
        BackGroundPref = (GameObject)Resources.Load("BackGround");
        if (PhotonNetwork.InLobby == false)
        {
            if(PhotonNetwork.IsConnectedAndReady == false)
            {
                PhotonNetwork.Reconnect();
            }
        }
        SoundMgr.Inst.AddButtonListener();
        waitSetting = new WaitUntil(() => InRoomPanel_PlayerList.transform.childCount == PhotonNetwork.CurrentRoom.PlayerCount - 1);
    }
    private void Start()
    {
        JoinRoomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
        InRoomPanel.SetActive(false);
        SoundMgr.Inst.MusicStart(0);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString()); //���� ���� ����
        PhotonNetwork.LoadLevel("Start");
    }

    #region Lobby
    [Header("Lobby")]
    [SerializeField] GameObject JoinRoomPanel = null;
    [SerializeField] GameObject CreateRoomPanel = null;
    [SerializeField] GameObject InRoomPanel = null;
    [SerializeField] ScrollRect scrollroomList = null;
    [SerializeField] Room RoomPref = null;

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // �� ��������� - �κ� ����, �����, �����, ����»���
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            //�� ������ isVisible�� false �̰ų� ����Ʈ���� ���ŵ� ����(�÷��̾ �ƹ��� ���)
            if (!roomList[i].IsVisible || roomList[i].RemovedFromList)
            {
                foreach (Room obj in scrollroomList.content.GetComponentsInChildren<Room>())
                {
                    if(roomList[i].Name.Equals(obj.RoomName)) Destroy(obj.gameObject); //���� ����鼭 �� ���� ���ʿ� ����
                }
                // �� ��Ͽ��� ����
                roomList.RemoveAt(i);
            }
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            bool set = false;
            foreach (Room obj in scrollroomList.content.GetComponentsInChildren<Room>())//�̹� ������ ���� ������ ������Ʈ
            {
                if (roomList[i].Name.Equals(obj.RoomName))
                {
                    set = true;
                    obj.SettingRoom(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers, roomList[i].IsOpen, obj.Mode);
                }
            }
            if(set == false)//���θ������ ��
            {
                Room room = Instantiate(RoomPref, scrollroomList.content.transform);
                Hashtable playmode = roomList[i].CustomProperties;
                string mode = (string)playmode["PlayMode"];
                room.SettingRoom(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers, roomList[i].IsOpen, mode);
            }
        }
    }
    public void OpenJoinRoomPanel(string roomname, string playmode)
    {
        isProcessing = true;
        JoinRoomPanel.SetActive(true);
        JoinRoomPanel_RoomName.text = roomname;
        PlayMode = playmode;
    }
    public void OnClick_CreateRoomButton()
    {
        if (isProcessing) return;
        isProcessing = true;
        JoinRoomPanel.SetActive(false);
        CreateRoomPanel.SetActive(true);
    }
    public void OnClick_BackButton()
    {
        if (isProcessing) return;
        PhotonNetwork.LoadLevel("Start");
    }
    #endregion
    
    #region JoinOrCreateRoom

    [Header("JoinRoomPanel")]
    [SerializeField] TextMeshProUGUI JoinRoomPanel_RoomName = null;
    [SerializeField] TMP_InputField JoinRoomPanel_NickName = null;

    [Header("CreateRoomPanel")]
    [SerializeField] TMP_InputField CreateRoomPanel_RoomName = null;
    [SerializeField] TMP_InputField CreateRoomPanel_NickName = null;

    RoomOptions roomOptions = new RoomOptions();
    
    string roomName = null;
    string PlayMode = "random"; //defualt
    public void OnClick_JoinRoomPanel_JoinButton()
    {
        if (string.IsNullOrEmpty(JoinRoomPanel_NickName.text)) return;

        isProcessing = false;
        PhotonNetwork.LocalPlayer.NickName = JoinRoomPanel_NickName.text;
        JoinRoomPanel_NickName.text = null;
        JoinRoomPanel.SetActive(false);

        PhotonNetwork.JoinRoom(JoinRoomPanel_RoomName.text, null);
    }
    public void OnEndEdit_CreateRoomName()
    {
        if (string.IsNullOrEmpty(CreateRoomPanel_RoomName.text)) return;
        roomName = CreateRoomPanel_RoomName.text;
    }
    public void OnValueChanged_Member(int member)
    {
        roomOptions.MaxPlayers = (byte)(member + 2);
    }
    public void OnValueChanged_Mode(int mode)
    {
        if (mode == 1) PlayMode = "personal";
    }
    public void OnEndEdit_CreateNickName()
    {
        if (string.IsNullOrEmpty(CreateRoomPanel_NickName.text)) return;
        PhotonNetwork.LocalPlayer.NickName = CreateRoomPanel_NickName.text;
    }
    public void OnClick_CreateRoomPanel_CreateButton()
    {
        if (string.IsNullOrEmpty(CreateRoomPanel_RoomName.text) || string.IsNullOrEmpty(CreateRoomPanel_NickName.text)) return;
        isProcessing = false;
        CreateRoomPanel_RoomName.text = null;
        CreateRoomPanel_NickName.text = null;
        CreateRoomPanel.SetActive(false);

        roomOptions.IsVisible = true;                        // ���� ���̰�
        roomOptions.IsOpen = true;                           // ���� ����
        roomOptions.CleanupCacheOnLeave = true;
        if (roomOptions.MaxPlayers == 0) roomOptions.MaxPlayers = 2; //maxPlayer ���������� 0(������)
        roomOptions.CustomRoomProperties = new Hashtable() { { "PlayMode", PlayMode } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "PlayMode" };
        
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public void OnClick_XButton()
    {
        isProcessing = false;
        JoinRoomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
    }

    #endregion

    #region InRoom
    [Header("InRoomPanel")]
    [SerializeField] TextMeshProUGUI InRoomPanel_RoomName = null;
    [SerializeField] GameObject InRoomPanel_BackImg = null;
    [SerializeField] GameObject InRoomPanel_CheckReadyPanel = null;

    [SerializeField] GameObject InRoomPanel_BackGroundList = null;
    [SerializeField] GameObject BackGroundPref = null;
    List<GameObject> backGroundList = new List<GameObject>();

    [SerializeField] GameObject InRoomPanel_PlayerList = null;
    [SerializeField] Button startButton = null;
    GameObject UserPro = null;
    int ReadyCount = 0;
    bool joinRoom = false;

    public override void OnJoinedRoom() //�� ���� �� �����ͼ����� ������ �������� ���Ӽ���(������Ŭ���̾�Ʈ)�� ����
    {
        if (PhotonNetwork.IsMasterClient == false) startButton.interactable = false;

        joinRoom = true;
        InRoomPanel.SetActive(true);
        InRoomPanel_BackImg.SetActive(true);
        InRoomPanel_RoomName.text = PhotonNetwork.CurrentRoom.Name;
        DBServer.Inst.playMode = PlayMode;

        for (int i = 1; i <= PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            backGroundList.Add(Instantiate(BackGroundPref, InRoomPanel_BackGroundList.transform));

        StartCoroutine(nameof(PhotonInst)); //������ ����
    }
    IEnumerator PhotonInst()
    {
        yield return waitSetting;
        UserPro = PhotonNetwork.Instantiate(nameof(UserPro), Vector3.zero, Quaternion.identity);
        UserPro.GetComponent<UserPro>().Set_UserInfo();
        UserPro.GetComponent<UserPro>().Set_UserImg(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinRoom = false;
        InRoomPanel.SetActive(true);
        InRoomPanel_BackImg.SetActive(false);
        InRoomPanel_RoomName.text = $"Connection Failed : <{returnCode}> {message}";
    }
    public void OnClick_StartButton()
    {
        ReadyCount = 0;
        foreach(UserPro player in FindObjectsOfType<UserPro>())
        {
            if (player.IsReady)
            {
                ReadyCount++;
            }
        }
        if(ReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("Main");
        }
        else
        {
            InRoomPanel_CheckReadyPanel.SetActive(true);
        }
    }
    public void OnClick_CheckReady()
    {
        InRoomPanel_CheckReadyPanel.SetActive(false);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // �ٸ� �÷��̾ ���� ����
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) PhotonNetwork.LeaveRoom(); //ȥ�� ������ �ڵ�����
        if (otherPlayer.ActorNumber < PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DBServer.Inst.Imgindex--;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient) startButton.interactable = true;
    }
    public void OnClick_LeaveRoomXButton()
    {
        if (joinRoom) PhotonNetwork.LeaveRoom();
        else InRoomPanel.SetActive(false);
    }
    public override void OnLeftRoom() // �濡�� ������ ����
    {
        for(int i = 0; i < backGroundList.Count; i++)
        {
            Destroy(backGroundList[i]);
        }
        backGroundList.Clear();
        PhotonNetwork.Destroy(UserPro);
        InRoomPanel.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    #endregion
}