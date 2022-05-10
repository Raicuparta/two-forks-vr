using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#endif

namespace LIV.SDK.Unity
{
    [Flags]
    public enum INVALIDATION_FLAGS : uint
    {
        NONE = 0,
        HMD_CAMERA = 1,
        STAGE = 2,
        MR_CAMERA_PREFAB = 4,
        EXCLUDE_BEHAVIOURS = 8
    }

    /// <summary>
    ///     The LIV SDK provides a spectator view of your application.
    /// </summary>
    /// <remarks>
    ///     <para>It contextualizes what the user feels & experiences by capturing their body directly inside your world!</para>
    ///     <para>Thanks to our software, creators can film inside your app and have full control over the camera.</para>
    ///     <para>With the power of out-of-engine compositing, a creator can express themselves freely without limits;</para>
    ///     <para>as a real person or an avatar!</para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// public class StartFromScriptExample : MonoBehaviour
    /// {
    ///     [SerializeField] Camera _hmdCamera;
    ///     [SerializeField] Transform _stage;
    ///     [SerializeField] Transform _stageTransform;
    ///     [SerializeField] Camera _mrCameraPrefab;
    ///     LIV.SDK.Unity.LIV _liv;
    ///     private void OnEnable()
    ///     {
    ///         _liv = gameObject.AddComponent<LIV.SDK.Unity.LIV>
    ///             ();
    ///             _liv.HMDCamera = _hmdCamera;
    ///             _liv.stage = _stage;
    ///             _liv.stageTransform = _stageTransform;
    ///             _liv.MRCameraPrefab = _mrCameraPrefab;
    ///             }
    ///             private void OnDisable()
    ///             {
    ///             if (_liv == null) return;
    ///             Destroy(_liv);
    ///             _liv = null;
    ///             }
    ///             }
    /// </code>
    /// </example>
    [HelpURL("https://liv.tv/sdk-unity-docs")]
    [AddComponentMenu("LIV/LIV")]
    public class LIV : MonoBehaviour
    {
        [Tooltip("This is the topmost transform of your VR rig.")]
        [FormerlySerializedAs("TrackedSpaceOrigin")]
        [SerializeField]
        private Transform _stage;

        [Tooltip("This transform is an additional wrapper to the user’s playspace.")]
        [FormerlySerializedAs("StageTransform")]
        [SerializeField]
        private Transform _stageTransform;

        [Tooltip("This is the camera responsible for rendering the user’s HMD.")]
        [FormerlySerializedAs("HMDCamera")]
        [SerializeField]
        private Camera _HMDCamera;

        [Tooltip("Camera prefab for customized rendering.")] [FormerlySerializedAs("MRCameraPrefab")] [SerializeField]
        private Camera _MRCameraPrefab;

        [Tooltip("This option disables all standard Unity assets for the Mixed Reality rendering.")]
        [FormerlySerializedAs("DisableStandardAssets")]
        [SerializeField]
        private bool _disableStandardAssets;

        [Tooltip("The layer mask defines exactly which object layers should be rendered in MR.")]
        [FormerlySerializedAs("SpectatorLayerMask")]
        [SerializeField]
        private LayerMask _spectatorLayerMask = ~0;

        [Tooltip("This is for removing unwanted scripts from the cloned MR camera.")]
        [FormerlySerializedAs("ExcludeBehaviours")]
        [SerializeField]
        private string[] _excludeBehaviours =
        {
            "AudioListener",
            "Collider",
            "SteamVR_Camera",
            "SteamVR_Fade",
            "SteamVR_ExternalCamera"
        };


        /// <summary>
        ///     Recovers corrupted alpha channel when using post-effects.
        /// </summary>
        [Tooltip("Recovers corrupted alpha channel when using post-effects.")]
        [FormerlySerializedAs("FixPostEffectsAlpha")]
        [SerializeField]
        private bool _fixPostEffectsAlpha;

        private bool _enabled;
        private string[] _excludeBehavioursCandidate;
        private Camera _HMDCameraCandidate;

