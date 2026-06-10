#if RENDERER_STACK
#endif
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;
#if UNITY_EDITOR
using GameObjectColliderUtility = VladislavTsurikov.GameObjectCollider.Editor.GameObjectColliderUtility;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public static class UnspawnGameObject
    {
        public static void Unspawn(IReadOnlyList<Prototype> unspawnPrototypes, bool unspawnSelected)
        {
            List<GameObject> unspawnPrefabs = new();

            foreach (Prototype proto in unspawnPrototypes)
            {
                if (unspawnSelected)
                {
                    if (!proto.Selected)
                    {
                        continue;
                    }
                }

                unspawnPrefabs.Add((GameObject)proto.PrototypeObject);
            }

            GameObjectUtility.Unspawn(unspawnPrefabs);

#if UNITY_EDITOR
            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
#endif
        }
    }
}
