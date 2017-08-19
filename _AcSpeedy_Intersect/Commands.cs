using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace _Autocad_Intersect
{
    public class Commands
    {
        #region Intersection between 3dPoly and PlanSurface: CPI
        [CommandMethod("CPI")]

        public void CurvePlaneIntersection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            ObjectId curId = Get.Select("\nSelect Curve", "Curve");
            ObjectId planeId = Get.Select("\nSelect plane surface", "PlanSurface");

            if (curId == ObjectId.Null || planeId == ObjectId.Null)
                return;

            using (var tr = doc.TransactionManager.StartTransaction())
            {
                // Start by opening the plane
                var plane = tr.GetObject(planeId, OpenMode.ForRead) as PlaneSurface;
                if (plane != null)
                {
                    // Get the PlaneSurface's defining GeLib plane
                    var p = plane.GetPlane();

                    // Open our curve...
                    var cur = tr.GetObject(curId, OpenMode.ForRead) as Curve;
                    if (cur != null) // Should never fail
                    {
                        var pts = p.IntersectWith(cur);

                        // If we have results we'll place them in modelspace
                        var ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForWrite);
                        foreach (Point3d pt in pts)
                        {
                            // Create a point in the drawing for each intersection
                            var dbp = new DBPoint(pt);
                            dbp.ColorIndex = 2; // Make them yellow
                            ms.AppendEntity(dbp);
                            tr.AddNewlyCreatedDBObject(dbp, true);

                            // Quick test to make sure each point lies on our source curve
                            doc.Editor.WriteMessage("\nPoint is {0} curve.", cur.IsOn(pt) ? "on" : "off");
                        }
                    }
                }
                tr.Commit();
            }
        }
        #endregion

        [CommandMethod("PPI")]
        public void PolyPolyIntersection()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            ObjectId curId = Get.Select("\nSelect Curve1", "Curve");
            ObjectId polyId = Get.Select("\nSelect Curve2", "Curve");

            if (curId == ObjectId.Null || polyId == ObjectId.Null)
                return;


            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                Curve poly = tr.GetObject(polyId, OpenMode.ForRead) as Curve;
                ObjectId regId = _Autocad_Intersect.Region.Extensions.CreateFromCurve(poly);

                if(regId.IsNull)
                  return;
                // Start by opening the plane
                var plane = tr.GetObject(regId, OpenMode.ForRead) as PlaneSurface;
                if (plane != null)
                {
                    // Get the PlaneSurface's defining GeLib plane
                    var p = plane.GetPlane();

                    // Open our curve...
                    var cur = tr.GetObject(curId, OpenMode.ForRead) as Curve;
                    if (cur != null) // Should never fail
                    {
                        var pts = p.IntersectWith(cur);

                        // If we have results we'll place them in modelspace
                        var ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(doc.Database), OpenMode.ForWrite);
                        foreach (Point3d pt in pts)
                        {
                            // Create a point in the drawing for each intersection
                            var dbp = new DBPoint(pt);
                            dbp.ColorIndex = 2; // Make them yellow
                            ms.AppendEntity(dbp);
                            tr.AddNewlyCreatedDBObject(dbp, true);

                            // Quick test to make sure each point lies on our source curve
                            doc.Editor.WriteMessage("\nPoint is {0} curve.", cur.IsOn(pt) ? "on" : "off");
                        }
                    }
                }
                tr.Commit();
            }
        }



    }

    public class _AcSpeedy : IExtensionApplication
    {
        public void Initialize()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage("\n-> Intersection between 3dPoly and PlanSurface: CPI");
            doc.Editor.WriteMessage("\n-> Intersection between 3dPoly and 3dPoly: PPI");
        }

        public void Terminate()
        {
        }
    }

}