        private INVALIDATION_FLAGS _invalidate = INVALIDATION_FLAGS.NONE;

        private Camera _MRCameraPrefabCandidate;

        private Transform _stageCandidate;
        private Coroutine _waitForEndOfFrameCoroutine;

        private bool _wasReady;

        /// <summary>
        ///     triggered when the LIV SDK is activated by the LIV App and enabled by the game.
        /// </summary>
        public Action onActivate = null;

        /// <summary>
        ///     triggered when the LIV SDK is deactivated by the LIV App or disabled by the game.
        /// </summary>
        public Action onDeactivate = null;

        /// <summary>
        ///     triggered after the Mixed Reality camera has finished rendering.
        /// </summary>
        public Action<SDKRender> onPostRender = null;

        /// <summary>
        ///     triggered after the LIV SDK starts rendering background image.
        /// </summary>
        public Action<SDKRender> onPostRenderBackground = null;

        /// <summary>
        ///     triggered after the LIV SDK starts rendering the foreground image.
        /// </summary>
        public Action<SDKRender> onPostRenderForeground = null;

        /// <summary>
        ///     triggered before the Mixed Reality camera is about to render.
        /// </summary>
        public Action<SDKRender> onPreRender = null;

        /// <summary>
        ///     triggered before the LIV SDK starts rendering background image.
        /// </summary>
        public Action<SDKRender> onPreRenderBackground = null;

        /// <summary>
        ///     triggered before the LIV SDK starts rendering the foreground image.
        /// </summary>
        public Action<SDKRender> onPreRenderForeground = null;

        /// <summary>
        ///     This is the topmost transform of your VR rig.
        /// </summary>
        /// <remarks>
        ///     <para>When implementing VR locomotion(teleporting, joystick, etc),</para>
        ///     <para>this is the GameObject that you should move around your scene.</para>
        ///     <para>It represents the centre of the user’s playspace.</para>
        /// </remarks>
        public Transform stage
        {
            get => _stage == null ? transform.parent : _stage;
            set
            {
                if (value == null) Debug.LogWarning("LIV: Stage cannot be null!");

                if (_stage != value)
                {
                    _stageCandidate = value;
                    _invalidate =
                        (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.STAGE,
                            true);
                }
            }
        }

        [Obsolete("Use stage instead")]
        public Transform trackedSpaceOrigin
        {
            get => stage;
            set => stage = value;
        }

        public Matrix4x4 stageLocalToWorldMatrix => stage != null ? stage.localToWorldMatrix : Matrix4x4.identity;

        public Matrix4x4 stageWorldToLocalMatrix => stage != null ? stage.worldToLocalMatrix : Matrix4x4.identity;

        /// <summary>
        ///     This transform is an additional wrapper to the user’s playspace.
        /// </summary>
        /// <remarks>
        ///     <para>It allows for user-controlled transformations for special camera effects & transitions.</para>
        ///     <para>If a creator is using a static camera, this transformation can give the illusion of camera movement.</para>
        /// </remarks>
        public Transform stageTransform
        {
            get => _stageTransform;
            set => _stageTransform = value;
        }

        /// <summary>
        ///     This is the camera responsible for rendering the user’s HMD.
        /// </summary>
        /// <remarks>
        ///     <para>The LIV SDK, by default clones this object to match your application’s rendering setup.</para>
        ///     <para>You can use your own camera prefab should you want to!</para>
        /// </remarks>
        public Camera HMDCamera
        {
            get => _HMDCamera;
            set
            {
                if (value == null) Debug.LogWarning("LIV: HMD Camera cannot be null!");

                if (_HMDCamera != value)
                {
                    _HMDCameraCandidate = value;
                    _invalidate = (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate,
                        (uint) INVALIDATION_FLAGS.HMD_CAMERA, true);
                }
            }
        }

