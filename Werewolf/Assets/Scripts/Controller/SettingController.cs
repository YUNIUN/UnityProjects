using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    private Toggle [] tog_arr;
    private Toggle tog_half;
    private ScrollRect sv_role;
    [SerializeField]
    private GameObject role_pref;

    void Awake()
    {
        byte sum = 8;
        tog_arr = new Toggle[sum];
        for (byte i = 0; i < tog_arr.Length; ++i)
        {
            tog_arr[i] = transform.Find("Toggle_" + (i + 1)).GetComponent<Toggle>();
        }
        tog_half = transform.Find("Toggle_Half").GetComponent<Toggle>();
        sv_role = transform.Find("Sv_role").GetComponent<ScrollRect>();
    }

    void Start()
    {
        GameController.Instance.killHalf = tog_half.isOn;
        for (byte i = 0; i < tog_arr.Length; ++i)
        {
            if (tog_arr[i].isOn)
            {
                GameObject role_ins = GameObject.Instantiate(role_pref, sv_role.content);
                role_ins.GetComponent<selPrefController>().status = i;
                GameController.Instance.ins_dic.Add(i, role_ins);
            }
        }
    }

    public void OnTogChanged(int index)
    {
        if (tog_arr[index].isOn)
        {
            GameObject role_ins = GameObject.Instantiate(role_pref, sv_role.content);
            role_ins.GetComponent<selPrefController>().status = (byte)index;
            GameController.Instance.ins_dic.Add((byte)index, role_ins);
        }
        else
        {
            GameObject.Destroy(GameController.Instance.ins_dic[(byte)index]);
            GameController.Instance.ins_dic.Remove((byte)index);
        }
    }

    public void OnHalfChanged()
    {
        GameController.Instance.killHalf = tog_half.isOn;
    }

    public void OnStart()
    {
        GameController.Instance.DealCards();
        gameObject.SetActive(false);
        transform.parent.Find("Panel_Draw").gameObject.SetActive(true);
    }
}
