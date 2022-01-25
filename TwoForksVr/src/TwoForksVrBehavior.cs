using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoForksVr
{
    public abstract class TwoForksVrBehavior : MonoBehaviour
    {
        private static readonly Dictionary<Type, List<TwoForksVrBehavior>> typeInstanceMap =
            new Dictionary<Type, List<TwoForksVrBehavior>>();

        protected virtual void Awake()
        {
            if (typeInstanceMap.ContainsKey(GetType()))
                typeInstanceMap[GetType()].Add(this);
            else
                typeInstanceMap[GetType()] = new List<TwoForksVrBehavior> {this};
        }

        protected virtual void OnDestroy()
        {
            typeInstanceMap.TryGetValue(GetType(), out var instance);

            instance?.Remove(this);
        }

        protected abstract void VeryLateUpdate();

        public static void InvokeVeryLateUpdate<TBehavior>() where TBehavior : TwoForksVrBehavior
        {
            typeInstanceMap.TryGetValue(typeof(TBehavior), out var instances);
            if (instances == null) return;
            foreach (var instance in instances) instance.InvokeVeryLateUpdateIfEnabled();
        }

        private void InvokeVeryLateUpdateIfEnabled()
        {
            if (!enabled) return;
            VeryLateUpdate();
        }
    }
}