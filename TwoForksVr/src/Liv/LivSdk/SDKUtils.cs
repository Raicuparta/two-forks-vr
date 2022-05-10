using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace LIV.SDK.Unity;

public static class SDKUtils
{
    public static TEXTURE_COLOR_SPACE GetDefaultColorSpace
    {
        get
        {
            switch (QualitySettings.activeColorSpace)
            {
                case ColorSpace.Gamma:
                    return TEXTURE_COLOR_SPACE.SRGB;
                case ColorSpace.Linear:
                    return TEXTURE_COLOR_SPACE.LINEAR;
            }

            return TEXTURE_COLOR_SPACE.UNDEFINED;
        }
    }

    public static void CreateClipPlane(Mesh mesh, int resX, int resY, bool useQuads, float skirtLength)
    {
        var vertexCount = (resX + 1) * (resY + 1);
        var triangleCount = useQuads ? resX * resY * 4 : resX * resY * 2 * 3;
        var v = new Vector3[vertexCount];
        var uv = new Vector2[vertexCount];
        var t = new int[triangleCount];

        var hWidth = 0.5f;
        var hHeight = 0.5f;

        var resX1 = resX + 1;
        var resY1 = resY + 1;

        for (var y = 0; y < resY1; y++)
        for (var x = 0; x < resX1; x++)
        {
            var vi = y * resX1 + x;
            var uvx = x / (float) resX;
            var uvy = y / (float) resY;
            var skirtX = x == 0 || x == resX ? skirtLength : 1f;
            var skirtY = y == 0 || y == resY ? skirtLength : 1f;
            v[vi] = new Vector2((-hWidth + uvx) * skirtX, (-hHeight + uvy) * skirtY);
            uv[vi] = new Vector2(Mathf.InverseLerp(1, resX - 1, x), Mathf.InverseLerp(1, resY - 1, y));
        }

        mesh.Clear();
        mesh.vertices = v;
        mesh.uv = uv;
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * float.MaxValue);

