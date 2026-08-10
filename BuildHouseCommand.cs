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
            AddRoof(doc, centerPoint, 10, 10, 10);
            AddDoor(doc, centerPoint, 10, 10, 10);

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

        private void AddRoof(RhinoDoc doc, Point3d centerPoint, double width, double depth, double height)
        {
            double roofRise = height / 2.0;
            double topZ = centerPoint.Z + height;
            double frontY = centerPoint.Y - depth / 2.0;
            double backY = centerPoint.Y + depth / 2.0;

            Point3d topFrontLeft = new Point3d(centerPoint.X - width / 2.0, frontY, topZ);
            Point3d topFrontRight = new Point3d(centerPoint.X + width / 2.0, frontY, topZ);
            Point3d topBackLeft = new Point3d(centerPoint.X - width / 2.0, backY, topZ);
            Point3d topBackRight = new Point3d(centerPoint.X + width / 2.0, backY, topZ);

            Point3d ridgeFront = new Point3d(centerPoint.X, frontY, topZ + roofRise);
            Point3d ridgeBack = new Point3d(centerPoint.X, backY, topZ + roofRise);

            Brep leftSlope = Brep.CreateFromCornerPoints(
                topFrontLeft, topBackLeft, ridgeBack, ridgeFront, doc.ModelAbsoluteTolerance);
            Brep rightSlope = Brep.CreateFromCornerPoints(
                topFrontRight, topBackRight, ridgeBack, ridgeFront, doc.ModelAbsoluteTolerance);

            doc.Objects.AddBrep(leftSlope);
            doc.Objects.AddBrep(rightSlope);
        }

        private void AddDoor(RhinoDoc doc, Point3d centerPoint, double width, double depth, double height)
        {
            double doorWidth = width * 0.2;
            double doorHeight = height * 0.6;
            double frontY = centerPoint.Y - depth / 2.0;

            double doorX0 = centerPoint.X - width * 0.25;
            double doorX1 = doorX0 + doorWidth;

            Point3d doorBottomLeft = new Point3d(doorX0, frontY, centerPoint.Z);
            Point3d doorBottomRight = new Point3d(doorX1, frontY, centerPoint.Z);
            Point3d doorTopRight = new Point3d(doorX1, frontY, centerPoint.Z + doorHeight);
            Point3d doorTopLeft = new Point3d(doorX0, frontY, centerPoint.Z + doorHeight);

            Brep door = Brep.CreateFromCornerPoints(
                doorBottomLeft, doorBottomRight, doorTopRight, doorTopLeft, doc.ModelAbsoluteTolerance);

            doc.Objects.AddBrep(door);
        }
    }
}
