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
        [CommandMethod("CPI")]
        public void CurvePlaneIntersection()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (null == doc)
                return;
            var db = doc.Database;
            var ed = doc.Editor;

            // Ask the user to select a curve

            var peo = new PromptEntityOptions("\nSelect a curve");
            peo.SetRejectMessage("Must be a curve.");
            peo.AddAllowedClass(typeof(Curve), false);

            var per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK)
                return;

            var curId = per.ObjectId;

            // And a PlaneSurface to check intersections against

            var peo2 = new PromptEntityOptions("\nSelect plane surface");
            peo.SetRejectMessage("Must be a planar surface.");
            peo.AddAllowedClass(typeof(PlaneSurface), false);

            var per2 = ed.GetEntity(peo2);
            if (per2.Status != PromptStatus.OK)
                return;

            var planeId = per2.ObjectId;

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

                        var ms =
                          (BlockTableRecord)tr.GetObject(
                            SymbolUtilityServices.GetBlockModelSpaceId(db),
                            OpenMode.ForWrite
                          );

                        foreach (Point3d pt in pts)
                        {
                            // Create a point in the drawing for each intersection

                            var dbp = new DBPoint(pt);
                            dbp.ColorIndex = 2; // Make them yellow
                            ms.AppendEntity(dbp);
                            tr.AddNewlyCreatedDBObject(dbp, true);

                            // Quick test to make sure each point lies on our source curve

                            ed.WriteMessage(
                              "\nPoint is {0} curve.", cur.IsOn(pt) ? "on" : "off"
                            );
                        }
                    }
                }
                tr.Commit();
            }
        }
    }

}
