using UnityEngine;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack
{
    public interface IShowIcon : ISelectable
    {
#if UNITY_EDITOR
        string Name { get; }
        bool IsRedIcon { get; }

        Texture2D PreviewTexture { get; }
#endif
    }
}