        {
            var faces = resX * resY;
            var vi = 0;
            var ti = 0;

            if (useQuads)
            {
                for (var i = 0; i < faces; i++)
                {
                    vi = i / resX * resX1 + i % resX;
                    t[ti++] = vi + 1;
                    t[ti++] = vi;
                    t[ti++] = vi + 1 + resX;
                    t[ti++] = vi + 2 + resX;
                }

                mesh.SetIndices(t, MeshTopology.Quads, 0);
            }
            else
            {
                for (var i = 0; i < faces; i++)
                {
                    vi = i / resX * resX1 + i % resX;

                    t[ti++] = vi + 2 + resX;
                    t[ti++] = vi + 1;
                    t[ti++] = vi;

                    t[ti++] = vi + 1 + resX;
                    t[ti++] = vi + 2 + resX;
                    t[ti++] = vi;
                }

                mesh.SetIndices(t, MeshTopology.Triangles, 0);
            }
        }
    }

    public static RenderTextureReadWrite GetReadWriteFromColorSpace(TEXTURE_COLOR_SPACE colorSpace)
    {
        switch (colorSpace)
        {
            case TEXTURE_COLOR_SPACE.LINEAR:
                return RenderTextureReadWrite.Linear;
            case TEXTURE_COLOR_SPACE.SRGB:
                return RenderTextureReadWrite.sRGB;
            default:
                return RenderTextureReadWrite.Default;
        }
    }

    public static TEXTURE_COLOR_SPACE GetColorSpace(RenderTexture renderTexture)
    {
        if (renderTexture == null) return TEXTURE_COLOR_SPACE.UNDEFINED;
        if (renderTexture.sRGB) return TEXTURE_COLOR_SPACE.SRGB;
        return TEXTURE_COLOR_SPACE.LINEAR;
    }

    public static RENDERING_PIPELINE GetRenderingPipeline(RenderingPath renderingPath)
    {
        switch (renderingPath)
        {
            case RenderingPath.DeferredLighting:
                return RENDERING_PIPELINE.DEFERRED;
            case RenderingPath.DeferredShading:
                return RENDERING_PIPELINE.DEFERRED;
            case RenderingPath.Forward:
                return RENDERING_PIPELINE.FORWARD;
            case RenderingPath.VertexLit:
                return RENDERING_PIPELINE.VERTEX_LIT;
            default:
                return RENDERING_PIPELINE.UNDEFINED;
        }
    }

    public static TEXTURE_DEVICE GetDevice()
    {
        switch (SystemInfo.graphicsDeviceType)
        {
            case GraphicsDeviceType.Direct3D11:
            case GraphicsDeviceType.Direct3D12:
                return TEXTURE_DEVICE.DIRECTX;
            case GraphicsDeviceType.Vulkan:
                return TEXTURE_DEVICE.VULKAN;
            case GraphicsDeviceType.Metal:
                return TEXTURE_DEVICE.METAL;
            case GraphicsDeviceType.OpenGLCore:
                return TEXTURE_DEVICE.OPENGL;
            default:
                return TEXTURE_DEVICE.UNDEFINED;
        }
    }

    public static bool ContainsFlag(ulong flags, ulong flag)
    {
        return (flags & flag) != 0;
    }

    public static ulong SetFlag(ulong flags, ulong flag, bool enabled)
    {
        if (enabled)
            return flags | flag;
        return flags & ~flag;
    }

    public static void GetCameraPositionAndRotation(SDKPose pose, Matrix4x4 originLocalToWorldMatrix,
        out Vector3 position, out Quaternion rotation)
    {
        position = originLocalToWorldMatrix.MultiplyPoint(pose.localPosition);
        rotation = RotateQuaternionByMatrix(originLocalToWorldMatrix, pose.localRotation);
    }

    public static void CleanCameraBehaviours(Camera camera, string[] excludeBehaviours)
    {
        // Remove all children from camera clone.
        foreach (Transform child in camera.transform) Object.Destroy(child.gameObject);

        if (excludeBehaviours == null) return;
        for (var i = 0; i < excludeBehaviours.Length; i++) Object.Destroy(camera.GetComponent(excludeBehaviours[i]));
    }

    public static void SetCamera(Camera camera, Transform cameraTransform, SDKInputFrame inputFrame,
        Matrix4x4 originLocalToWorldMatrix, int layerMask)
    {
        var worldPosition = Vector3.zero;
        var worldRotation = Quaternion.identity;
        var verticalFieldOfView = inputFrame.pose.verticalFieldOfView;
        var nearClipPlane = inputFrame.pose.nearClipPlane;
        var farClipPlane = inputFrame.pose.farClipPlane;
        Matrix4x4 projectionMatrix = inputFrame.pose.projectionMatrix;

        GetCameraPositionAndRotation(inputFrame.pose, originLocalToWorldMatrix, out worldPosition, out worldRotation);

        cameraTransform.position = worldPosition;
        cameraTransform.rotation = worldRotation;
        camera.fieldOfView = verticalFieldOfView;
        camera.nearClipPlane = nearClipPlane;
        camera.farClipPlane = farClipPlane;
        camera.projectionMatrix = projectionMatrix;
        camera.cullingMask = layerMask;
    }

    public static Quaternion RotateQuaternionByMatrix(Matrix4x4 matrix, Quaternion rotation)
    {
        return Quaternion.LookRotation(
            matrix.MultiplyVector(Vector3.forward),
            matrix.MultiplyVector(Vector3.up)
        ) * rotation;
    }

    public static SDKTrackedSpace GetTrackedSpace(Transform transform)
    {
        if (transform == null) return SDKTrackedSpace.empty;
        return new SDKTrackedSpace
        {
            trackedSpaceWorldPosition = transform.position,
            trackedSpaceWorldRotation = transform.rotation,
            trackedSpaceLocalScale = transform.localScale,
            trackedSpaceLocalToWorldMatrix = transform.localToWorldMatrix,
            trackedSpaceWorldToLocalMatrix = transform.worldToLocalMatrix
        };
    }

    public static bool DestroyObject<T>(ref T reference) where T : Object
    {
        if (reference == null) return false;
        Object.Destroy(reference);
        reference = default;
        return true;
    }

    public static bool DisposeObject<T>(ref T reference) where T : IDisposable
    {
        if (reference == null) return false;
        reference.Dispose();
        reference = default;
        return true;
    }

    public static bool CreateTexture(ref RenderTexture renderTexture, int width, int height, int depth,
        RenderTextureFormat format)
    {
        DestroyTexture(ref renderTexture);
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("LIV: Unable to create render texture. Texture dimension must be higher than zero.");
            return false;
        }

        renderTexture = new RenderTexture(width, height, depth, format)
        {
            antiAliasing = 1,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false,
            anisoLevel = 0
        };

        if (!renderTexture.Create())
        {
            Debug.LogError("LIV: Unable to create render texture.");
            return false;
        }

        return true;
    }

    public static void DestroyTexture(ref RenderTexture _renderTexture)
    {
        if (_renderTexture == null) return;
        if (_renderTexture.IsCreated()) _renderTexture.Release();
        _renderTexture = null;
    }

    public static void ApplyUserSpaceTransform(SDKRender render)
    {
        if (render.stageTransform == null) return;
        render.stageTransform.localPosition = render.inputFrame.stageTransform.localPosition;
        render.stageTransform.localRotation = render.inputFrame.stageTransform.localRotation;
        render.stageTransform.localScale = render.inputFrame.stageTransform.localScale;
    }

    public static void CreateBridgeOutputFrame(SDKRender render)
    {
        var renderingPipeline = RENDERING_PIPELINE.UNDEFINED;
#if LIV_UNIVERSAL_RENDER
            renderingPipeline = RENDERING_PIPELINE.UNIVERSAL;
#else
        if (render.cameraInstance != null)
            renderingPipeline = GetRenderingPipeline(render.cameraInstance.actualRenderingPath);
#endif
        SDKBridge.CreateFrame(new SDKOutputFrame
        {
            renderingPipeline = renderingPipeline,
            trackedSpace = GetTrackedSpace(render.stageTransform == null ? render.stage : render.stageTransform)
        });
    }

    public static bool FeatureEnabled(FEATURES features, FEATURES feature)
    {
        return ContainsFlag((ulong) features, (ulong) feature);
    }

    // Disable standard assets if required.
    public static void DisableStandardAssets(Camera cameraInstance, ref MonoBehaviour[] behaviours,
        ref bool[] wasBehaviourEnabled)
    {
        behaviours = null;
        wasBehaviourEnabled = null;
        behaviours = cameraInstance.gameObject.GetComponents<MonoBehaviour>();
        wasBehaviourEnabled = new bool[behaviours.Length];
        for (var i = 0; i < behaviours.Length; i++)
        {
            var behaviour = behaviours[i];
            // generates garbage
            if (behaviour.enabled && behaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
            {
                behaviour.enabled = false;
                wasBehaviourEnabled[i] = true;
            }
        }
    }

    // Restore disabled behaviours.
    public static void RestoreStandardAssets(ref MonoBehaviour[] behaviours, ref bool[] wasBehaviourEnabled)
    {
        if (behaviours != null)
            for (var i = 0; i < behaviours.Length; i++)
                if (wasBehaviourEnabled[i])
                    behaviours[i].enabled = true;
    }

    public static void ForceForwardRendering(Camera cameraInstance, Mesh clipPlaneMesh,
        Material forceForwardRenderingMaterial)
    {
        var forceForwardRenderingMatrix = cameraInstance.transform.localToWorldMatrix *
                                          Matrix4x4.TRS(Vector3.forward * (cameraInstance.nearClipPlane + 0.1f),
                                              Quaternion.identity, Vector3.one);
        Graphics.DrawMesh(clipPlaneMesh, forceForwardRenderingMatrix, forceForwardRenderingMaterial, 0, cameraInstance,
            0, new MaterialPropertyBlock(), false, false, false);
    }
}