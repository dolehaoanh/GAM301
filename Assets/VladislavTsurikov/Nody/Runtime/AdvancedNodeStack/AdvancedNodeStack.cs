using System;
using System.Linq;
using OdinSerializer.Utilities;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public abstract class AdvancedNodeStack<T> : NodeStack<T>
        where T : Node
    {
        /// <summary>
        ///     A public link to the list is required for UnityEditorInternal.ReorderableList
        /// </summary>
        public ObservableList<T> List => _elementList;

        protected virtual bool SupportsPersistentNodes => false;

        private protected override void CreateElements()
        {
            OnCreateElements();

            GetType().GetAttribute<CreateNodesAttribute>()?.Types
                .ForEach(type => CreateElementIfMissingType(type));

            if (SupportsPersistentNodes)
            {
                AllTypesDerivedFrom<T>.Types
                    .Where(type => type.GetAttribute<PersistentAttribute>() != null)
                    .ForEach(type => CreateElementIfMissingType(type));
            }
        }

        protected virtual void OnCreateElements()
        {
        }

        public void CreateAllElementTypes()
        {
            CreateElementIfMissingType(AllTypesDerivedFrom<T>.Types);
        }

        public T2 GetElement<T2>() where T2 : Node
        {
            object node = GetElement(typeof(T2), out _);

            if (node == null)
            {
                return null;
            }

            return (T2)node;
        }

        protected void CreateElementIfMissingType(Type[] types)
        {
            foreach (Type type in types)
            {
                CreateElementIfMissingType(type);
            }
        }

        protected T CreateElementIfMissingType(Type type)
        {
            T settings = GetElement(type);
            if (settings == null)
            {
                return Create(type);
            }

            return settings;
        }

        protected void AddElementIfMissingType(T element)
        {
            T settings = GetElement(element.GetType());

            if (settings == null)
            {
                Add(element);
            }
        }
    }
}
