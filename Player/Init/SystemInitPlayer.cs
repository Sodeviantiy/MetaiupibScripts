using IubipGame.ScriptsGame.Player.Setting;
using IubipGame.ScriptsGame.Utility;
using Photon.Pun;
using UnityEngine;

namespace IubipGame.ScriptsGame.Player.Init
{
    public class SystemInitPlayer : SystemInit
    {
        private static InitPlayerSetting _settingInit => InitPlayerSetting.Instance;
        public static void Init()
        {
            new SystemInitPlayer().RunInit();
        }
        protected override void RunInit()
        {
            var PointSpawn = _settingInit.GetRandomPointToSpawn();
            GameObject CharacterInstantiated = _settingInit.GetPrefabPlayer().gameObject;
            PhotonNetwork.Instantiate(CharacterInstantiated.name, PointSpawn, default);
        }
    }
}
