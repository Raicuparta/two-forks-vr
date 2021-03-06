#if !LIV_UNIVERSAL_RENDER
using System;
using TwoForksVr.Assets;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace LIV.SDK.Unity
{
    public partial class SDKRender : IDisposable
    {
        private readonly CameraEvent _applyTextureEvent = CameraEvent.AfterEverything;
        private readonly CameraEvent _captureTextureEvent = CameraEvent.BeforeImageEffects;

        private readonly CameraEvent _clipPlaneCameraEvent = CameraEvent.AfterForwardOpaque;

        private readonly CameraEvent _clipPlaneCombineAlphaCameraEvent = CameraEvent.AfterEverything;

        // Renders captured texture
        private CommandBuffer _applyTextureCommandBuffer;

        private RenderTexture _backgroundRenderTexture;

        // Captures texture before post-effects
        private CommandBuffer _captureTextureCommandBuffer;

        // Renders the clip plane in the foreground texture
        private CommandBuffer _clipPlaneCommandBuffer;

        // Tessellated height map clear material for visual debugging
        private Material _clipPlaneComplexDebugMaterial;

        // Tessellated height map clear material
        private Material _clipPlaneComplexMaterial;

        // Tessellated quad
        private Mesh _clipPlaneMesh;

        // Transparent material for visual debugging
        private Material _clipPlaneSimpleDebugMaterial;

        // Clear material
        private Material _clipPlaneSimpleMaterial;

        // Renders the clipped opaque content in to the foreground texture alpha
        private CommandBuffer _combineAlphaCommandBuffer;
        private Material _combineAlphaMaterial;
        private RenderTexture _complexClipPlaneRenderTexture;
        private Material _forceForwardRenderingMaterial;
        private RenderTexture _foregroundRenderTexture;

        private CameraEvent _optimizedRenderingCameraEvent = CameraEvent.AfterEverything;

        // Renders background and foreground in single render
        private CommandBuffer _optimizedRenderingCommandBuffer;
        private RenderTexture _optimizedRenderTexture;
        private Material _writeMaterial;
        private Material _writeOpaqueToAlphaMaterial;

        public SDKRender(LIV liv)
        {
            this.liv = liv;
            CreateAssets();
        }

        private bool useDeferredRendering =>
            cameraInstance.actualRenderingPath == RenderingPath.DeferredLighting ||
            cameraInstance.actualRenderingPath == RenderingPath.DeferredShading;

        private bool interlacedRendering => SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.INTERLACED_RENDER);

        private bool canRenderBackground
        {
            get
            {
                if (interlacedRendering)
                    // Render only if frame is even 
                    if (Time.frameCount % 2 != 0)
                        return false;
                return SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.BACKGROUND_RENDER) &&
                       _backgroundRenderTexture != null;
            }
        }

        private bool canRenderForeground
        {
            get
            {
                if (interlacedRendering)
                    // Render only if frame is odd 
                    if (Time.frameCount % 2 != 1)
                        return false;
                return SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.FOREGROUND_RENDER) &&
                       _foregroundRenderTexture != null;
            }
        }

        private bool canRenderOptimized => SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.OPTIMIZED_RENDER) &&
                                           _optimizedRenderTexture != null;

        public void Dispose()
        {
            ReleaseBridgePoseControl();
            DestroyAssets();
            SDKUtils.DestroyTexture(ref _backgroundRenderTexture);
            SDKUtils.DestroyTexture(ref _foregroundRenderTexture);
            SDKUtils.DestroyTexture(ref _optimizedRenderTexture);
            SDKUtils.DestroyTexture(ref _complexClipPlaneRenderTexture);
        }

        private Material GetClipPlaneMaterial(bool debugClipPlane, bool complexClipPlane, ColorWriteMask colorWriteMask)
        {
            Material output;

            if (complexClipPlane)
            {
                output = debugClipPlane ? _clipPlaneComplexDebugMaterial : _clipPlaneComplexMaterial;
                output.SetTexture(SDKShaders.LIV_CLIP_PLANE_HEIGHT_MAP_PROPERTY, _complexClipPlaneRenderTexture);
                output.SetFloat(SDKShaders.LIV_TESSELLATION_PROPERTY, _inputFrame.clipPlane.tesselation);
            }
            else
            {
                output = debugClipPlane ? _clipPlaneSimpleDebugMaterial : _clipPlaneSimpleMaterial;
            }

            output.SetInt(SDKShaders.LIV_COLOR_MASK, (int) colorWriteMask);
            return output;
        }

        private Material GetGroundClipPlaneMaterial(bool debugClipPlane, ColorWriteMask colorWriteMask)
        {
            Material output;
            output = debugClipPlane ? _clipPlaneSimpleDebugMaterial : _clipPlaneSimpleMaterial;
            output.SetInt(SDKShaders.LIV_COLOR_MASK, (int) colorWriteMask);
            return output;
        }

        public void Render()
        {
            UpdateBridgeResolution();
            UpdateBridgeInputFrame();
            SDKUtils.ApplyUserSpaceTransform(this);
            UpdateTextures();
            InvokePreRender();
            if (canRenderBackground) RenderBackground();
            if (canRenderForeground) RenderForeground();
            if (canRenderOptimized) RenderOptimized();
            IvokePostRender();
            SDKUtils.CreateBridgeOutputFrame(this);
            SDKBridge.IssuePluginEvent();
        }

        // Default render without any special changes
        private void RenderBackground()
        {
            SDKUtils.SetCamera(cameraInstance, cameraInstance.transform, _inputFrame, localToWorldMatrix,
                spectatorLayerMask);
            cameraInstance.targetTexture = _backgroundRenderTexture;

            RenderTexture tempRenderTexture = null;

            var overridePostProcessing =
                SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.OVERRIDE_POST_PROCESSING);
            if (overridePostProcessing)
            {
                tempRenderTexture = RenderTexture.GetTemporary(_backgroundRenderTexture.width,
                    _backgroundRenderTexture.height, 0, _backgroundRenderTexture.format);
#if UNITY_EDITOR
                tempRenderTexture.name = "LIV.TemporaryRenderTexture";
#endif
                _captureTextureCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tempRenderTexture);
                _applyTextureCommandBuffer.Blit(tempRenderTexture, BuiltinRenderTextureType.CurrentActive);

                cameraInstance.AddCommandBuffer(_captureTextureEvent, _captureTextureCommandBuffer);
                cameraInstance.AddCommandBuffer(_applyTextureEvent, _applyTextureCommandBuffer);
            }

            SDKShaders.StartRendering();
            SDKShaders.StartBackgroundRendering();
            InvokePreRenderBackground();
            SendTextureToBridge(_backgroundRenderTexture, TEXTURE_ID.BACKGROUND_COLOR_BUFFER_ID);
            cameraInstance.Render();
            InvokePostRenderBackground();
            cameraInstance.targetTexture = null;
            SDKShaders.StopBackgroundRendering();
            SDKShaders.StopRendering();

            if (overridePostProcessing)
            {
                cameraInstance.RemoveCommandBuffer(_captureTextureEvent, _captureTextureCommandBuffer);
                cameraInstance.RemoveCommandBuffer(_applyTextureEvent, _applyTextureCommandBuffer);

                _captureTextureCommandBuffer.Clear();
                _applyTextureCommandBuffer.Clear();

                RenderTexture.ReleaseTemporary(tempRenderTexture);
            }
        }

        // Extract the image which is in front of our clip plane
        // The compositing is heavily relying on the alpha channel, therefore we want to make sure it does
        // not get corrupted by the postprocessing or any shader
        private void RenderForeground()
        {
            var debugClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.DEBUG_CLIP_PLANE);
            var renderComplexClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.COMPLEX_CLIP_PLANE);
            var renderGroundClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.GROUND_CLIP_PLANE);
            var overridePostProcessing =
                SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.OVERRIDE_POST_PROCESSING);
            var fixPostEffectsAlpha = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.FIX_FOREGROUND_ALPHA) |
                                      liv.fixPostEffectsAlpha;

            MonoBehaviour[] behaviours = null;
            bool[] wasBehaviourEnabled = null;
            if (disableStandardAssets)
                SDKUtils.DisableStandardAssets(cameraInstance, ref behaviours, ref wasBehaviourEnabled);

            // Capture camera defaults
            var capturedClearFlags = cameraInstance.clearFlags;
            var capturedBgColor = cameraInstance.backgroundColor;
            var capturedFogColor = RenderSettings.fogColor;

            // Make sure that fog does not corrupt alpha channel
            RenderSettings.fogColor = new Color(capturedFogColor.r, capturedFogColor.g, capturedFogColor.b, 0f);
            SDKUtils.SetCamera(cameraInstance, cameraInstance.transform, _inputFrame, localToWorldMatrix,
                spectatorLayerMask);
            cameraInstance.clearFlags = CameraClearFlags.Color;
            cameraInstance.backgroundColor = Color.clear;
            cameraInstance.targetTexture = _foregroundRenderTexture;

            var capturedAlphaRenderTexture = RenderTexture.GetTemporary(_foregroundRenderTexture.width,
                _foregroundRenderTexture.height, 0, _foregroundRenderTexture.format);
