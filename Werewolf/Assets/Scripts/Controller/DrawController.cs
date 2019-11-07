using System.Collections;
using Assets.Scripts.Model;
using UnityEngine.UI;
using UnityEngine;

public class DrawController : MonoBehaviour
{
    private Text txt_tips;
    private Image msgPanel;
    private Text txt_status;

    private GameObject btnDraw;
    private GameObject btnBegin;

    private byte PlayerID;
    void Awake()
    {
        txt_tips = transform.Find("Txt_Tips").GetComponent<Text>();
        msgPanel = transform.Find("message").GetComponent<Image>();
        txt_status = msgPanel.transform.Find("Txt_status").GetComponent<Text>();
        btnDraw = transform.Find("Btn_Draw").gameObject;
        btnBegin = transform.Find("Btn_Begin").gameObject;
    }

    private void OnEnable()
    {
        InitDraw();
    }

    public void OnDrawStatus()
    {
        txt_status.text = PlayerModel.nameByStatus[GameController.Instance.idToModel[PlayerID].status];
        msgPanel.gameObject.SetActive(true);
    }

    public void OnCloseMsg()
    {
        msgPanel.gameObject.SetActive(false);
        if (PlayerID + 1 == GameController.Instance.idToModel.Count)
        {
            txt_tips.gameObject.SetActive(false);
            btnDraw.SetActive(false);
            btnBegin.SetActive(true);
        }
        ++PlayerID;
        txt_tips.text = PlayerID + 1 + "号玩家抽取角色卡";
    }

    public void OnStartOver()
    {
        GameController.Instance.idToModel.Clear();
        transform.parent.Find("Panel_Setting").gameObject.SetActive(true);
        gameObject.SetActive(false);
        InitDraw();
    }

    public void OnGameBegin()
    {
        transform.parent.Find("Panel_Core").gameObject.SetActive(true);
        gameObject.SetActive(false);
        InitDraw();
    }

    private void InitDraw()
    {
        txt_tips.gameObject.SetActive(true);
        btnDraw.SetActive(true);
        btnBegin.SetActive(false);
        PlayerID = 0;
        txt_tips.text = PlayerID + 1 + "号玩家抽取角色卡";
    }
}
