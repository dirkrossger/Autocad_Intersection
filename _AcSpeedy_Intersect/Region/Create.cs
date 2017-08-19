using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace _Autocad_Intersect_Region
{
    public class Extensions
    {
        public static ObjectId CreateFromCurve(Curve poly)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {

                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = tr.GetObject(doc.Database.BlockTableId,
                                                OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;


                //Curve poly = tr.GetObject(polyId, OpenMode.ForRead) as Curve;
                DBObjectCollection objColl = new DBObjectCollection();
                objColl.Add(poly);

                DBObjectCollection myRegionColl = new DBObjectCollection();
                Autodesk.AutoCAD.DatabaseServices.Region objreg = new Autodesk.AutoCAD.DatabaseServices.Region();
                DBObjectCollection objRegions = new DBObjectCollection();
                try
                {
                    objRegions = Autodesk.AutoCAD.DatabaseServices.Region.CreateFromCurves(objColl);
                    objreg = objRegions[0] as Autodesk.AutoCAD.DatabaseServices.Region;

                    acBlkTblRec.AppendEntity(objreg);
                    tr.AddNewlyCreatedDBObject(objreg, true);

                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    // eInvalidInput exception
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Error: unable to create region collection:\n" + ex.Message);

                }
                
                tr.Commit();
                return objreg.ObjectId;
            }
        }
    }
}
