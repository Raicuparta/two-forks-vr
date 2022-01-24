using System;
using System.Collections.Generic;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr
{
    public abstract class TwoForksVrBehavior: MonoBehaviour
    {
        private static readonly Dictionary<Type, List<TwoForksVrBehavior>> typeInstanceMap = new Dictionary<Type, List<TwoForksVrBehavior>>();

        protected virtual void Awake()
        {
            if (typeInstanceMap.ContainsKey(GetType()))
            {
                typeInstanceMap[GetType()].Add(this);
            }
            else
            {
                typeInstanceMap[GetType()] = new List<TwoForksVrBehavior>() {this};
            }
        }

        protected virtual void OnDestroy()
        {
            Logs.LogInfo($"########## destroying {GetType().Name} {name}");
            typeInstanceMap.TryGetValue(GetType(), out var instance);
            if (instance == null)
            {
                Logs.LogError($"########## UNABLE TO DESTROY {GetType().Name} {name}");
                return;
            }

            instance.Remove(this);
        }

        public static void InvokeVeryLateUpdate<TBehavior>() where TBehavior : TwoForksVrBehavior
        {
            typeInstanceMap.TryGetValue(typeof(TBehavior), out var instances);
            if (instances == null) return;
            foreach (var instance in instances)
            {
                instance.InvokeVeryLateUpdateIfEnabled();
            }
        }

        private void InvokeVeryLateUpdateIfEnabled()
        {
            if (!this)
            {
                Logs.LogError($"###### trying to update deleted object");
                return;
            }
            if (!enabled) return;
            VeryLateUpdate();
        }

        public abstract void VeryLateUpdate();
    }
}