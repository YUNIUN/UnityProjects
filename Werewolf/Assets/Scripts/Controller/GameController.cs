using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;

class GameController
{
    public bool killHalf;
    public Dictionary<byte, GameObject> ins_dic;
    public Dictionary<byte, PlayerModel> idToModel;

    private GameController() 
    {
        ins_dic = new Dictionary<byte, GameObject>();
        idToModel = new Dictionary<byte, PlayerModel>();
    }
    private static GameController _instance;

    public static GameController Instance
    {
        get 
        {
            if (_instance == null)
            {
                _instance = new GameController();
            }
            return _instance; 
        }
    }

    public bool isExist(byte status)
    {
        foreach (var itor in idToModel)
        {
            if (itor.Value.status == status)
            {
                return true;
            }
        }
        return false;
    }

    public bool isAlive(byte status)
    {
        foreach (var itor in idToModel)
        {
            if (itor.Value.status == status)
            {
                return itor.Value.alive;
            }
        }
        return false;
    }

    public void DealCards()
    {
        List<byte> role_arr = new List<byte>();
        foreach (var itor in ins_dic)
        {
            for (int i = 0; i < itor.Value.GetComponent<selPrefController>().num; ++i)
            {
                role_arr.Add(itor.Key);
            }
        }

        for (byte id = 0; role_arr.Count > 0; ++id)
        {
            int pos = Random.Range(0, role_arr.Count);
            PlayerModel pm = PlayerFactory.Produce(id, role_arr[pos]);
            idToModel.Add(id, pm);
            role_arr.RemoveAt(pos);
        }
    }

    public byte Kill(byte killed, byte poisoned) // 游戏继续0，好人获胜1，狼人获胜2，猎人打枪3
    {
        if(killed < 255)
            idToModel[killed].alive = false;
        if(poisoned < 255)
            idToModel[poisoned].alive = false;
        byte result = CheckOver();
        if (result > 0)
            return result;
        else
        {
            if (killed < 255 && idToModel[killed].status == (byte)PlayerStatus.Hunter)
            {
                return 3;
            }
        }
        return 0;
    }

    private byte CheckOver() // 游戏继续0，好人获胜1，狼人获胜2，
    {
        if (killHalf)
        {
            bool w = false;
            bool g = false;
            bool v = false;
            foreach (var item in idToModel)
            {
                if (item.Value.alive)
                {
                    if (item.Value.type == (byte)PlayerType.Werewolf)
                        w = true;
                    else if(item.Value.type == (byte)PlayerType.Villager)
                        v = true;
                    else if (item.Value.type == (byte)PlayerType.God)
                        g = true;
                }
            }
            if (!w)
                return 1;
            if (!(v && g))
                return 2;
        }
        else
        {
            bool w = false;
            bool p = false;
            foreach (var item in idToModel)
            {
                if (item.Value.alive)
                {
                    if (item.Value.type == (byte)PlayerType.Werewolf)
                        w = true;
                    else
                        p = true;
                }
            }
            if (!w)
                return 1;
            if (!p)
                return 2;
        }
        return 0;
    }
}
