using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR.InteractionSystem;

namespace TwoForksVr.Locomotion
{
    public class TeleportArc : MonoBehaviour
    {
        private const float arcVelocity = 10f;

        public int segmentCount = 60;
        public float thickness = 0.01f;

        [Tooltip("The amount of time in seconds to predict the motion of the projectile.")]
        public float arcDuration = 3.0f;

        private readonly LayerMask traceLayerMask = LayerHelper.GetMask(GameLayer.Default, GameLayer.Terrain);
        private readonly bool arcInvalid = false;
        private Transform arcObjectsTransfrom;
        private float arcTimeOffset;


        //Private data
        private LineRenderer[] lineRenderers;

        private Material material;
        private int prevSegmentCount;
        private float prevThickness;
        private Vector3 projectileVelocity;
        private readonly float scale = 1;
        private bool showArc = true;
        private Vector3 startPos;
        private bool useGravity = true;

        public static TeleportArc Create(TeleportController teleportController, Material material)
        {
            var instance = new GameObject("VrTeleportArc").AddComponent<TeleportArc>();
            instance.transform.SetParent(teleportController.transform.parent);
            instance.material = material;

            return instance;
        }

        private void Start()
        {
            arcTimeOffset = Time.time;
            Show();
        }

        private void Update()
        {
            if (thickness != prevThickness || segmentCount != prevSegmentCount)
            {
                CreateLineRendererObjects();
                prevThickness = thickness;
                prevSegmentCount = segmentCount;
            }
        }

        private void CreateLineRendererObjects()
        {
            //Destroy any existing line renderer objects
            if (arcObjectsTransfrom != null) Destroy(arcObjectsTransfrom.gameObject);

            var arcObjectsParent = new GameObject("ArcObjects");
            arcObjectsTransfrom = arcObjectsParent.transform;
            arcObjectsTransfrom.SetParent(transform);

            //Create new line renderer objects
            lineRenderers = new LineRenderer[segmentCount];
            for (var i = 0; i < segmentCount; ++i)
            {
                var newObject = new GameObject("LineRenderer_" + i);
                newObject.transform.SetParent(arcObjectsTransfrom);

                lineRenderers[i] = newObject.AddComponent<LineRenderer>();

                lineRenderers[i].receiveShadows = false;
                lineRenderers[i].reflectionProbeUsage = ReflectionProbeUsage.Off;
                lineRenderers[i].lightProbeUsage = LightProbeUsage.Off;
                lineRenderers[i].shadowCastingMode = ShadowCastingMode.Off;
                lineRenderers[i].material = material;
                lineRenderers[i].startWidth = thickness * scale;
                lineRenderers[i].endWidth = thickness * scale;
                lineRenderers[i].enabled = false;
            }
        }


        public void Show()
        {
            showArc = true;
            if (lineRenderers == null) CreateLineRendererObjects();
        }


        public void Hide()
        {
            //Hide the line segments if they were previously being shown
            if (showArc) HideLineSegments(0, segmentCount);
            showArc = false;
        }

        // Draws each segment of the arc individually
        public bool DrawArc(out RaycastHit hitInfo)
        {
            startPos = transform.position;
            projectileVelocity = transform.forward * arcVelocity;
            useGravity = true;

            var timeStep = arcDuration / segmentCount;

            var currentTimeOffset = 0f;

            var segmentStartTime = currentTimeOffset;

            var arcHitTime = FindProjectileCollision(out hitInfo);

            if (arcInvalid)
            {
                //Only draw first segment
                lineRenderers[0].enabled = true;
                lineRenderers[0].SetPosition(0, GetArcPositionAtTime(0.0f));
                lineRenderers[0].SetPosition(1, GetArcPositionAtTime(arcHitTime < timeStep ? arcHitTime : timeStep));

                HideLineSegments(1, segmentCount);
            }
            else
            {
                //Draw the first segment outside the loop if needed
                var loopStartSegment = 0;

                var stopArc = false;
                var currentSegment = 0;
                if (segmentStartTime < arcHitTime)
                    for (currentSegment = loopStartSegment; currentSegment < segmentCount; ++currentSegment)
                    {
                        //Clamp the segment end time to the arc duration
                        var segmentEndTime = segmentStartTime + timeStep;
                        if (segmentEndTime >= arcDuration)
                        {
                            segmentEndTime = arcDuration;
                            stopArc = true;
                        }

                        if (segmentEndTime >= arcHitTime)
                        {
                            segmentEndTime = arcHitTime;
                            stopArc = true;
                        }

                        DrawArcSegment(currentSegment, segmentStartTime, segmentEndTime);

                        segmentStartTime += timeStep;

                        //If the previous end time or the next start time is beyond the duration then stop the arc
                        if (stopArc || segmentStartTime >= arcDuration || segmentStartTime >= arcHitTime) break;
                    }
                else
                    currentSegment--;

                //Hide the rest of the line segments
                HideLineSegments(currentSegment + 1, segmentCount);
            }

            return arcHitTime != float.MaxValue;
        }

        private void DrawArcSegment(int index, float startTime, float endTime)
        {
            lineRenderers[index].enabled = true;
            lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
            lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        }

        private float FindProjectileCollision(out RaycastHit hitInfo)
        {
            var timeStep = arcDuration / segmentCount;
            var segmentStartTime = 0.0f;

            hitInfo = new RaycastHit();

            var segmentStartPos = GetArcPositionAtTime(segmentStartTime);
            for (var i = 0; i < segmentCount; ++i)
            {
                var segmentEndTime = segmentStartTime + timeStep;
                var segmentEndPos = GetArcPositionAtTime(segmentEndTime);

                if (Physics.Linecast(segmentStartPos, segmentEndPos, out hitInfo, traceLayerMask))
                    if (hitInfo.collider.GetComponent<IgnoreTeleportTrace>() == null)
                    {
                        Util.DrawCross(hitInfo.point, Color.red, 0.5f);
                        var segmentDistance = Vector3.Distance(segmentStartPos, segmentEndPos);
                        var hitTime = segmentStartTime + timeStep * (hitInfo.distance / segmentDistance);
                        return hitTime;
                    }

                segmentStartTime = segmentEndTime;
                segmentStartPos = segmentEndPos;
            }

            return float.MaxValue;
        }


        private Vector3 GetArcPositionAtTime(float time)
        {
            var gravity = useGravity ? Physics.gravity : Vector3.zero;

            var arcPos = startPos + (projectileVelocity * time + 0.5f * time * time * gravity) * scale;
            return arcPos;
        }


        private void HideLineSegments(int startSegment, int endSegment)
        {
            if (lineRenderers == null) return;
            for (var i = startSegment; i < endSegment; ++i) lineRenderers[i].enabled = false;
        }
    }
}