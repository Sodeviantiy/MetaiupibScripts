using IubipGame.ScriptsGame.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IubipGame.ScriptsGame.Player.Setting
{

    [CreateAssetMenu(fileName = "InitPlayerSetting", menuName = "PROJECT_SETTING/InitPlayerSetting", order = 1)]
    public class InitPlayerSetting : ScriptableGlobalSetting<InitPlayerSetting>
    {

        [SerializeField] private Vector3[] PointsSpawnPlayer;
        [SerializeField] private PersonController PlayerPrefab;
        
        public override void DebugOnGizmos()
        {
            foreach (var Point in PointsSpawnPlayer)
            {
                Gizmos.DrawWireSphere(Point, 2);
            }
        }

        public PersonController GetPrefabPlayer()
        {
            return PlayerPrefab;
        }

        public Vector3 GetRandomPointToSpawn()
        {
            return PointsSpawnPlayer[Random.Range(0, PointsSpawnPlayer.Length)];
        }
    }

}
