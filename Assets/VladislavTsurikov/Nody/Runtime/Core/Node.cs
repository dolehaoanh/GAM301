using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Nody.Runtime.Core.Ports;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public abstract class Node : Element, ISelectable, IRemovable
    {
        [OdinSerialize]
        private Vector2 _graphPosition;

        [OdinSerialize]
        private string _nodeId = Guid.NewGuid().ToString();

        [NonSerialized]
        private PortDefinitionContext _portCache;

        [OdinSerialize]
        protected bool _selected;

        //protected internal object Stack;
        [NonSerialized]
        public object Stack;

        public event Action<Node> Dirtied;

        public string NodeId
        {
            get => _nodeId;
            set => _nodeId = value;
        }

        public Vector2 GraphPosition
        {
            get => _graphPosition;
            set => _graphPosition = value;
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                bool changedSelect = _selected != value;

                _selected = value;

                if (changedSelect)
                {
                    if (value)
                    {
                        OnSelect();
                    }
                    else
                    {
                        OnDeselect();
                    }
                }
            }
        }

        void IRemovable.OnRemove()
        {
            IsSetup = false;
            OnDeleteElement();
            ((IDisableable)this).OnDisable();
        }

        public virtual bool IsDeletable()
        {
            return true;
        }

        internal void OnCreateInternal()
        {
            OnCreate();
        }

        protected virtual void OnDeleteElement()
        {
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnDeselect()
        {
        }

        protected virtual void OnSelect()
        {
        }

        public virtual bool DeleteElement()
        {
            return true;
        }

        protected override void OnDirtied()
        {
            Dirtied?.Invoke(this);
        }

        public virtual void OnDefinePorts(PortDefinitionContext context)
        {
        }

        public PortDefinitionContext GetPorts()
        {
            if (_portCache == null)
            {
                _portCache = new PortDefinitionContext();
                OnDefinePorts(_portCache);
            }

            return _portCache;
        }

        protected void InvalidatePorts()
        {
            _portCache = null;
        }

        public virtual void ExecuteInStack(object context)
        {
        }

        public virtual void ExecuteInGraph(Dictionary<string, object> inputs, out Dictionary<string, object> outputs)
        {
            outputs = new Dictionary<string, object>();
        }
    }
}
