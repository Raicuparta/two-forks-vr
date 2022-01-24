using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoForksVr
{
    public abstract class TwoForksVrBehavior: MonoBehaviour
    {
        private static readonly Dictionary<Type, Action> updateActions = new Dictionary<Type, Action>();

        protected virtual void Awake()
        {
            if (updateActions.ContainsKey(GetType()))
            {
                updateActions[GetType()] += InvokeVeryLateUpdateIfEnabled;
            }
            else
            {
                updateActions[GetType()] = InvokeVeryLateUpdateIfEnabled;
            }
        }

        protected virtual void OnDestroy()
        {
            updateActions.TryGetValue(GetType(), out var instance);
            if (instance == null) return;
            instance -= InvokeVeryLateUpdateIfEnabled;
        }

        public static void InvokeVeryLateUpdate<TBehavior>() where TBehavior : TwoForksVrBehavior
        {
            updateActions.TryGetValue(typeof(TBehavior), out var updateAction);
            updateAction?.Invoke();
        }

        private void InvokeVeryLateUpdateIfEnabled()
        {
            if (!enabled) return;
            VeryLateUpdate();
        }

        public abstract void VeryLateUpdate();
    }
}