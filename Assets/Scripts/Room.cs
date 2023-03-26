using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Room : Selectable
{
    [SerializeField] TextMeshProUGUI Name = null;
    [SerializeField] TextMeshProUGUI Member = null;
    [SerializeField] Button canClick = null;
    [SerializeField] Image imagecolor = null;

    public string RoomName { get; set; }
    public string Mode { get; set; }
    bool isOpen = true;
    bool isSelected = false;

    protected override void Start()
    {
        if (Name == null) Name = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (Member == null) Member = this.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (canClick == null) canClick = this.transform.GetComponent<Button>();
    }
    public void SettingRoom(string name, int cur, int max, bool open, string mode)
    {
        RoomName = name;
        Mode = mode;
        Name.text = $"{name}({mode})";
        Member.text = $"{cur}/{max}";
        isOpen = open;
        if (mode == "random") imagecolor.color = Color.green;
        if (isOpen == false || cur == max) canClick.interactable = false;
        else canClick.interactable = true;
    }
    public void OnClick_JoinRoomButton()
    {
        if (LobbySceneMgr.Inst.isProcessing) return;
        SoundMgr.Inst.UISound_OnClick();
        LobbySceneMgr.Inst.OpenJoinRoomPanel(RoomName, Mode);
    }

    void Update()
    {
        if (IsHighlighted())
        {
            if (isSelected) return;
            SoundMgr.Inst.UISound_HighLighted();
            isSelected = true;
        }
        else
            isSelected = false;
    }

}
