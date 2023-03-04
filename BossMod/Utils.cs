using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BossMod
{
    public static class Utils
    {
        public static Gamelogic GetGameLogic()
        {
            return Gamelogic.instance;
        }
        
        public static Il2CppSystem.Collections.Generic.List<PlayerController> GetAllPlayers()
        {
            return Gamelogic.ActivePlayers;
        }

        public static PlayerController GetLocalPlayer()
        {
            return Gamelogic.instance.ObservedController;
        }

        public static GameObject GetPlayerObject(this PlayerController source)
        {
            return (from obj in GetDontDestroyOnLoadObjects() where obj != null where obj.name.StartsWith("Character_") let comp = obj.GetComponent<PlayerController>() where comp != null where comp == source select obj).FirstOrDefault();
        }

        public static IEnumerable<GameObject> GetDontDestroyOnLoadObjects()
        {
            var results = new List<GameObject>();
            var parent = Main.Container.transform.parent;
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child == null)
                    continue;
                
                results.Add(child.gameObject);
            }
            return results;
        }
    }
}