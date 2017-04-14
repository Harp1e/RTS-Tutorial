using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS {
    public static class WorkManager
    {
        public static Rect CalculateSelectionBox (Bounds selectionBounds, Rect playingArea)
        {
            float cx = selectionBounds.center.x;
            float cy = selectionBounds.center.y;
            float cz = selectionBounds.center.z;

            float ex = selectionBounds.extents.x;
            float ey = selectionBounds.extents.y;
            float ez = selectionBounds.extents.z;

            List<Vector3> corners = new List<Vector3> ();
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz + ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy + ey, cz - ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz + ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz + ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx + ex, cy - ey, cz - ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz + ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy + ey, cz - ez)));
            corners.Add (Camera.main.WorldToScreenPoint (new Vector3 (cx - ex, cy - ey, cz - ez)));

            Bounds screenBounds = new Bounds (corners[0], Vector3.zero);
            for (int i = 1  ; i < corners.Count; i++)
            {
                screenBounds.Encapsulate (corners[i]);
            }

            float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
            float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
            float selectBoxWidth = 2 * screenBounds.extents.x;
            float selectBoxHeight = 2 * screenBounds.extents.y;

            return new Rect (selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
        }
    }
}
