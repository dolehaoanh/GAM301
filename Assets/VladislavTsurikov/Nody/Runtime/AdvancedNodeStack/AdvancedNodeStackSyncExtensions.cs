using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public static class AdvancedNodeStackSyncExtensions
    {
        public static bool ShouldSyncToTypes<TNode>(this AdvancedNodeStack<TNode> stack, Type[] types)
            where TNode : Node
        {
            if (types == null)
            {
                return false;
            }

            if (stack == null || stack.ElementList == null || stack.ElementList.Count != types.Length)
            {
                return true;
            }

            for (int i = 0; i < types.Length; i++)
            {
                Type expectedType = types[i];
                TNode current = stack.ElementList[i];

                if (expectedType == null || current == null)
                {
                    return true;
                }

                if (current.GetType() != expectedType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool SyncToTypesIfNeeded<TNode>(this AdvancedNodeStack<TNode> stack, Type[] types,
            string context = null)
            where TNode : Node
        {
            if (stack == null || types == null)
            {
                return false;
            }

            if (!stack.ShouldSyncToTypes(types))
            {
                return false;
            }

            if (context != null)
            {
                Debug.Log($"[{context}] SyncToTypes triggered. Types count: {types.Length}");
            }

            stack.SyncToTypes(types);
            return true;
        }

        private static void SyncToTypes<TNode>(this AdvancedNodeStack<TNode> stack, Type[] types)
            where TNode : Node
        {
            if (stack == null)
            {
                return;
            }

            if (stack.List.Count == 0)
            {
                if (types == null)
                {
                    return;
                }

                for (int i = 0; i < types.Length; i++)
                {
                    CreateForSync(stack, types[i]);
                }

                return;
            }

            stack.RemoveInvalidElements();

            if (types == null)
            {
                return;
            }

            stack.IsDirty = true;

            Dictionary<Type, Queue<TNode>> pool = new();
            for (int i = 0; i < stack.List.Count; i++)
            {
                TNode element = stack.List[i];
                Type elementType = element.GetType();

                if (!pool.TryGetValue(elementType, out Queue<TNode> queue))
                {
                    queue = new Queue<TNode>();
                    pool.Add(elementType, queue);
                }

                queue.Enqueue(element);
            }

            List<TNode> newList = new(types.Length);
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                if (pool.TryGetValue(type, out Queue<TNode> queue) && queue.Count > 0)
                {
                    newList.Add(queue.Dequeue());
                    continue;
                }

                TNode created = CreateForSync(stack, type);
                if (created != null)
                {
                    newList.Add(created);
                }
            }

            HashSet<TNode> keep = new(newList);
            for (int i = stack.List.Count - 1; i >= 0; i--)
            {
                if (!keep.Contains(stack.List[i]))
                {
                    stack.Remove(i);
                }
            }

            stack.List.Clear();
            stack.List.AddRange(newList);
        }

        private static TNode CreateForSync<TNode>(AdvancedNodeStack<TNode> stack, Type type)
            where TNode : Node
        {
            if (type == null || !stack.AllowCreate(type))
            {
                return null;
            }

            if (type.GetAttribute(typeof(DontCreateAttribute)) != null)
            {
                return null;
            }

            TNode element = stack.Instantiate(type, false);
            if (element == null)
            {
                return null;
            }

            stack.List.Add(element);
            if (stack.List.Count == 1)
            {
                element.Selected = true;
            }

            element.Stack = stack;
            element.SetupWithSetupData(true, stack.SetupData);
            element.OnCreateInternal();
            stack.IsDirty = true;

            return element;
        }
    }
}
