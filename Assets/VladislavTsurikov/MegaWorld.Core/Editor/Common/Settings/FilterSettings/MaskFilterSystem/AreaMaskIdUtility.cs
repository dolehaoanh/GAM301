#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.TerrainUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public static class AreaMaskIdUtility
    {
        public static string[] GetIds()
        {
            AreaMask[] areaMasks = Object.FindObjectsByType<AreaMask>(FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            HashSet<string> ids = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < areaMasks.Length; i++)
            {
                AreaMask areaMask = areaMasks[i];
                if (areaMask == null)
                {
                    continue;
                }

                IReadOnlyList<AreaMaskEntry> entries = areaMask.Masks;
                for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
                {
                    AreaMaskEntry entry = entries[entryIndex];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.Id))
                    {
                        continue;
                    }

                    ids.Add(entry.Id);
                }
            }

            return ids.OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToArray();
        }
    }
}
#endif
