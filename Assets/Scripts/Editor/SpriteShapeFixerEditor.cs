using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace BLE.SevenSins
{
    [CustomEditor(typeof(SpriteShapeFixer))]
    public class SpriteShapeFixerEditor : Editor
    {
        #region Main
        public void OnEnable()
        {
            Fixer = (SpriteShapeFixer)target;
            Controller = Fixer.gameObject.GetComponent<SpriteShapeController>();
            FixColliderPoints = new List<Vector2>();
            Spline = Controller.spline;
        }

        private SpriteShapeFixer Fixer { get; set; }
        private SpriteShapeController Controller { get; set; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            switch (Fixer.mode)
            {
                case SpriteShapeFixer.Mode.FixUnityCollider: DoFixCollider(); break;
                case SpriteShapeFixer.Mode.RectangularCollider: DoRectangularCollider(); break;
            }
        }

        private void DoColliderSection(string buttonLabel, string[] propertyNames, Func<Vector2[]> colliderPointsGetter)
        {
            bool pressedButton = GUILayout.Button(buttonLabel);
            foreach (string name in propertyNames)
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name));
            serializedObject.ApplyModifiedProperties();

            if (false == pressedButton)
                return;

            if (false == Controller.hasCollider)
            {
                Note.Log("SpriteShape doesn't have a collider!", Color.red);
                return;
            }

            Undo.RecordObject(Controller, buttonLabel);
            Controller.autoUpdateCollider = false;

            Vector2[] newPoints = colliderPointsGetter();
            if (Controller.edgeCollider != null)
            {
                Undo.RecordObject(Controller.edgeCollider, buttonLabel);
                Controller.edgeCollider.points = newPoints;
            }
            else
            {
                Undo.RecordObject(Controller.polygonCollider, buttonLabel);
                Controller.polygonCollider.points = newPoints;
            }

            Controller.RefreshSpriteShape();
            EditorUtility.SetDirty(Controller);
        }

        private List<Vector2> FixColliderPoints { get; set; }
        private Spline Spline { get; set; }
        #endregion



        #region Collider Fixing
        private enum PointKind
        {
            Default,
            SlopeIn,
            Slope,
            SlopeOut
        }

        private struct Flags
        {
            public bool originalIsOffset;
            public bool pointIsAddedBefore;
            public bool pointIsAddedAfter;

            public void Log(int pointIndex)
            {
                Note.Log($"{pointIndex}\nIsOffset {originalIsOffset}\n Before {pointIsAddedBefore}\n After {pointIsAddedAfter}");
            }
        }

        private const string FIX_COLLIDER_LABEL = "Fix Collider";
        private readonly string[] FIX_COLLIDER_PROPERTY_NAMES = new string[2] { "slopesAngle", "slopesOffset" };

        /// <summary>
        /// Unity's own SpriteShapeController.UpdateCollider creates weird Colliders,
        /// that cut corners or offset the collider in a strange way. To compensate,
        /// this syncs the SpriteShapes collider with the spriteshape and allowes
        /// for additional collider offsetting at slopes.
        /// </summary>
        private void DoFixCollider()
        {
            DoColliderSection(FIX_COLLIDER_LABEL,
                FIX_COLLIDER_PROPERTY_NAMES,
                GetFixColliderPoints);
        }

        private Vector2[] GetFixColliderPoints()
        {
            FixColliderPoints.Clear();
            FixColliderPointKind = PointKind.Default;
            LastFixColliderAngleDirection = 0f;

            for (int i = 0; i < Spline.GetPointCount(); i++)
            {
                Vector2 shapePoint = Spline.GetPosition(i);
                if (i == Spline.GetPointCount() - 1 || Fixer.slopesOffset == 0f)
                {
                    FixColliderPointKind = PointKind.Default;
                    FixColliderPoints.Add(shapePoint);
                    continue;
                }

                Flags flags = CalculateFlags((Vector2)Spline.GetPosition(i + 1) - shapePoint);
                AddColliderPoint(shapePoint, flags);
            }
            FixColliderPoints.Add(Spline.GetPosition(0)); // To close the shape

            Vector2[] newPoints = new Vector2[FixColliderPoints.Count];
            for (int i = 0; i < FixColliderPoints.Count; i++)
                newPoints[i] = FixColliderPoints[i];
            return newPoints;
        }

        /// <summary>
        /// Sets flags depending on an angle and what kind of point the current is.
        /// </summary>
        private Flags CalculateFlags(Vector2 currentToNextVector)
        {
            float angle = Vector2.SignedAngle(currentToNextVector, Vector2.right);

            Flags flags = new Flags();
            if (Mathf.Abs(angle).IsAround(Fixer.slopesAngle, 0.05f))
            {
                LastFixColliderAngleDirection = Mathf.Sign(angle);
                switch (FixColliderPointKind)
                {
                    case PointKind.Default:
                    case PointKind.SlopeOut:
                        flags.pointIsAddedAfter = LastFixColliderAngleDirection < 0f;
                        flags.originalIsOffset = LastFixColliderAngleDirection > 0f;

                        FixColliderPointKind = PointKind.SlopeIn;
                        break;

                    case PointKind.SlopeIn:
                        flags.originalIsOffset = true;
                        FixColliderPointKind = PointKind.Slope;
                        break;

                    case PointKind.Slope:
                        flags.originalIsOffset = true;
                        break;
                }
            }
            else if (FixColliderPointKind != PointKind.Default && FixColliderPointKind != PointKind.SlopeOut)
            {
                flags.pointIsAddedBefore = LastFixColliderAngleDirection >= 0f;
                flags.originalIsOffset = LastFixColliderAngleDirection < 0f;
                FixColliderPointKind = PointKind.SlopeOut;
            }
            else
            {
                FixColliderPointKind = PointKind.Default;
            }

            return flags;
        }

        private void AddColliderPoint(Vector2 originalPoint, Flags flags)
        {
            if (flags.originalIsOffset)
            {
                FixColliderPoints.Add(originalPoint + Vector2.left * LastFixColliderAngleDirection * Fixer.slopesOffset);
            }
            else if (flags.pointIsAddedAfter)
            {
                FixColliderPoints.Add(originalPoint);
                FixColliderPoints.Add(originalPoint + Vector2.left * LastFixColliderAngleDirection * Fixer.slopesOffset);
            }
            else if (flags.pointIsAddedBefore)
            {
                FixColliderPoints.Add(originalPoint + Vector2.left * LastFixColliderAngleDirection * Fixer.slopesOffset);
                FixColliderPoints.Add(originalPoint);
            }
            else
            {
                FixColliderPoints.Add(originalPoint);
            }
        }

        private PointKind FixColliderPointKind { get; set; }
        private float LastFixColliderAngleDirection { get; set; }
        #endregion



        #region Auto Box Type Collider
        private const string RECTANGULAR_COLLIDER_LABEL = "Update Collider";
        private static readonly string[] RECTANGULAR_PROPERTY_NAMES = new string[3] { "height", "borderOffset", "trapezoidOffset" };
        private const float ANGLE_90 = 90f;
        private const float ANGLE_180 = 180f;

        /// <summary>
        /// Calculates vertices for a SpriteShape's Collider, that form a hard edged, rectangular
        /// shape. Immitates what open SpriteShapes are doing with thick Edge colliders, just
        /// with hard edges.
        /// </summary>
        private void DoRectangularCollider()
        {
            DoColliderSection(RECTANGULAR_COLLIDER_LABEL,
                RECTANGULAR_PROPERTY_NAMES,
                GetRectangularPoints);
        }

        private Vector2[] GetRectangularPoints()
        {
            int pointCount = Spline.GetPointCount();
            Vector2[] originalPoints = new Vector2[pointCount];
            List<Vector2> newPoints = new List<Vector2>();

            // Handle first point
			Vector2 toSecond = (Spline.GetPosition(1) - Spline.GetPosition(0)).normalized;
			Vector2 firstPoint = (Vector2)Spline.GetPosition(0) + toSecond * Fixer.borderOffset;
            newPoints.Add(firstPoint);
            newPoints.Add(firstPoint + GetRightNormal(0) * Fixer.height + toSecond * Fixer.trapezoidOffset);
            originalPoints[0] = firstPoint;

            // Handle middle points
            for (int i = 1; i < pointCount-1; i++)
            {
                Vector2 point = Spline.GetPosition(i);
                Vector2 prevPoint = Spline.GetPosition(i - 1);
                Vector2 nextPoint = Spline.GetPosition(i + 1);
                originalPoints[i] = point;

                Vector2 normLeft = GetLeftNormal(i);
                Vector2 normRight = GetRightNormal(i);
                Vector2 normDirLeft = normLeft * Fixer.height;
                Vector2 normDirRight = normRight * Fixer.height;

                Vector2 start1 = prevPoint + normDirLeft;
                Vector2 end1 = point + normDirLeft;
                Vector2 start2 = point + normDirRight;
                Vector2 end2 = nextPoint + normDirRight;

                // Only consider one normal, if left and right point in the same direction.
                float dot = Vector2.Dot(normLeft, normRight);
                if (Mathf.Approximately(dot, 1f))
                {
                    newPoints.Add(end1);
                    continue;
                }

                // Use both normals, if they spread too wide.
                float angle = Vector2.SignedAngle(normLeft, normRight);
                if (angle < 0f || Mathf.Approximately(angle, ANGLE_180))
                {
                    newPoints.Add(end1);
                    newPoints.Add(start2);
                    continue;
                }

                // If the normals face towards each other calculate an intersection.
                if (VectorAddons.TryGetLineIntersection(start1, end1, start2, end2, out Vector2 intersection))
                    newPoints.Add(intersection);
            }

            // Handle last point
            int lastIndex = pointCount - 1;
			Vector2 toPrev = (Spline.GetPosition(lastIndex-1) - Spline.GetPosition(lastIndex)).normalized;
            Vector2 lastPoint = (Vector2)Spline.GetPosition(lastIndex) + toPrev * Fixer.borderOffset;
            originalPoints[lastIndex] = lastPoint;
            newPoints.Add(lastPoint + GetLeftNormal(lastIndex) * Fixer.height + toPrev * Fixer.trapezoidOffset);
            newPoints.Reverse();

            // Combine original and new points
            int finalCount = originalPoints.Length + newPoints.Count;
            Vector2[] finalPoints = new Vector2[finalCount];
            for(int i = 0; i < finalCount; i++)
            {
                if (i < originalPoints.Length)
                    finalPoints[i] = originalPoints[i];
                else
                    finalPoints[i] = newPoints[i - originalPoints.Length];
            }
            return finalPoints;
        }

        private Vector2 GetLeftNormal(int pointIndex)
        {
            Vector2 point = Spline.GetPosition(pointIndex);
            Vector2 prevPoint = Spline.GetPosition(pointIndex - 1);
            Vector2 dir = (point - prevPoint).normalized;
            Vector2 normal = Quaternion.Euler(Vector3.forward * ANGLE_90) * dir;
            return normal;
        }

        private Vector2 GetRightNormal(int pointIndex)
        {
            Vector2 point = Spline.GetPosition(pointIndex);
            Vector2 nextPoint = Spline.GetPosition(pointIndex + 1);
            Vector2 dir = (nextPoint - point).normalized;
            Vector2 normal = Quaternion.Euler(Vector3.forward * ANGLE_90) * dir;
            return normal;
        }
        #endregion
    }
}