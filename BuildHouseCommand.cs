using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace RhinoProject
{
    public class BuildHouseCommand : Command
    {
        public BuildHouseCommand()
        {
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static BuildHouseCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "BuildHouse";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d centerPoint;
            using (GetPoint getPointAction = new GetPoint())
            {
                getPointAction.SetCommandPrompt("Select the center point for the house");
                if (getPointAction.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No point was selected.");
                    return getPointAction.CommandResult();
                }
                centerPoint = getPointAction.Point();
            }
            
            AddBody(doc, centerPoint, 10, 10, 10);

            doc.Views.Redraw();
            RhinoApp.WriteLine("The {0} command added a house to the document.", EnglishName);

            return Result.Success;
        }

        private void AddBody(RhinoDoc doc, Point3d centerPoint, double width, double depth, double height)
        {
            Point3d minCorner = new Point3d(
                centerPoint.X - width / 2.0,
                centerPoint.Y - depth / 2.0,
                centerPoint.Z);

            Point3d maxCorner = new Point3d(
                centerPoint.X + width / 2.0,
                centerPoint.Y + depth / 2.0,
                centerPoint.Z + height);

            Box body = new Box(new BoundingBox(minCorner, maxCorner));
            doc.Objects.AddBox(body);
        }
    }
}
