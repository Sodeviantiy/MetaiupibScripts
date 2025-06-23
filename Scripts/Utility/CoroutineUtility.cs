using System;
using System.Collections;
using UnityEngine;

namespace IubipGame.ScriptsGame.Utility
{
    public class CoroutineUtility : MonoBehaviour
    {
        public static IEnumerator WaitByTrue(Func<bool> wFunc)
        {
            while (!wFunc())
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator DoItAfter(Func<bool> wFunc, Action eventAction)
        {
           yield return WaitByTrue(wFunc);
           eventAction?.Invoke();
        }
    }
}
