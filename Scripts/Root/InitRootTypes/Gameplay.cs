using IubipGame.ScriptsGame.Photon.Init;
using IubipGame.ScriptsGame.Player.Init;
using IubipGame.ScriptsGame.Settings;
using IubipGame.ScriptsGame.Utility;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Gameplay : RootMonoBehaviour
{
    public override TypeRoot TypeRoot { get => TypeRoot.Gameplay; }
    public override IEnumerator Run()
    {
        SystemInitPhoton.Init();
        yield return new WaitUntil(() => PhotonNetwork.IsConnected && PhotonNetwork.InRoom);
        SystemInitPlayer.Init();
    }
}
