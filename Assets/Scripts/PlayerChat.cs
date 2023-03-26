using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class PlayerChat : MonoBehaviourPun
{
    [SerializeField] GameObject answerImage = null;
    [SerializeField] PlayerPro myplayer = null;
    [SerializeField] TMP_InputField inputField = null;

    string RightAnswer = null;
    // Start is called before the first frame update
    void Start()
    {
        answerImage.SetActive(false);
        if (photonView.IsMine == false)
        {
            inputField.interactable = false;
            return;
        }
        if (myplayer == null) myplayer = this.transform.GetComponent<PlayerPro>();
        answerImage.GetComponent<Image>().color = Color.green;
        inputField.onEndEdit.AddListener(delegate { GoAnswer(); });
        inputField.onValueChanged.AddListener(textView);
        inputField.onSelect.AddListener((text) => inputField.text = null);
    }

    public void SetTheme(string Atext) //제시어 설정
    {
        RightAnswer = Atext;
    }

    #region Answer
    public void Answer()
    {
        photonView.RPC(nameof(RPC_SetAnswer), RpcTarget.All);
        if(photonView.IsMine) inputField.text = "Enter text...";
        else inputField.text = null;
    }
    [PunRPC]
    void RPC_SetAnswer()
    {
        answerImage.SetActive(true);
    }
    #endregion

    #region End
    public void End()
    {
        photonView.RPC(nameof(RPC_SetEnd), RpcTarget.All);
    }
    [PunRPC]
    void RPC_SetEnd()
    {
        inputField.text = null;
        answerImage.SetActive(false);
    }
    #endregion
    
    void textView(string text)
    {
        photonView.RPC(nameof(RPC_textView), RpcTarget.Others, text);
    }
    [PunRPC]
    void RPC_textView(string text)
    {
        if (text == "Enter text...") return;
        inputField.text = text;
    }

    void GoAnswer()
    {
        if (RightAnswer.Equals(inputField.text))
        {
            myplayer.Set_UserScore(50);
        }
        else
        {
            SoundMgr.Inst.UISound_WrongAnswer();
        }
        inputField.text = null;
    }
}
