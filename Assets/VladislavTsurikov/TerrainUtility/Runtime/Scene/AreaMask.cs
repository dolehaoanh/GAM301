using System;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.TerrainUtility.Runtime
{
    public sealed class AreaMask : MonoBehaviour
    {
        [SerializeField]
        private AreaMaskEntry[] _masks = Array.Empty<AreaMaskEntry>();

        public IReadOnlyList<AreaMaskEntry> Masks => _masks;

        public string[] GetMaskIds()
        {
            if (_masks == null || _masks.Length == 0)
            {
                return Array.Empty<string>();
            }

            List<string> ids = new(_masks.Length);
            for (int i = 0; i < _masks.Length; i++)
            {
                AreaMaskEntry mask = _masks[i];
                if (mask == null || string.IsNullOrWhiteSpace(mask.Id))
                {
                    continue;
                }

                ids.Add(mask.Id);
            }

            return ids.ToArray();
        }

        public void SetMasks(IReadOnlyList<AreaMaskEntry> masks)
        {
            if (masks == null || masks.Count == 0)
            {
                _masks = Array.Empty<AreaMaskEntry>();
                return;
            }

            _masks = new AreaMaskEntry[masks.Count];
            for (int i = 0; i < masks.Count; i++)
            {
                AreaMaskEntry source = masks[i];
                _masks[i] = new AreaMaskEntry { Id = source?.Id ?? string.Empty, Mask = source?.Mask };
            }
        }

        public bool TryGetMask(string id, out Texture2D mask)
        {
            mask = null;

            if (string.IsNullOrWhiteSpace(id) || _masks == null)
            {
                return false;
            }

            for (int i = 0; i < _masks.Length; i++)
            {
                AreaMaskEntry areaMaskEntry = _masks[i];
                if (areaMaskEntry == null)
                {
                    continue;
                }

                if (!string.Equals(areaMaskEntry.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                mask = areaMaskEntry.Mask;
                return mask != null;
            }

            return false;
        }
    }
}
