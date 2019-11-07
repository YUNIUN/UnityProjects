using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine.UI;
using UnityEngine;

public class CorePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject togIns;
    [SerializeField]
    private AudioClip[] aclips;
    [SerializeField]
    private AudioSource asource;
    private ScrollRect sv_core;
    private ToggleGroup tg_players;
    private Text txt_tips;
    private Button btn_sure;
    private Button btn_cancel;
    private Toggle[] tog_arr;
    private Image gameOverPanel;
    private Text txt_result;
    private byte killed;
    private byte poisoned;
    private bool save;
    private bool poison;
    private bool flag_goon;
    private bool flag_cancel;
    private bool flag_inter;

    void Awake()
    {
        sv_core = transform.Find("Sv_Core").GetComponent<ScrollRect>();
        tg_players = transform.Find("Sv_Core").GetComponent<ToggleGroup>();
        txt_tips = transform.Find("Txt_tips").GetComponent<Text>();
        btn_sure = transform.Find("Btn_Sure").GetComponent<Button>();
        btn_cancel = transform.Find("Btn_Cancel").GetComponent<Button>();
        gameOverPanel = transform.Find("gameOver").GetComponent<Image>();
        txt_result = gameOverPanel.transform.Find("Txt_result").GetComponent<Text>();
    }

    void OnEnable()
    {
        tog_arr = new Toggle[GameController.Instance.idToModel.Count];
        foreach (var itor in GameController.Instance.idToModel)
        {
            GameObject tog_ins = GameObject.Instantiate(togIns, sv_core.content);
            tog_ins.transform.Find("Label").GetComponent<Text>().text = (itor.Key + 1) / 10 + "" + (itor.Key + 1) % 10 + "号玩家";
            tog_arr[itor.Key] = tog_ins.GetComponent<Toggle>();
            tog_arr[itor.Key].group = sv_core.GetComponent<ToggleGroup>();
            float wid = sv_core.content.GetComponent<RectTransform>().rect.width / 3 - 1.5f;
            sv_core.content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wid, wid);
        }
        killed = 255;
        poisoned = 255;
        save = true;
        poison = true;
        flag_goon = false;
        flag_cancel = false;
        flag_inter = false;
        StartCoroutine("GameStart");
    }

    void Update()
    {
        if (tg_players.AnyTogglesOn() && flag_inter)
            btn_sure.interactable = true;
        else
            btn_sure.interactable = false;
        for (byte id = 0; id < tog_arr.Length; ++id)
        {
            if (!GameController.Instance.idToModel[id].alive)
                tog_arr[id].interactable = false;
        }
    }

    IEnumerator GameStart()
    {
        while (!gameOverPanel.gameObject.activeSelf)
        {
            PlayTips("天黑请闭眼", 0);
            yield return new WaitForSeconds(3.5f);
            flag_inter = true;
            btn_cancel.interactable = true;
            if (GameController.Instance.isExist((byte)PlayerStatus.Prophet))
            {
                PlayTips("预言家请睁眼，请选择你要查看的玩家", 1);
                if (GameController.Instance.isAlive((byte)PlayerStatus.Prophet))
                {
                    yield return StartCoroutine("ProphetDoing");
                }
                else
                    yield return new WaitForSeconds(8.0f);
                yield return StartCoroutine("RoundOver");
            }
            if (GameController.Instance.isExist((byte)PlayerStatus.Werewolf))
            {
                PlayTips("狼人请睁眼，请选择你要杀死的玩家", 2);
                yield return StartCoroutine("WerewolfDoing");
                yield return StartCoroutine("RoundOver");
            }
            if (GameController.Instance.isExist((byte)PlayerStatus.Witch))
            {
                PlayTips("女巫请睁眼", 3);
                if (GameController.Instance.isAlive((byte)PlayerStatus.Witch))
                {
                    yield return StartCoroutine("WitchDoing");
                }
                else
                    yield return new WaitForSeconds(8.0f);
                yield return StartCoroutine("RoundOver");
            }
            yield return StartCoroutine("VoteDoing");
            tg_players.SetAllTogglesOff();
            btn_cancel.interactable = false;
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator ProphetDoing()
    {
        while (!flag_goon)
            yield return null;
        if (flag_cancel)
            flag_cancel = false;
        else
        {
            byte id;
            for (id = 0; id < tog_arr.Length; ++id)
            {
                if (tog_arr[id].isOn)
                    break;
            }
            if (GameController.Instance.idToModel[id].type == (byte)PlayerType.Werewolf)
                txt_tips.text = "他是狼";
            else
                txt_tips.text = "他是好人";
            btn_cancel.interactable = false;
            flag_inter = false;
            yield return new WaitForSeconds(3.0f);
        }
        flag_goon = false;
    }

    IEnumerator WerewolfDoing()
    {
        while (!flag_goon)
            yield return null;
        if (flag_cancel)
            flag_cancel = false;
        else
        {
            for (byte id = 0; id < tog_arr.Length; ++id)
            {
                if (tog_arr[id].isOn)
                {
                    killed = id;
                    break;
                }
            }
        }
        flag_goon = false;
    }

    IEnumerator WitchDoing()
    {
        bool isSave = true;
        if (save)
        {
            btn_sure.interactable = true;
            btn_cancel.interactable = true;
            if (killed < 255)
            {
                tog_arr[killed].isOn = true;
                txt_tips.text = "今晚即将死亡的是" + (killed + 1) + "号玩家，是否解救？";
                while (!flag_goon)
                    yield return null;
                if (flag_cancel)//没救
                {
                    isSave = false;
                    flag_cancel = false;
                }
                else //救了
                {
                    killed = 255;
                    save = false;
                }
                flag_goon = false;
            }
            else
            {
                isSave = false;
                txt_tips.text = "今晚是平安夜";
                btn_cancel.interactable = false;
                yield return new WaitForSeconds(3.0f);
            }
        }
        else if (poison)
        {
            btn_sure.interactable = true;
            btn_cancel.interactable = true;
            txt_tips.text = "请选择要毒的玩家";
            tg_players.SetAllTogglesOff();
            while (!flag_goon)
                yield return null;
            if (flag_cancel)//不毒
                flag_cancel = false;
            else//毒人
            {
                for (byte id = 0; id < tog_arr.Length; ++id)
                {
                    if (tog_arr[id].isOn)
                    {
                        poisoned = id;
                        poison = false;
                        break;
                    }
                }
            }
            flag_goon = false;
        }
        else
        {
            btn_sure.interactable = false;
            btn_cancel.interactable = false;
            yield return new WaitForSeconds(8.0f);
        }

        if ((!isSave) && poison)
        {
            //没救
            btn_sure.interactable = true;
            btn_cancel.interactable = true;
            txt_tips.text = "请选择要毒的玩家";
            tg_players.SetAllTogglesOff();
            while (!flag_goon)
                yield return null;
            if (flag_cancel)//不毒
                flag_cancel = false;
            else//毒人
            {
                for (byte id = 0; id < tog_arr.Length; ++id)
                {
                    if (tog_arr[id].isOn)
                    {
                        poisoned = id;
                        poison = false;
                        break;
                    }
                }
            }
            flag_goon = false;
        }
    }

    IEnumerator VoteDoing()
    {
        // 结算晚上
        string morning_str = "天亮了";
        if (killed == 255 && poisoned == 255)
            morning_str = morning_str + ",昨晚是平安夜";
        else if (killed < 255 && poisoned < 255 && killed != poisoned)
        {
            byte min = killed < poisoned ? killed : poisoned;
            morning_str = morning_str + ",昨晚死的是" + (min + 1) + "号玩家和" + (killed + poisoned - min + 1) + "号玩家";
        }
        else
        {
            byte die = killed < 255 ? killed : poisoned;
            morning_str = morning_str + ",昨晚死的是" + (die + 1) + "号玩家";
        }
        PlayTips(morning_str, 5);
        byte result = GameController.Instance.Kill(killed, poisoned);
        if (result == 1)//好人获胜
        {
            gameOverPanel.gameObject.SetActive(true);
            txt_result.text = "游戏结束好人获胜";
            StopAllCoroutines();
        }
        else if (result == 2)//狼人获胜
        {
            gameOverPanel.gameObject.SetActive(true);
            txt_result.text = "游戏结束狼人获胜";
            StopAllCoroutines();
        }
        else if (result == 3)//猎人开枪
        {
            yield return StartCoroutine("HunterDoing");
        }

        // 结算白天
        yield return new WaitForSeconds(3);
        txt_tips.text = "投票杀死狼人";
        btn_cancel.interactable = false;
        while (!flag_goon)
             yield return null;
        for (byte id = 0; id < tog_arr.Length; ++id)
        {
            if (tog_arr[id].isOn)
            {
                result = GameController.Instance.Kill(id, 255);
                if (result == 1)//好人获胜
                {
                    gameOverPanel.gameObject.SetActive(true);
                    txt_result.text = "游戏结束好人获胜";
                    StopAllCoroutines();
                }
                else if (result == 2)//狼人获胜
                {
                    gameOverPanel.gameObject.SetActive(true);
                    txt_result.text = "游戏结束狼人获胜";
                    StopAllCoroutines();
                }
                else if (result == 3)//猎人开枪
                {
                    yield return StartCoroutine("HunterDoing");
                }
                break;
            }
        }
        flag_goon = false;
    }

    IEnumerator HunterDoing()
    {
        yield return new WaitForSeconds(1);
        txt_tips.text = "猎人请开枪";
        while (!flag_goon)
            yield return null;
        if (flag_cancel)
            flag_cancel = false;
        else
        {
            for (byte id = 0; id < tog_arr.Length; ++id)
            {
                if (tog_arr[id].isOn)
                {
                    GameController.Instance.Kill(id, 255);
                    break;
                }
            }
        }
        flag_goon = false;
        tg_players.SetAllTogglesOff();
    }

    IEnumerator RoundOver()
    {
        PlayTips("你的回合已结束，请闭眼", 4);
        tg_players.SetAllTogglesOff();
        flag_inter = true;
        btn_cancel.interactable = false;
        yield return new WaitForSeconds(6.0f);
        btn_cancel.interactable = true;
    }

    private void PlayTips(string str, int index)
    {
        asource.Stop();
        asource.PlayOneShot(aclips[index]);
        txt_tips.text = str;
    }

    public void OnSure()
    {
        flag_goon = true;
    }

    public void OnCancel()
    {
        flag_goon = true;
        flag_cancel = true;
    }

    public void OnReStart()
    {
        for (byte id = 0; id < tog_arr.Length; ++id)
        {
            GameObject.Destroy(tog_arr[id].gameObject);
        }
        tog_arr = null;
        GameController.Instance.idToModel.Clear();
        gameObject.transform.parent.Find("Panel_Setting").gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
