using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace TwoForksVr.UI
{
    public class AttachMenuToCamera : AttachToCamera
    {
        private const float offset = 3f;
        private BoxCollider collider;
        private vgUIInputModule[] inputModules = new vgUIInputModule[]{};
        
        private void OnEnable()
        {
            UpdateTransform();
        }

        private void Start()
        {
            UpdateTransform();
            SetUpCollider();
            SetUpInputModule();
        }

        private void Update()
        {
            UpdateTransform();
            
            var active = IsAnyInputModuleActive();
            if (active && !collider.enabled)
            {
                collider.enabled = true;
            }
            else if (!active && collider.enabled)
            {
                collider.enabled = false;
            }
        }

        private bool IsAnyInputModuleActive()
        {
            if (inputModules.Length == 0) return false;
            foreach (var inputModule in inputModules)
            {
                if (inputModule && inputModule.gameObject.activeInHierarchy && inputModule.enabled)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetUpInputModule()
        {
            // TODO clean this up.
            
            var rootSceneObjects = gameObject.scene.GetRootGameObjects();
            foreach (var sceneObject in rootSceneObjects)
            {
                var modules = sceneObject.GetComponentsInChildren<vgUIInputModule>(true);
                foreach (var module in modules)
                {
                    inputModules = modules.AddItem(module).ToArray();
                }
            }
        }
        
        private void SetUpCollider()
        {
            collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null) return;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            collider = gameObject.gameObject.AddComponent<BoxCollider>();
            var rectSize = rectTransform.sizeDelta;
            collider.size = new Vector3(rectSize.x, rectSize.y, 0.1f);
            gameObject.layer = LayerMask.NameToLayer("UI");
            gameObject.GetComponent<Canvas>().worldCamera = Camera.main; // TODO update this from cameratransform?
            
            gameObject.AddComponent<DebugCollider>();
        }
        
        protected override void HandleTargetCameraSet()
        {
            transform.position = GetTargetPosition();
            velocity = Vector3.zero;
        }

        private static Vector3 GetTargetPosition()
        {
            if (!CameraTransform) return Vector3.zero;

            var cameraPosition = CameraTransform.position;
            var forward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up).normalized;
            return cameraPosition + forward * offset;
        }
        
        private float maxSquareDistance = 3f;
        
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        private Vector3? currentTarget;
        
        private void UpdateTransform()
        {
            if (!CameraTransform) return;
            var targetThisFrame = GetTargetPosition();

            var squareDistance = Vector3.SqrMagnitude(targetThisFrame - transform.position);

            currentTarget = (squareDistance > maxSquareDistance || currentTarget == null) ? targetThisFrame : currentTarget;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                currentTarget ?? targetThisFrame,
                ref velocity,
                smoothTime,
                float.PositiveInfinity,
                Time.unscaledDeltaTime);
            transform.LookAt(2 * transform.position - CameraTransform.position);
        }
    }
}
