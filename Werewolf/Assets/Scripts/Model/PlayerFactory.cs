namespace Assets.Scripts.Model
{
    class PlayerFactory
    {
        public static PlayerModel Produce(byte id, byte status)
        {
            PlayerModel pm = new PlayerModel();
            pm.id = id;
            pm.status = status;
            pm.alive = true;
            switch (status)
            {
                case (byte)PlayerStatus.Villager:
                case (byte)PlayerStatus.Patsy:
                case (byte)PlayerStatus.Angel:
                    pm.type = (byte)PlayerType.Villager;
                    break;
                case (byte)PlayerStatus.Werewolf:
                    pm.type = (byte)PlayerType.Werewolf;
                    break;
                case (byte)PlayerStatus.Prophet:
                case (byte)PlayerStatus.Hunter:
                case (byte)PlayerStatus.Witch:
                    pm.type = (byte)PlayerType.God;
                    break;
                case (byte)PlayerStatus.Mustee:
                default:
                    pm.type = (byte)PlayerType.Other;
                    break;
            }
            return pm;
        }
    }
}
