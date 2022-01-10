using System;
using Platformer.Helpers;

namespace Platformer.Players
{
    public class PlayerInfoManager
    {
        public static int CurrentPlayerIndex;
        public static PlayerInfo CurrentPlayerInfo { get; set; }

        public static int Count
        {
            get { return GlobalParameters.PlayersInfoList.Count; }
        }

        static PlayerInfoManager()
        {
            CurrentPlayerInfo = GlobalParameters.PlayersInfoList[CurrentPlayerIndex];
        }

        public static PlayerInfo GetGoodGuy(PlayerType type)
        {
            var index = (int)type;
            if (index > GlobalParameters.PlayersInfoList.Count)
                throw new IndexOutOfRangeException("El player no existe");

            return GlobalParameters.PlayersInfoList[index];
        }

        public static PlayerInfo GetGoodGuy(int type)
        {
            var index = type;
            if (index > GlobalParameters.PlayersInfoList.Count)
                throw new IndexOutOfRangeException("El player no existe");

            return GlobalParameters.PlayersInfoList[index];
        }
    }
}