#if UNITY_EDITOR
            capturedAlphaRenderTexture.name = "LIV.CapturedAlphaRenderTexture";
#endif

            // Render opaque pixels into alpha channel
            _clipPlaneCommandBuffer.DrawMesh(_clipPlaneMesh, Matrix4x4.identity, _writeOpaqueToAlphaMaterial, 0, 0);

            // Render clip plane
            var clipPlaneTransform = localToWorldMatrix * (Matrix4x4) _inputFrame.clipPlane.transform;
            _clipPlaneCommandBuffer.DrawMesh(_clipPlaneMesh, clipPlaneTransform,
                GetClipPlaneMaterial(debugClipPlane, renderComplexClipPlane, ColorWriteMask.All), 0, 0);

            // Render ground clip plane
            if (renderGroundClipPlane)
            {
                var groundClipPlaneTransform = localToWorldMatrix * (Matrix4x4) _inputFrame.groundClipPlane.transform;
                _clipPlaneCommandBuffer.DrawMesh(_clipPlaneMesh, groundClipPlaneTransform,
                    GetGroundClipPlaneMaterial(debugClipPlane, ColorWriteMask.All), 0, 0);
            }

            // Copy alpha in to texture
            _clipPlaneCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, capturedAlphaRenderTexture);
            cameraInstance.AddCommandBuffer(_clipPlaneCameraEvent, _clipPlaneCommandBuffer);

            // Fix alpha corruption by post processing
            RenderTexture tempRenderTexture = null;
            if (overridePostProcessing || fixPostEffectsAlpha)
            {
                tempRenderTexture = RenderTexture.GetTemporary(_foregroundRenderTexture.width,
                    _foregroundRenderTexture.height, 0, _foregroundRenderTexture.format);
#if UNITY_EDITOR
                tempRenderTexture.name = "LIV.TemporaryRenderTexture";
#endif
                _captureTextureCommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tempRenderTexture);
                cameraInstance.AddCommandBuffer(_captureTextureEvent, _captureTextureCommandBuffer);

                _writeMaterial.SetInt(SDKShaders.LIV_COLOR_MASK,
                    overridePostProcessing ? (int) ColorWriteMask.All : (int) ColorWriteMask.Alpha);
                _applyTextureCommandBuffer.Blit(tempRenderTexture, BuiltinRenderTextureType.CurrentActive,
                    _writeMaterial);
                cameraInstance.AddCommandBuffer(_applyTextureEvent, _applyTextureCommandBuffer);
            }

            // Combine captured alpha with result alpha
            _combineAlphaMaterial.SetInt(SDKShaders.LIV_COLOR_MASK, (int) ColorWriteMask.Alpha);
            _combineAlphaCommandBuffer.Blit(capturedAlphaRenderTexture, BuiltinRenderTextureType.CurrentActive,
                _combineAlphaMaterial);
            cameraInstance.AddCommandBuffer(_clipPlaneCombineAlphaCameraEvent, _combineAlphaCommandBuffer);

            if (useDeferredRendering)
                SDKUtils.ForceForwardRendering(cameraInstance, _clipPlaneMesh, _forceForwardRenderingMaterial);

            SDKShaders.StartRendering();
            SDKShaders.StartForegroundRendering();
            InvokePreRenderForeground();
            SendTextureToBridge(_foregroundRenderTexture, TEXTURE_ID.FOREGROUND_COLOR_BUFFER_ID);
            cameraInstance.Render();
            InvokePostRenderForeground();
            cameraInstance.targetTexture = null;
            SDKShaders.StopForegroundRendering();
            SDKShaders.StopRendering();

            if (overridePostProcessing || fixPostEffectsAlpha)
            {
                cameraInstance.RemoveCommandBuffer(_captureTextureEvent, _captureTextureCommandBuffer);
                cameraInstance.RemoveCommandBuffer(_applyTextureEvent, _applyTextureCommandBuffer);

                _captureTextureCommandBuffer.Clear();
                _applyTextureCommandBuffer.Clear();

                RenderTexture.ReleaseTemporary(tempRenderTexture);
            }

            cameraInstance.RemoveCommandBuffer(_clipPlaneCameraEvent, _clipPlaneCommandBuffer);
            cameraInstance.RemoveCommandBuffer(_clipPlaneCombineAlphaCameraEvent, _combineAlphaCommandBuffer);

            RenderTexture.ReleaseTemporary(capturedAlphaRenderTexture);

            _clipPlaneCommandBuffer.Clear();
            _combineAlphaCommandBuffer.Clear();

            // Revert camera defaults
            cameraInstance.clearFlags = capturedClearFlags;
            cameraInstance.backgroundColor = capturedBgColor;
            RenderSettings.fogColor = capturedFogColor;

            SDKUtils.RestoreStandardAssets(ref behaviours, ref wasBehaviourEnabled);
        }

        // Renders a single camera in a single texture with occlusion only from opaque objects.
        // This is the most performant option for mixed reality.
        // It does not support any transparency in the foreground layer.
        private void RenderOptimized()
        {
            var debugClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.DEBUG_CLIP_PLANE);
            var renderComplexClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.COMPLEX_CLIP_PLANE);
            var renderGroundClipPlane = SDKUtils.FeatureEnabled(inputFrame.features, FEATURES.GROUND_CLIP_PLANE);

            SDKUtils.SetCamera(cameraInstance, cameraInstance.transform, _inputFrame, localToWorldMatrix,
                spectatorLayerMask);
            cameraInstance.targetTexture = _optimizedRenderTexture;

            // Clear alpha channel
            _writeMaterial.SetInt(SDKShaders.LIV_COLOR_MASK, (int) ColorWriteMask.Alpha);
            _optimizedRenderingCommandBuffer.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CurrentActive,
                _writeMaterial);

            // Render opaque pixels into alpha channel            
            _writeOpaqueToAlphaMaterial.SetInt(SDKShaders.LIV_COLOR_MASK, (int) ColorWriteMask.Alpha);
            _optimizedRenderingCommandBuffer.DrawMesh(_clipPlaneMesh, Matrix4x4.identity, _writeOpaqueToAlphaMaterial,
                0, 0);

            // Render clip plane            
            var clipPlaneTransform = localToWorldMatrix * (Matrix4x4) _inputFrame.clipPlane.transform;
            _optimizedRenderingCommandBuffer.DrawMesh(_clipPlaneMesh, clipPlaneTransform,
                GetClipPlaneMaterial(debugClipPlane, renderComplexClipPlane, ColorWriteMask.Alpha), 0, 0);

            // Render ground clip plane            
            if (renderGroundClipPlane)
            {
                var groundClipPlaneTransform = localToWorldMatrix * (Matrix4x4) _inputFrame.groundClipPlane.transform;
                _optimizedRenderingCommandBuffer.DrawMesh(_clipPlaneMesh, groundClipPlaneTransform,
                    GetGroundClipPlaneMaterial(debugClipPlane, ColorWriteMask.Alpha), 0, 0);
            }

            cameraInstance.AddCommandBuffer(CameraEvent.AfterEverything, _optimizedRenderingCommandBuffer);

            // TODO: this is just proprietary
            SDKShaders.StartRendering();
            SDKShaders.StartBackgroundRendering();
            InvokePreRenderBackground();
            SendTextureToBridge(_optimizedRenderTexture, TEXTURE_ID.OPTIMIZED_COLOR_BUFFER_ID);
            cameraInstance.Render();
            InvokePostRenderBackground();
            cameraInstance.targetTexture = null;
            SDKShaders.StopBackgroundRendering();
            SDKShaders.StopRendering();

            cameraInstance.RemoveCommandBuffer(CameraEvent.AfterEverything, _optimizedRenderingCommandBuffer);
            _optimizedRenderingCommandBuffer.Clear();
        }

        private void CreateAssets()
        {
            var cameraReferenceEnabled = cameraReference.enabled;
            if (cameraReferenceEnabled) cameraReference.enabled = false;
            var cameraReferenceActive = cameraReference.gameObject.activeSelf;
            if (cameraReferenceActive) cameraReference.gameObject.SetActive(false);

            var cloneGO = Object.Instantiate(cameraReference.gameObject, liv.stage);
            cameraInstance = cloneGO.GetComponent<Camera>();

            SDKUtils.CleanCameraBehaviours(cameraInstance, liv.excludeBehaviours);

            if (cameraReferenceActive != cameraReference.gameObject.activeSelf)
                cameraReference.gameObject.SetActive(cameraReferenceActive);
            if (cameraReferenceEnabled != cameraReference.enabled) cameraReference.enabled = cameraReferenceEnabled;

            cameraInstance.name = "LIV Camera";
            if (cameraInstance.tag == "MainCamera") cameraInstance.tag = "Untagged";

            cameraInstance.transform.localScale = Vector3.one;
            cameraInstance.rect = new Rect(0, 0, 1, 1);
            cameraInstance.depth = 0;
#if UNITY_5_4_OR_NEWER
            _cameraInstance.stereoTargetEye = StereoTargetEyeMask.None;
#endif
#if UNITY_5_6_OR_NEWER
            _cameraInstance.allowMSAA = false;
#endif
            cameraInstance.enabled = false;
            cameraInstance.gameObject.SetActive(true);

            _clipPlaneMesh = new Mesh();
            SDKUtils.CreateClipPlane(_clipPlaneMesh, 10, 10, true, 1000f);
            _clipPlaneSimpleMaterial = new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_CLIP_PLANE_SIMPLE_SHADER]);
            _clipPlaneSimpleDebugMaterial =
                new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_CLIP_PLANE_SIMPLE_DEBUG_SHADER]);
            _clipPlaneComplexMaterial =
                new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_CLIP_PLANE_COMPLEX_SHADER]);
            _clipPlaneComplexDebugMaterial =
                new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_CLIP_PLANE_COMPLEX_DEBUG_SHADER]);
            _writeOpaqueToAlphaMaterial =
                new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_WRITE_OPAQUE_TO_ALPHA_SHADER]);
            _combineAlphaMaterial = new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_COMBINE_ALPHA_SHADER]);
            _writeMaterial = new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_WRITE_SHADER]);
            _forceForwardRenderingMaterial =
                new Material(VrAssetLoader.LivShaders[SDKShaders.LIV_FORCE_FORWARD_RENDERING_SHADER]);
            _clipPlaneCommandBuffer = new CommandBuffer();
            _combineAlphaCommandBuffer = new CommandBuffer();
            _captureTextureCommandBuffer = new CommandBuffer();
            _applyTextureCommandBuffer = new CommandBuffer();
            _optimizedRenderingCommandBuffer = new CommandBuffer();

