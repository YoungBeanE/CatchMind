using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public delegate void Callback(UserData userData);

public class StartSceneMgr : MonoBehaviourPunCallbacks
{
    private static StartSceneMgr instance;
    public static StartSceneMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StartSceneMgr>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(StartSceneMgr), typeof(StartSceneMgr)).GetComponent<StartSceneMgr>();
                }
            }
            return instance;
        }
    }

    public Callback callbackSignIn = null;
    public Callback callbackSignUp = null;
    public Callback callbackFindID = null;
    public Callback callbackFindPWD = null;
    public Callback callbackWithdraw = null;

    bool isProcessing = false;
    
    #region Network
    void Awake()
    {
        // Set Screen Size 16 : 9 fullscreen false
        Screen.SetResolution(800, 450, false);
        // Improves the performance of the Photon network.
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        // Automatically synchronized scenes from other clients when switching scenes from the master server.
        PhotonNetwork.AutomaticallySyncScene = true;
        // Don't get a photonmessage when access the room.
        PhotonNetwork.IsMessageQueueRunning = false;
        //Connect network.
        if (PhotonNetwork.IsConnected == false) PhotonNetwork.ConnectUsingSettings();

        if (SignIn == null) SignIn = FindObjectOfType<Canvas>().gameObject;
        StartCoroutine(FailOrReturn());
        SoundMgr.Inst.AddButtonListener();
        SoundMgr.Inst.MusicStart(0);
    }

    IEnumerator FailOrReturn()
    {
        if (PhotonNetwork.IsConnectedAndReady == false) logoImg.SetActive(true);
        if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
        yield return new WaitForSeconds(2f);
        if (SignIn.activeInHierarchy == false) SignIn.SetActive(true);
        if (PhotonNetwork.IsConnectedAndReady) logoImg.SetActive(false);
    }
    public override void OnConnectedToMaster()  // Call when the master server is connected
    {
        SignIn.SetActive(true);
        logoImg.SetActive(false);
        SignUpPanel.SetActive(false);
        FindIDPanel.SetActive(false);
        FindPWDPanel.SetActive(false);
    }
    public override void OnDisconnected(DisconnectCause cause)  // Call when the master server is not connected
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
    #endregion

    #region Start
    [Header("Start")]
    [SerializeField] GameObject SignIn = null;
    [SerializeField] GameObject logoImg = null;
    [SerializeField] TMP_InputField signinID = null;
    [SerializeField] TMP_InputField signinPWD = null;
    [SerializeField] TextMeshProUGUI notfound = null;
    [SerializeField] GameObject SignUpPanel = null;
    [SerializeField] GameObject FindIDPanel = null;
    [SerializeField] GameObject FindPWDPanel = null;

    UserData userData = new UserData();
    public void OnEndEdit_signinID()
    {
        userData.id = signinID.text;
        notfound.text = null;
    }
    public void OnEndEdit_signinPWD()
    {
        userData.password = signinPWD.text;
        notfound.text = null;
    }
    public void OnClickSignInButton() //로그인
    {
        if (isProcessing) return;
        if (userData.id.Length < 4 || userData.password.Length < 6)
        {
            notfound.text = "잘못된 형식입니다";
            signinID.text = null;
            signinPWD.text = null;
            return;
        }
        isProcessing = true;
        if (callbackSignIn != null) callbackSignIn(userData);
    }
    public void Notfound() //로그인 정보 불일치
    {
        notfound.text = "일치하는 회원 없음";
        signinID.text = null;
        signinPWD.text = null;
        isProcessing = false;
    }
    public void GoLobby() //로그인 정보 일치
    {
        notfound.text = null;
        isProcessing = false;
        PhotonNetwork.JoinLobby();
    }
    public void OnClickSignUpButton() //회원가입
    {
        if (isProcessing) return;
        isProcessing = true;
        SignUpPanel.SetActive(true);
        canuseID.text = null;
        SignUpuserData = new UserData();
        SettingCheckChar();
        isProcessing = false;
    }
    public void OnClickFindIDButton()
    {
        if (isProcessing) return;
        isProcessing = true;
        userID.text = null;
        FindIDPanel.SetActive(true);
        FindIDuserData = new UserData();
        isProcessing = false;
    } //아이디찾기
    public void OnClickFindPWDButton()
    {
        if (isProcessing) return;
        isProcessing = true;
        userPWD.text = null;
        FindPWDPanel.SetActive(true);
        FindPWDuserData = new UserData();
        isProcessing = false;
    } //비밀번호찾기
    public void OnClickWithdrawButton() //탈퇴
    {
        if (isProcessing) return;
        if (userData.id.Length < 4 || userData.password.Length < 6)
        {
            signinID.text = null;
            signinPWD.text = null;
            notfound.text = "탈퇴하려는 회원의 아이디와 비밀번호를 입력하시오.";
            return;
        }
        isProcessing = true;
        if (callbackWithdraw != null) callbackWithdraw(userData);
    }
    public void Withdraw() //탈퇴완료
    {
        notfound.text = "탈퇴완료";
        isProcessing = false;
    }
    public void Fail_Withdraw() //탈퇴실패
    {
        notfound.text = "일치하는 회원 없음";
        signinID.text = null;
        signinPWD.text = null;
        isProcessing = false;
    }
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    public void OnClickBackButton() //로그인창으로
    {
        if (isProcessing) return;
        notfound.text = null;

        signupID.text = null;
        signupPWD.text = null;
        signupcheckPWD.text = null;
        signupName.text = null;
        signupEmail.text = null;
        canuseID.text = null;

        findidName.text = null;
        findidEmail.text = null;
        userID.text = null;

        findpwdID.text = null;
        findpwdName.text = null;
        findpwdEmail.text = null;
        userPWD.text = null;

        SignUpPanel.SetActive(false);
        FindIDPanel.SetActive(false);
        FindPWDPanel.SetActive(false);
        isProcessing = false;
    }
    #endregion

    #region SignUp Panel
    [Header("Sign Up Panel")]
    [SerializeField] TMP_InputField signupID = null;
    [SerializeField] TMP_InputField signupPWD = null;
    [SerializeField] TMP_InputField signupcheckPWD = null;
    [SerializeField] TMP_InputField signupName = null;
    [SerializeField] TMP_InputField signupEmail = null;
    [SerializeField] TextMeshProUGUI canuseID = null;

    UserData SignUpuserData = null;

    string checkID = null;
    #region checkID
    int[] canid = new int[62];
    void SettingCheckChar()
    {
        for (int i = 0; i < canid.Length; i++)
        {
            if (i >= 36)
            {
                canid[i] = i + 61;
            }
            else if (i >= 10)
            {
                canid[i] = i + 55;
            }
            else
                canid[i] = i + 48;
        }
    }
    bool CheckChar(char charid)
    {
        for (int i = 0; i < canid.Length; i++)
        {
            if (canid[i] == charid)
                return true; //영어 숫자 사용
        }
        return false; //특수문자사용
    }
    #endregion

    public void OnEndEdit_signupID()
    {
        canuseID.text = null;
    }
    public void OnClicksignupCheckIDButton() //아이디 중복여부 체크
    {
        if (isProcessing) return;
        canuseID.color = Color.red;
        if (signupID.text.Length < 4 || signupID.text.Length > 10)
        {
            signupID.text = null;
            canuseID.text = "사용불가능";
            return;
        }
        char[] checkid = signupID.text.ToCharArray();
        for (int i = 0; i < checkid.Length; i++)
        {
            if (CheckChar(checkid[i]) == false)
            {
                signupID.text = null;
                canuseID.text = "사용불가능";
                return;
            }
        }
        isProcessing = true;
        DBServer.Inst.CheckID(signupID.text);
    }
    public void checkedID(string res) //아이디 중복여부 리턴
    {
        if (res == "can")
        {
            canuseID.color = Color.blue;
            canuseID.text = "사용가능";
            checkID = signupID.text;
        }
        else if(res == "cannot")
        {
            canuseID.text = "중복아이디";
            signupID.text = null;
        }
        else if (res == "error")
        {
            canuseID.text = "사용불가능";
            signupID.text = null;
        }
        isProcessing = false;
    }
    public void OnEndEdit_signupPWD()
    {
        signupcheckPWD.text = null;
        SignUpuserData.password = signupPWD.text;
    }
    public void OnEndEdit_signupcheckPWD() //비밀번호 재확인
    {
        if (SignUpuserData.password == signupcheckPWD.text)
        {
            SignUpuserData.password = signupcheckPWD.text;
        }
        else
        {
            signupcheckPWD.text = "비밀번호 불일치";
            signupPWD.text = null;
            SignUpuserData.password = null;
        }
    }
    public void OnEndEdit_signupName()
    {
        SignUpuserData.name = signupName.text;
    }
    public void OnEndEdit_signupEmail()
    {
        SignUpuserData.email = signupEmail.text;
    }
    public void OnClicksignupButton() //가입요청
    {
        if (isProcessing) return;
        if (signupID.text.Length < 4)
        {
            signupID.text = null;
            return;
        }
        if(canuseID.text != "사용가능")
        {
            signupID.text = null;
            canuseID.text = "아이디 중복확인 필요";
            return;
        }
        if (checkID != signupID.text)
        {
            signupID.text = null;
            canuseID.text = "아이디 중복확인 필요";
            return;
        }
        if (SignUpuserData.password.Length < 6)
        {
            signupPWD.text = null;
            return;
        }
        if(SignUpuserData.name.Length < 2)
        {
            signupName.text = null;
            return;
        }
        if (SignUpuserData.email.Length < 8)
        {
            signupEmail.text = null;
            return;
        }
        
        isProcessing = true;
        SignUpuserData.id = signupID.text;
        SignUpuserData.level = "1";//회원가입시 레벨1 초기화
        if (callbackSignUp != null) callbackSignUp(SignUpuserData);
    }
    public void SignUp() //가입완료
    {
        signupID.text = null;
        signupPWD.text = null;
        signupcheckPWD.text = null;
        signupName.text = null;
        signupEmail.text = null;
        SignUpPanel.SetActive(false);
        notfound.text = "가입완료";
        isProcessing = false;
    }
    public void Fail_SignUp() //가입실패
    {
        canuseID.color = Color.red;
        canuseID.text = "가입실패";
        signupID.text = null;
        signupPWD.text = null;
        signupcheckPWD.text = null;
        signupName.text = null;
        signupEmail.text = null;
        isProcessing = false;
    }
    #endregion

    #region Find ID Panel
    [Header("Find ID Panel")]
    [SerializeField] TMP_InputField findidName = null;
    [SerializeField] TMP_InputField findidEmail = null;
    [SerializeField] TMP_InputField userID = null;

    UserData FindIDuserData = null;
    public void OnEndEdit_findidName()
    {
        userID.text = null;
        FindIDuserData.name = findidName.text;
    }
    public void OnEndEdit_findidEmail()
    {
        userID.text = null;
        FindIDuserData.email = findidEmail.text;
    }
    public void OnClick_findIdButton() //아이디 검색
    {
        if (isProcessing) return;
        if (FindIDuserData.name.Length < 3)
        {
            findidName.text = null;
            return;
        }
        if (FindIDuserData.email.Length < 10)
        {
            findidEmail.text = null;
            return;
        }
        isProcessing = true;
        if (callbackFindID != null) callbackFindID(FindIDuserData);
    }
    public void UserID(string id)
    {
        userID.text = id;
        isProcessing = false;
    }
    public void NotfoundID()
    {
        userID.text = "일치하는 회원 없음";
        findidName.text = null;
        findidEmail.text = null;
        isProcessing = false;
    }
    public void OnClick_IDcheckButton()
    {
        findidName.text = null;
        findidEmail.text = null;
        userID.text = null;
        FindIDPanel.SetActive(false);
        isProcessing = false;
    }
    #endregion

    #region Find PWD Panel
    [Header("Find PWD Panel")]

    [SerializeField] SendMail sendEmail = null;
    [SerializeField] TMP_InputField findpwdID = null;
    [SerializeField] TMP_InputField findpwdName = null;
    [SerializeField] TMP_InputField findpwdEmail = null;

    [SerializeField] TextMeshProUGUI foundUser = null;
    [SerializeField] Button sendMail = null;

    [SerializeField] TMP_InputField PAC = null;
    [SerializeField] TMP_InputField userPWD = null;

    UserData FindPWDuserData = null;
    string pwd = null;
    int Code = 0;
    int tempCode = 0;
    public void OnEndEdit_findpwdID()
    {
        userPWD.text = null;
        FindPWDuserData.id = findpwdID.text;
    }
    public void OnEndEdit_findpwdName()
    {
        userPWD.text = null;
        FindPWDuserData.name = findpwdName.text;
    }
    public void OnEndEdit_findpwdEmail()
    {
        userPWD.text = null;
        FindPWDuserData.email = findpwdEmail.text;
    }
    public void OnClick_findPWDButton() //비밀번호 검색
    {
        if (isProcessing) return;
        if (FindPWDuserData.id.Length < 4)
        {
            findpwdID.text = null;
            return;
        }
        if (FindPWDuserData.name.Length < 2)
        {
            findpwdName.text = null;
            return;
        }
        if (FindPWDuserData.email.Length < 8)
        {
            findpwdEmail.text = null;
            return;
        }
        isProcessing = true;
        foundUser.color = Color.red;
        if (callbackFindPWD != null) callbackFindPWD(FindPWDuserData);
    }
    public void UserPWD(string password)
    {
        foundUser.color = Color.blue;
        foundUser.text = "일치하는 회원 있음";
        pwd = password;
        sendMail.interactable = true;
        isProcessing = false;
    }
    public void NotfoundPWD()
    {
        foundUser.text = "일치하는 회원 없음";
        sendMail.interactable = false;
        findpwdID.text = null;
        findpwdName.text = null;
        findpwdEmail.text = null;
        isProcessing = false;
    }
    public void OnClick_SendMail()
    {
        Code = Random.Range(100000, 1000000);
        sendEmail.Send(findpwdEmail.text, Code);
    }
    public void OnEndEdit_PAC()
    {
        userPWD.text = null;
        int.TryParse(PAC.text, out tempCode);
    }
    public void OnClick_CheckCode()
    {
        if (string.IsNullOrEmpty(PAC.text) || PAC.text.Length < 6)
        {
            PAC.text = null;
            return;
        }
        if (tempCode == Code)
        {
            userPWD.text = pwd;
        }
        PAC.text = null;
    }
    public void OnClick_PWDcheckButton()
    {
        findpwdID.text = null;
        findpwdName.text = null;
        findpwdEmail.text = null;
        userPWD.text = null;
        FindPWDPanel.SetActive(false);
        isProcessing = false;
    }
    #endregion
}
