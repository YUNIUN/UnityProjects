namespace Assets.Scripts.Model
{
    enum PlayerStatus
    {
        Villager, //村民
        Werewolf, //狼人
        Hunter,   //猎人
        Witch,    //女巫
        Prophet,  //预言家
        Mustee,   //混血儿
        Patsy,    //替罪羊
        Angel     //天使
    }
    enum PlayerType
    {
        Villager, //村民
        Werewolf, //狼人
        God,      //神
        Other     //其他
    }
    public class PlayerModel
    {
        public static string[] nameByStatus = { "村民", "狼人", "猎人", "女巫", "预言家", "混血儿", "替罪羊", "天使" };
        public byte id;     //id
        public byte status; //身份
        public byte type;   //民：0、狼：1、神：2、其他3
        public bool alive;
    }
}