#if UNITY_EDITOR
            _clipPlaneMesh.name = "LIV.clipPlane";
            _clipPlaneSimpleMaterial.name = "LIV.clipPlaneSimple";
            _clipPlaneSimpleDebugMaterial.name = "LIV.clipPlaneSimpleDebug";
            _clipPlaneComplexMaterial.name = "LIV.clipPlaneComplex";
            _clipPlaneComplexDebugMaterial.name = "LIV.clipPlaneComplexDebug";
            _writeOpaqueToAlphaMaterial.name = "LIV.writeOpaqueToAlpha";
            _combineAlphaMaterial.name = "LIV.combineAlpha";
            _writeMaterial.name = "LIV.write";
            _forceForwardRenderingMaterial.name = "LIV.forceForwardRendering";
            _clipPlaneCommandBuffer.name = "LIV.renderClipPlanes";
            _combineAlphaCommandBuffer.name = "LIV.foregroundCombineAlpha";
            _captureTextureCommandBuffer.name = "LIV.captureTexture";
            _applyTextureCommandBuffer.name = "LIV.applyTexture";
            _optimizedRenderingCommandBuffer.name = "LIV.optimizedRendering";
#endif
        }

        private void DestroyAssets()
        {
            if (cameraInstance != null)
            {
                Object.Destroy(cameraInstance.gameObject);
                cameraInstance = null;
            }

            SDKUtils.DestroyObject(ref _clipPlaneMesh);
            SDKUtils.DestroyObject(ref _clipPlaneSimpleMaterial);
            SDKUtils.DestroyObject(ref _clipPlaneSimpleDebugMaterial);
            SDKUtils.DestroyObject(ref _clipPlaneComplexMaterial);
            SDKUtils.DestroyObject(ref _clipPlaneComplexDebugMaterial);
            SDKUtils.DestroyObject(ref _writeOpaqueToAlphaMaterial);
            SDKUtils.DestroyObject(ref _combineAlphaMaterial);
            SDKUtils.DestroyObject(ref _writeMaterial);
            SDKUtils.DestroyObject(ref _forceForwardRenderingMaterial);

            SDKUtils.DisposeObject(ref _clipPlaneCommandBuffer);
            SDKUtils.DisposeObject(ref _combineAlphaCommandBuffer);
            SDKUtils.DisposeObject(ref _captureTextureCommandBuffer);
            SDKUtils.DisposeObject(ref _applyTextureCommandBuffer);
            SDKUtils.DisposeObject(ref _optimizedRenderingCommandBuffer);
        }
    }
}
#endif