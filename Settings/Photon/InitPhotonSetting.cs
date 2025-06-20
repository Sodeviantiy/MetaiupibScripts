using IubipGame.ScriptsGame.Utility;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

namespace IubipGame.ScriptsGame.Photon.Setting
{
    [CreateAssetMenu(fileName = "InitPhotonSetting", menuName = "PROJECT_SETTING/InitPhotonSetting", order = 1)]
    public class InitPhotonSetting : ScriptableGlobalSetting<InitPhotonSetting>
    {

        [SerializeField] private bool _autoConnect = true;
        [SerializeField] private int _version = 1;
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private int _playerTTL = -1;

        private ConnectAndJoinRandom _connectSetting;

        public void SetConnectSetting(ref ConnectAndJoinRandom instantComponent)
        {
            instantComponent.AutoConnect = _autoConnect;
            instantComponent.Version = (byte)_version;
            instantComponent.MaxPlayers = (byte)_maxPlayers;
            instantComponent.playerTTL = _playerTTL;

            _connectSetting = instantComponent;
        }

    }
}
