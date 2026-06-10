using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public class ScatterStack : NodeStackOnlyDifferentTypes<Scatter>
    {
        private WaitingNextFrame _waitingNextFrame;
        public override bool AllowRemoveLastElement => false;

        public async UniTask Samples(BoxArea boxArea, Action<Vector3> onAddSample, CancellationToken token = default)
        {
            List<Scatter> enabledScatter = new(_elementList.FindAll(scatter => scatter.Active));

            List<Vector3> samples = new();

            for (int i = 0; i < enabledScatter.Count; i++)
            {
                await enabledScatter[i]
                    .Samples(token, boxArea, samples, i == enabledScatter.Count - 1 ? onAddSample : null);
            }
        }

        public void SetWaitingNextFrame(WaitingNextFrame waitingNextFrame)
        {
            _waitingNextFrame = waitingNextFrame;
        }

        public bool IsWaitForNextFrame()
        {
            if (_waitingNextFrame == null)
            {
                return false;
            }

            return _waitingNextFrame.IsWaitForNextFrame();
        }
    }
}