        /// <summary>
        ///     Camera prefab for customized rendering.
        /// </summary>
        /// <remarks>
        ///     <para>By default, LIV uses the HMD camera as a reference for the Mixed Reality camera.</para>
        ///     <para>It is cloned and set up as a Mixed Reality camera.This approach works for most apps.</para>
        ///     <para>However, some games can experience issues because of custom MonoBehaviours attached to this camera.</para>
        ///     <para>You can use a custom camera prefab for those cases.</para>
        /// </remarks>
        public Camera MRCameraPrefab
        {
            get => _MRCameraPrefab;
            set
            {
                if (_MRCameraPrefab != value)
                {
                    _MRCameraPrefabCandidate = value;
                    _invalidate = (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate,
                        (uint) INVALIDATION_FLAGS.MR_CAMERA_PREFAB, true);
                }
            }
        }

        /// <summary>
        ///     This option disables all standard Unity assets for the Mixed Reality rendering.
        /// </summary>
        /// <remarks>
        ///     <para>Unity’s standard assets can interfere with the alpha channel that LIV needs to composite MR correctly.</para>
        /// </remarks>
        public bool disableStandardAssets
        {
            get => _disableStandardAssets;
            set => _disableStandardAssets = value;
        }

        /// <summary>
        ///     The layer mask defines exactly which object layers should be rendered in MR.
        /// </summary>
        /// <remarks>
        ///     <para>You should use this to hide any in-game avatar that you’re using.</para>
        ///     <para>LIV is meant to include the user’s body for you!</para>
        ///     <para>Certain HMD-based effects should be disabled here too.</para>
        ///     <para>Also, this can be used to render special effects or additional UI only to the MR camera.</para>
        ///     <para>Useful for showing the player’s health, or current score!</para>
        /// </remarks>
        public LayerMask spectatorLayerMask
        {
            get => _spectatorLayerMask;
            set => _spectatorLayerMask = value;
        }

        /// <summary>
        ///     This is for removing unwanted scripts from the cloned MR camera.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         By default, we remove the AudioListener, Colliders and SteamVR scripts, as these are not necessary for
        ///         rendering MR!
        ///     </para>
        ///     <para>The excluded string must match the name of the MonoBehaviour.</para>
        /// </remarks>
        public string[] excludeBehaviours
        {
            get => _excludeBehaviours;
            set
            {
                if (_excludeBehaviours != value)
                {
                    _excludeBehavioursCandidate = value;
                    _invalidate = (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate,
                        (uint) INVALIDATION_FLAGS.EXCLUDE_BEHAVIOURS, true);
                }
            }
        }

        public bool fixPostEffectsAlpha
        {
            get => _fixPostEffectsAlpha;
            set => _fixPostEffectsAlpha = value;
        }

        /// <summary>
        ///     Is the curret LIV SDK setup valid.
        /// </summary>
        public bool isValid
        {
            get
            {
                if (_invalidate != INVALIDATION_FLAGS.NONE) return false;

                if (_HMDCamera == null)
                {
                    Debug.LogError("LIV: HMD Camera is a required parameter!");
                    return false;
                }

                if (_stage == null) Debug.LogWarning("LIV: Tracked space origin should be assigned!");

                if (_spectatorLayerMask == 0)
                    Debug.LogWarning("LIV: The spectator layer mask is set to not show anything. Is this correct?");

                return true;
            }
        }

        /// <summary>
        ///     Is the LIV SDK currently active.
        /// </summary>
        public bool isActive { get; private set; }

        private bool _isReady => isValid && _enabled && SDKBridge.IsActive;

        /// <summary>
        ///     Script responsible for the MR rendering.
        /// </summary>
        public SDKRender render { get; private set; }

        private void Update()
        {
            UpdateSDKReady();
            Invalidate();
        }

        private void OnEnable()
        {
            _enabled = true;
            UpdateSDKReady();
        }

        private void OnDisable()
        {
            _enabled = false;
            UpdateSDKReady();
        }

        private IEnumerator WaitForUnityEndOfFrame()
        {
            while (Application.isPlaying && enabled)
            {
                yield return new WaitForEndOfFrame();
                if (isActive) render.Render();
            }
        }

