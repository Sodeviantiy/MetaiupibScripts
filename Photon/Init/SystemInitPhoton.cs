using IubipGame.ScriptsGame.Photon.Setting;
using IubipGame.ScriptsGame.Utility;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace IubipGame.ScriptsGame.Photon.Init
{
    public class SystemInitPhoton : SystemInit
    {
        public GameObject _punGameObject { get; private set; }

        public static void Init()
        {
            new SystemInitPhoton().RunInit();
        }

        protected override void RunInit()
        {
            _punGameObject = new GameObject("[PunPhoton]");
            var componentConnect = _punGameObject.AddComponent<ConnectAndJoinRandom>();
            
            InitPhotonSetting.Instance.SetConnectSetting(ref componentConnect);
            componentConnect.ConnectNow();
            componentConnect.OnJoinedRoom();
            
        }
    }
}
