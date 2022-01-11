using System.Linq;
using HarmonyLib;
using TwoForksVr.Debugging;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoForksVr.UI
{
    public class AttachMenuToCamera: MonoBehaviour
    {
        private BoxCollider collider;
        private vgUIInputModule[] inputModules = new vgUIInputModule[]{};

        private void Start()
        {
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
        
        private void UpdateTransform()
        {
            transform.position = MenuFollowTarget.Instance.transform.position;
            transform.rotation =  MenuFollowTarget.Instance.transform.rotation;
        }
    }
}
