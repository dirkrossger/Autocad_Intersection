using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace _Autocad_Intersect
{
    public class Get
    {
        public static ObjectId Select(string message, string obj)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (null == doc)
                return ObjectId.Null;
            var db = doc.Database;
            var ed = doc.Editor;

            var peo = new PromptEntityOptions(message);
            ObjectId curId = ObjectId.Null;

            // Ask the user to select a curve
            switch (obj)
            {
                case "Curve":
                    peo.SetRejectMessage("Must be a curve.");
                    peo.AddAllowedClass(typeof(Curve), false);

                    var per = ed.GetEntity(peo);
                    if (per.Status == PromptStatus.OK)
                    {
                        curId = per.ObjectId;
                        return curId;
                    }
                    else
                        return ObjectId.Null;

                case "PlanSurface":
                    peo.SetRejectMessage("Must be a PlanSurface.");
                    peo.AddAllowedClass(typeof(PlaneSurface), false);

                    per = ed.GetEntity(peo);
                    if (per.Status == PromptStatus.OK)
                    {
                        curId = per.ObjectId;
                        return curId;
                    }
                    else
                        return ObjectId.Null;
            }
            return ObjectId.Null;
        }
    }
}
