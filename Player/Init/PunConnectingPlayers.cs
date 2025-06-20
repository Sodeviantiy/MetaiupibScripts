using Photon.Pun;

namespace IubipGame.ScriptsGame.Player.Init
{
    public class PunConnectingPlayers : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            SystemInitPlayer.Init();
        }
    }
}
