using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine.UI;
using UnityEngine;

public class selPrefController : MonoBehaviour
{
    [HideInInspector]
    public byte status;
    [HideInInspector]
    public byte num;
    // Start is called before the first frame update
    void Start()
    {
        num = 1;
        transform.Find("Txt_role").GetComponent<Text>().text = PlayerModel.nameByStatus[status];
        transform.Find("Txt_num").GetComponent<Text>().text = num.ToString();
    }

    public void OnBtnAdd()
    {
        transform.Find("Txt_num").GetComponent<Text>().text = (++num).ToString();
    }

    public void OnBtnSub()
    {
        if(num > 1)
            transform.Find("Txt_num").GetComponent<Text>().text = (--num).ToString();
    }
}
