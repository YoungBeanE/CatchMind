using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

[System.Serializable]
public class UserData
{
    public string id = "";
    public string password = "";
    public string name = "";
    public string email = "";
    public string level = "";
}

public class DBServer : MonoBehaviour
{
    private static DBServer instance;
    public static DBServer Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DBServer>();
                if (instance == null)
                {
                    instance = new GameObject("DBServer", typeof(DBServer)).GetComponent<DBServer>();
                }
                //DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    [SerializeField] string serverURL = null; //http://192.168.0.49
    [SerializeField] int port;
    public string playMode;

    #region UserData
    UserData userInfo = new UserData(); //로그인하면 유저정보 세팅
    public byte Imgindex { get; set; } = 0;
    public string GetUserInfo()
    {
        return userInfo.id + userInfo.password + userInfo.name + userInfo.email + userInfo.level;
    }
    public string GetUserInfo_id()
    {
        return userInfo.id;
    }
    public string GetUserInfo_password()
    {
        return userInfo.password;
    }
    public string GetUserInfo_name()
    {
        return userInfo.name;
    }
    public string GetUserInfo_email()
    {
        return userInfo.email;
    }
    public string GetUserInfo_level()
    {
        return userInfo.level;
    }
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StartSceneMgr.Inst.callbackSignIn = SignIn;
        StartSceneMgr.Inst.callbackSignUp = SignUp;
        StartSceneMgr.Inst.callbackFindID = FindID;
        StartSceneMgr.Inst.callbackFindPWD = FindPWD;
        StartSceneMgr.Inst.callbackWithdraw = Withdraw;

        //UserData[] list = new UserData[5];
        //list.OrderBy(z => z.highscore)
        
    }

    #region DataBase
    void SignIn(UserData userData)
    {
        userInfo = userData;
        StartCoroutine(getProcess($"sigin/{userInfo.id}/{userInfo.password}"));
    }
    void SignUp(UserData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);
        StartCoroutine(postProcess(jsonData, "signup"));
    }
    void FindID(UserData userData)
    {
        userInfo = userData;
        StartCoroutine(getProcess($"findid/{userInfo.name}/{userInfo.email}"));
    }
    void FindPWD(UserData userData)
    {
        userInfo = userData;
        StartCoroutine(getProcess($"findpwd/{userInfo.id}/{userInfo.name}/{userInfo.email}"));
    }
    void Withdraw(UserData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);
        StartCoroutine(postProcess(jsonData, "withdraw"));
    }
    public void CheckID(string checkID)
    {
        userInfo.id = checkID;
        StartCoroutine(getProcess($"checkid/{userInfo.id}")); ///keywordlist
    }
    public void GetKeyWord()
    {
        StartCoroutine(getProcess("keywordlist"));
    }
    public void LevelUp(int lev)
    {
        userInfo.level = lev.ToString();
        string jsonData = JsonUtility.ToJson(userInfo);
        StartCoroutine(postProcess(jsonData, "levelup"));
    }
    IEnumerator postProcess(string jsonData, string url)
    {
        string targetURL = serverURL + ":" + port + "/" + url;

        using (UnityWebRequest req = UnityWebRequest.Post(targetURL, jsonData))
        {
            //그냥 보내면 json이 깨지기 때문에 byte 형식으로 바꾸고
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            //request의 함수를 이용해 업로드핸들러 생성
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            //다운로드 핸들러 생성
            req.downloadHandler = new DownloadHandlerBuffer();
            //헤드를 형성한다.
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();//실제로 데이터를 보내는 부분
            if (req.result == UnityWebRequest.Result.ConnectionError) //에러인경우 아닌경우 재확인 필요*******************************
            {
                Debug.Log(req.error);
                if (url == "signup") StartSceneMgr.Inst.Fail_SignUp();
                else if (url == "withdraw") StartSceneMgr.Inst.Fail_Withdraw();
                else if (url == "levelup") Debug.Log("레벨업 에러");
            }
            else
            {
                Debug.Log(req.downloadHandler.text);
                if (url == "signup" && req.downloadHandler.text == "ok") StartSceneMgr.Inst.SignUp();
                else if (url == "withdraw" && req.downloadHandler.text == "delete") StartSceneMgr.Inst.Withdraw();
                else if (url == "levelup" && req.downloadHandler.text == "up") Debug.Log("레벨업");
            }
            req.Dispose();//req객체를 삭제함 메모리누수발생
        }
    }
    IEnumerator getProcess(string url)
    {
        string targetURL = serverURL + ":" + port + "/" + url;

        using (UnityWebRequest www = UnityWebRequest.Get(targetURL))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                if (url == $"checkid/{userInfo.id}") StartSceneMgr.Inst.checkedID("error");
                else if (url == $"sigin/{userInfo.id}/{userInfo.password}") StartSceneMgr.Inst.Notfound();
                else if (url == $"findid/{userInfo.name}/{userInfo.email}") StartSceneMgr.Inst.NotfoundID();
                else if (url == $"findpwd/{userInfo.id}/{userInfo.name}/{userInfo.email}") StartSceneMgr.Inst.NotfoundPWD();
                else if (url == "keywordlist") MainSceneMgr.Inst.NotfoundKeyWord();
            }
            else
            {
                if (url == $"checkid/{userInfo.id}") StartSceneMgr.Inst.checkedID(www.downloadHandler.text);
                else if (url == $"sigin/{userInfo.id}/{userInfo.password}")
                {
                    if (www.downloadHandler.text == "notfound") StartSceneMgr.Inst.Notfound();
                    else
                    {
                        userInfo = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                        StartSceneMgr.Inst.GoLobby();
                    }
                }
                else if(url == $"findid/{userInfo.name}/{userInfo.email}")
                {
                    if (www.downloadHandler.text == "notfound") StartSceneMgr.Inst.NotfoundID();
                    else
                    {
                        userInfo = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                        StartSceneMgr.Inst.UserID(userInfo.id);
                    }
                }
                else if (url == $"findpwd/{userInfo.id}/{userInfo.name}/{userInfo.email}")
                {
                    if (www.downloadHandler.text == "notfound") StartSceneMgr.Inst.NotfoundPWD();
                    else
                    {
                        userInfo = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                        StartSceneMgr.Inst.UserPWD(userInfo.password);
                    }
                }
                else if(url == "keywordlist")
                {
                    MainSceneMgr.Inst.SetKeyWord(www.downloadHandler.text);
                }
            }
            //userList = JsonConvert.DeserializeObject<List<UserList>>(www.downloadHandler.text);
            www.Dispose(); //www객체를 삭제함
        }
    }
    #endregion

    /*
   
    IEnumerator AddUI()
    {
        UserList userinfo = new UserList();
        userinfo.name = NameInput.text;
        userinfo.age = int.Parse(AgeInput.text);
        userinfo.height = int.Parse(HeightInput.text);

        string jsonData = JsonUtility.ToJson(userinfo);
        yield return postProcess(jsonData, "adduserinfo");
        userList.Add(userinfo);
        userUI.Add(getLoadUserData());
        userUI[userUI.Count - 1].Init(this.gameObject, userinfo.name, userUI.Count - 1);
    }
    
    IEnumerator DeleteUI()
    {
        string jsonData = string.Format("{{\"name\":\"{0}\"}}", NameInput.text);
        yield return postProcess(jsonData, "deleteuserinfo");
        Destroy(userUI[curMyindex].gameObject);
        userList.RemoveAt(curMyindex);
        userUI.RemoveAt(curMyindex);
        for (int i = 0; i < userUI.Count; i++)
        {
            userUI[i].myindex = i;
        }
    }
    */

}