        private void UpdateSDKReady()
        {
            var ready = _isReady;
            if (ready != _wasReady)
            {
                OnSDKReadyChanged(ready);
                _wasReady = ready;
            }
        }

        private void OnSDKReadyChanged(bool value)
        {
            if (value)
                OnSDKActivate();
            else
                OnSDKDeactivate();
        }

        private void OnSDKActivate()
        {
            Debug.Log("LIV: Compositor connected, setting up Mixed Reality!");
            SubmitSDKOutput();
            CreateAssets();
            StartRenderCoroutine();
            isActive = true;
            if (onActivate != null) onActivate.Invoke();
        }

        private void OnSDKDeactivate()
        {
            Debug.Log("LIV: Compositor disconnected, cleaning up Mixed Reality.");
            if (onDeactivate != null) onDeactivate.Invoke();
            StopRenderCoroutine();
            DestroyAssets();
            isActive = false;
        }

        private void CreateAssets()
        {
            DestroyAssets();
            render = new SDKRender(this);
        }

        private void DestroyAssets()
        {
            if (render != null)
            {
                render.Dispose();
                render = null;
            }
        }

        private void StartRenderCoroutine()
        {
            StopRenderCoroutine();
            _waitForEndOfFrameCoroutine = StartCoroutine(WaitForUnityEndOfFrame());
        }

        private void StopRenderCoroutine()
        {
            if (_waitForEndOfFrameCoroutine != null)
            {
                StopCoroutine(_waitForEndOfFrameCoroutine);
                _waitForEndOfFrameCoroutine = null;
            }
        }

        private void SubmitSDKOutput()
        {
            var output = SDKApplicationOutput.empty;
            output.supportedFeatures = FEATURES.BACKGROUND_RENDER |
                                       FEATURES.FOREGROUND_RENDER |
                                       FEATURES.OVERRIDE_POST_PROCESSING |
                                       FEATURES.FIX_FOREGROUND_ALPHA;

            output.sdkID = SDKConstants.SDK_ID;
            output.sdkVersion = SDKConstants.SDK_VERSION;
            output.engineName = SDKConstants.ENGINE_NAME;
            output.engineVersion = Application.unityVersion;
            output.applicationName = Application.productName;
            output.applicationVersion = Application.version;
            output.graphicsAPI = SystemInfo.graphicsDeviceType.ToString();
#if UNITY_2017_2_OR_NEWER
            output.xrDeviceName = XRSettings.loadedDeviceName;
#endif
            SDKBridge.SubmitApplicationOutput(output);
        }

        private void Invalidate()
        {
            if (SDKUtils.ContainsFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.STAGE))
            {
                _stage = _stageCandidate;
                _stageCandidate = null;
                _invalidate =
                    (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.STAGE, false);
            }

            if (SDKUtils.ContainsFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.HMD_CAMERA))
            {
                _HMDCamera = _HMDCameraCandidate;
                _HMDCameraCandidate = null;
                _invalidate =
                    (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.HMD_CAMERA,
                        false);
            }

            if (SDKUtils.ContainsFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.MR_CAMERA_PREFAB))
            {
                _MRCameraPrefab = _MRCameraPrefabCandidate;
                _MRCameraPrefabCandidate = null;
                _invalidate = (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate,
                    (uint) INVALIDATION_FLAGS.MR_CAMERA_PREFAB, false);
            }

            if (SDKUtils.ContainsFlag((uint) _invalidate, (uint) INVALIDATION_FLAGS.EXCLUDE_BEHAVIOURS))
            {
                _excludeBehaviours = _excludeBehavioursCandidate;
                _excludeBehavioursCandidate = null;
                _invalidate = (INVALIDATION_FLAGS) SDKUtils.SetFlag((uint) _invalidate,
                    (uint) INVALIDATION_FLAGS.EXCLUDE_BEHAVIOURS, false);
            }
        }
    }
}