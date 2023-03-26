using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;

public static class AppSettinsExtensions
{
    public static ChatAppSettings GetChatSettings(this Photon.Realtime.AppSettings appSettings)
    {
        return new ChatAppSettings
        {
            AppIdChat = appSettings.AppIdChat,
            AppVersion = appSettings.AppVersion,
            FixedRegion = appSettings.IsBestRegion ? null : appSettings.FixedRegion,
            NetworkLogging = appSettings.NetworkLogging,
            Protocol = appSettings.Protocol,
            EnableProtocolFallback = appSettings.EnableProtocolFallback,
            Server = appSettings.IsDefaultNameServer ? null : appSettings.Server,
            Port = (ushort)appSettings.Port,
            ProxyServer = appSettings.ProxyServer
            // values not copied from AppSettings class: AuthMode
            // values not needed from AppSettings class: EnableLobbyStatistics 
        };
    }
    
}
public class UIChatMgr : MonoBehaviour, IChatClientListener
{
    [SerializeField] ScrollRect srChat = null; //��ȭ���
    [SerializeField] TextMeshProUGUI txtChat = null; //��ȭ����
    [SerializeField] TMP_InputField myChat = null; //��ȭ�Է�â

    ChatClient myChatClient = null;


    // Start is called before the first frame update
    void Start()
    {
        if(myChatClient == null)
        {
            myChatClient = new ChatClient(this);
            myChatClient.UseBackgroundWorkerForSending = true;
            myChatClient.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);
            myChatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        }
    }

    public void OnDestroy()
    {
        if(myChatClient != null)
        {
            myChatClient.Disconnect();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (myChatClient != null)
        {
            myChatClient.Service(); // OnGetMessages�� �ڵ� ȣ������. ���� ȣ������ �ʿ����.
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            myChat.ActivateInputField();
        }
    }

    public void OnEndEdit(string inStr)
    {
        if (inStr.Length <= 0) { return; }

        //����ä������ ������
        myChatClient.PublishMessage("public", inStr);
        myChat.text = "";

        srChat.verticalNormalizedPosition = 0; //��ũ�Ѻ並 �� ���������� ��ũ�ѽ�Ų��.(1�̸� ����)
        //addChatLine(PhotonNetwork.NickName, inStr);
    }
    void addChatLine(string userName, string chatLine)
    {
        txtChat.text += $"{userName} : {chatLine}\n";
    }
    //ä�� �������̽� ���� �Լ�
    #region
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected() //->OnSubscribed �� ȣ��
    {
        //ä�ΰ���
        myChatClient.Subscribe("public", 0);
        //throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }
    //������ ���� ä�ø޼����� �޾ƿ��� �Լ�
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        // ��� + �� �޼����� ����
        for(int i = 0; i < messages.Length; i++)
        {
            addChatLine(senders[i], messages[i].ToString());
        }
        //throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}
