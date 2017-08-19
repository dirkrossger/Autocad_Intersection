using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace _Autocad_Intersect
{


    public static class Extensions
    {
        ///<summary>
        /// Returns an array of Point3d objects from a Point3dCollection.
        ///</summary>
        ///<returns>An array of Point3d objects.</returns>

        public static Point3d[] ToArray(this Point3dCollection pts)
        {
            var res = new Point3d[pts.Count];
            pts.CopyTo(res, 0);
            return res;
        }

        ///<summary>
        /// Get the intersection points between this planar entity and a curve.
        ///</summary>
        ///<param name="cur">The curve to check intersections against.</param>
        ///<returns>An array of Point3d intersections.</returns>

        public static Point3d[] IntersectWith(this Plane p, Curve cur)
        {
            var pts = new Point3dCollection();

            // Get the underlying GeLib curve
            var gcur = cur.GetGeCurve();

            // Project this curve onto our plane
            var proj = gcur.GetProjectedEntity(p, p.Normal) as Curve3d;
            if (proj != null)
            {
                // Create a DB curve from the projected Ge curve
                using (var gcur2 = Curve.CreateFromGeCurve(proj))
                {
                    // Check where it intersects with the original curve:
                    // these should be our intersection points on the plane
                    cur.IntersectWith(gcur2, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                }
            }
            return pts.ToArray();
        }

        ///<summary>
        /// Test whether a point is on this curve.
        ///</summary>
        ///<param name="pt">The point to check against this curve.</param>
        ///<returns>Boolean indicating whether the point is on the curve.</returns>

        public static bool IsOn(this Curve cv, Point3d pt)
        {
            try
            {
                // Return true if operation succeeds

                var p = cv.GetClosestPointTo(pt, false);
                return (p - pt).Length <= Tolerance.Global.EqualPoint;
            }
            catch { }

            // Otherwise we return false

            return false;
        }
    }

}
