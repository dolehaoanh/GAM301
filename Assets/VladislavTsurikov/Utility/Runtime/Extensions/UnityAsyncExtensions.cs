#if SCENE_MANAGER_ADDRESSABLES
using System;
using System.Runtime.CompilerServices;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class UnityAsyncExtensions
    {
        public static AddressableAsyncOperationAwaiter GetAwaiter(
            this AsyncOperationHandle<SceneInstance> asyncOperation)
        {
            return new AddressableAsyncOperationAwaiter(asyncOperation);
        }
    }

    public class AddressableAsyncOperationAwaiter : INotifyCompletion
    {
        private AsyncOperationHandle<SceneInstance> _asyncOperationHandle;

        public bool IsCompleted => _asyncOperationHandle.IsDone;

        public AddressableAsyncOperationAwaiter(AsyncOperationHandle<SceneInstance> asyncOperationHandle) =>
            _asyncOperationHandle = asyncOperationHandle;

        public void OnCompleted(Action continuation)
        {
            if (_asyncOperationHandle.IsDone)
            {
                continuation();
            }
            else
            {
                _asyncOperationHandle.Completed += _ =>
                {
                    if (_asyncOperationHandle.IsDone)
                    {
                        continuation();
                    }
                };
            }
        }

        public void GetResult()
        {
        }
    }
}
#endif
