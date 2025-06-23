
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR

  #endif
namespace IubipGame.ScriptsGame.Settings.Scenes
{
    [CreateAssetMenu(fileName = "Profile", menuName = "PROJECT_SETTING/ProfileScenes", order = 1)]
    public class LoadingScenes : ScriptableObject
    {
        [SerializeField]
        public List<DataScene> AllSceneProfile = new();
        
        private void OnValidate()
        {
            if (!AllSceneProfile.Any())
                return;

            var validateScene = AllSceneProfile.Where(Element => Element.BootType != TypeRoot.Null).Where(Element => AllSceneProfile.Count(x => x.BootType == Element.BootType) > 1);
            
            foreach (DataScene Element in validateScene)
            {
                Element.BootType = TypeRoot.Null;
                Debug.LogWarning("Такая сцена уже существует");
            }
            AllSceneProfile.Sort(((x, y) => y.BootType - x.BootType));
        }
    }
}
