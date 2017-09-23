using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FocusLockable
{
    public static class BoundsExtension
    {
        // Corners
        public const int LBF = 0;
        public const int LBB = 1;
        public const int LTF = 2;
        public const int LTB = 3;
        public const int RBF = 4;
        public const int RBB = 5;
        public const int RTF = 6;
        public const int RTB = 7;

        public static void GetCornerPositionsFromRendererBounds(this Bounds bounds, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[BoundsExtension.LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[BoundsExtension.LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[BoundsExtension.LTF] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[BoundsExtension.LTB] = new Vector3(leftEdge, topEdge, backEdge);
            positions[BoundsExtension.RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[BoundsExtension.RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[BoundsExtension.RTF] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[BoundsExtension.RTB] = new Vector3(rightEdge, topEdge, backEdge);
        }
    }
}