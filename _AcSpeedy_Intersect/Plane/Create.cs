using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace _Autocad_Intersect_Plane
{
    public class Extension
    {
        public static void CreatePlane()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table record for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                // Create a 3D solid box
                using (Solid3d acSol3D = new Solid3d())
                {
                    acSol3D.CreateBox(5, 7, 10);

                    // Position the center of the 3D solid at (5,5,0) 
                    acSol3D.TransformBy(Matrix3d.Displacement(new Point3d(5, 5, 0) -
                                                                Point3d.Origin));

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acSol3D);
                    acTrans.AddNewlyCreatedDBObject(acSol3D, true);

                    // Create a copy of the original 3D solid and change the color of the copy
                    Solid3d acSol3DCopy = acSol3D.Clone() as Solid3d;
                    acSol3DCopy.ColorIndex = 1;

                    // Define the mirror plane
                    Plane acPlane = new Plane(new Point3d(1.25, 0, 0),
                                                new Point3d(1.25, 2, 0),
                                                new Point3d(1.25, 2, 2));

                    // Mirror the 3D solid across the plane
                    acSol3DCopy.TransformBy(Matrix3d.Mirroring(acPlane));

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acSol3DCopy);
                    acTrans.AddNewlyCreatedDBObject(acSol3DCopy, true);
                }

                // Save the new objects to the database
                acTrans.Commit();
            }
        }

        public static Plane CreatePlane(Point3dCollection ptColl)
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Define the mirror plane
            Plane acPlane = new Plane(new Point3d(ptColl[0].X, ptColl[0].Y, ptColl[0].Z),
                                      new Point3d(ptColl[0].X, ptColl[0].Y, ptColl[0].Z + 1000),
                                      new Point3d(ptColl[1].X, ptColl[1].Y, ptColl[1].Z));
            if (acPlane == null)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Error: unable to create Plane:");
                return null;
            }
            else
                return acPlane;
        }

    }
}
