using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility
{
    public static class GetRandomPrototype
    {
        public static Prototype GetMaxSuccessProto(IReadOnlyList<Prototype> protoList)
        {
            if (protoList.Count == 0)
            {
                Debug.Log("You have not selected more than one prototype.");
                return null;
            }

            if (protoList.Count == 1)
            {
                return protoList[0];
            }

            List<float> successProto = new();

            foreach (Prototype proto in protoList)
            {
                SuccessSettings successSettings = (SuccessSettings)proto.GetElement(typeof(SuccessSettings));

                if (!proto.Active)
                {
                    successProto.Add(-1);
                    continue;
                }

                float randomSuccess = Random.Range(0.0f, 1.0f);

                if (randomSuccess > successSettings.SuccessValue / 100)
                {
                    randomSuccess = successSettings.SuccessValue / 100;
                }

                successProto.Add(randomSuccess);
            }

            float maxSuccessProto = successProto.Max();

            int maxIndexProto = successProto.IndexOf(maxSuccessProto);
            return protoList[maxIndexProto];
        }

        public static PlacedObjectPrototype GetRandomSelectedPrototype(Group group)
        {
            List<Prototype> protoList = group.GetAllSelectedPrototypes();

            return (PlacedObjectPrototype)protoList[Random.Range(0, protoList.Count)];
        }
    }
}
