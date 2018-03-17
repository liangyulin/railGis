using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{
    class CSourceBackup
    {
    }



    //skyline 线路生成管道

    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Reflection;
    //using System.Windows;
    //using System.Windows.Media.Media3D;
    //using TerraExplorerX;

    //namespace Eastdawn.Pipeline.Windows
    //{
    //    /// <summary>
    //    /// 生成管道模型
    //    /// </summary>
    //    public class BuildPipeCommand
    //    {
    //        public bool CanExecute(object parameter)
    //        {
    //            return true;
    //        }

    //        public void Execute(object parameter)
    //        {
    //            var selectedObject = ISGWorld6Extensions.SGWorld.ProjectTree.GetSelectedObject();
    //            if (selectedObject == null)
    //            {
    //                var action = new Action<object>(PipeLineString);
    //                ISGWorld6Extensions.SGWorld.SelectObject<object>(action);
    //            }
    //            else
    //            {
    //                var polyline = selectedObject is ILayer6 ? (selectedObject as ILayer6).SelectedFeatures[0] as IFeature6 : selectedObject;
    //                ILineString route = polyline.GetProperty<IGeometry>(polyline.GetRealType(o => o.WrapComType()), "Geometry") as ILineString;
    //                if (route == null)
    //                    return;
    //                PipeLineString(route);
    //            }
    //        }

    //        void PipeLineString(object lineString)
    //        {
    //            List<ILineString> lines = BezierLineString(lineString);
    //            lines.ForEach(line => DrawOnPolyline(line));
    //        }
    //        // 直线分段
    //        List<ILineString> BezierLineString(object lineString)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;

    //            IEnumerable<Point> routePoints = (lineString as ILineString).Points.Cast();
    //            List<IPosition6> positions = routePoints.Select(p => sgWorld.Creator.CreatePosition(p.X, p.Y, 0, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE)).ToList();
    //            positions.ForEach(p => p.Altitude = sgWorld.Terrain.GetTerrainAltitude(p.X, p.Y));          
    //            List<ILineString> lines = new List<ILineString>();
    //            if (positions.Count == 2)
    //            {
    //                lines.Add(lineString as ILineString);
    //                return lines;
    //            }
    //            // 拐角
    //            for (int i = 1; i < positions.Count-1; i++)
    //            {
    //                IPosition6 prePos = MidPosition(positions[i - 1], positions[i]);
    //                IPosition6 nextPos = MidPosition(positions[i+1], positions[i]);
    //                Vector3D p0 = new Vector3D(prePos.X, prePos.Y, prePos.Altitude);
    //                Vector3D p1 = new Vector3D(positions[i].X, positions[i].Y, positions[i].Altitude);
    //                Vector3D p2 = new Vector3D(nextPos.X, nextPos.Y, nextPos.Altitude);
    //                List<double> lineVertex = Bezier(p0, p1, p2);
    //                var line = sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(lineVertex.ToArray());
    //                lines.Add(line);
    //            }
    //            // 直线段
    //            for (int i = 0; i < positions.Count - 1; i ++)
    //            {
    //                IPosition6 prePos = MidPosition(positions[i], positions[i+1]);
    //                IPosition6 nextPos = MidPosition(positions[i + 1], positions[i]);
    //                if(i == 0)
    //                    lines.Add(CreateLineString(positions[i],prePos));
    //                else if(i == positions.Count - 2)
    //                    lines.Add(CreateLineString(nextPos, positions[positions.Count - 1]));
    //                else
    //                    lines.Add(CreateLineString(nextPos,prePos ));
    //            }

    //            return lines;
    //        }

    //        // 取临近p1 20米的点
    //        IPosition6 MidPosition(IPosition6 p0, IPosition6 p1)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            double dis = p0.DistanceTo(p1);
    //            var p2 = p0.MoveToward(p1, dis - 20);
    //            p2.Altitude = sgWorld.Terrain.GetTerrainAltitude(p2.X, p2.Y);
    //            return p2;
    //        }
    //        // 两点创建内插直线
    //        ILineString CreateLineString(IPosition6 p0, IPosition6 p1)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            List<double> lineVertex = new List<double>();
    //            List<IPosition6> positions = new List<IPosition6>();
    //            GetIPositions(p0, p1, 100, ref positions);
    //            positions.ForEach(pos =>
    //            {
    //                lineVertex.Add(pos.X);
    //                lineVertex.Add(pos.Y);
    //                lineVertex.Add(pos.Altitude);
    //            });
    //            return sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(lineVertex.ToArray());
    //        }
    //        // 直线段分段 高程内插
    //        public static void GetIPositions(IPosition6 firstPosition, IPosition6 lastPosition, double interval, ref List<IPosition6> positions)
    //        {
    //            int SegmentLength = (int)Math.Floor(firstPosition.DistanceTo(lastPosition) / interval);
    //            var dh = ((lastPosition.Altitude - firstPosition.Altitude) / SegmentLength);
    //            positions.Add(firstPosition);
    //            for (int i = 1; i < SegmentLength; i++)
    //            {
    //                var pos = firstPosition.MoveToward(lastPosition, i * interval);
    //                pos.Altitude = firstPosition.Altitude + i*dh;
    //                positions.Add(pos);
    //            }
    //            positions.Add(lastPosition);
    //        }
    //        // 生成bezier曲线
    //        List<double> Bezier(Vector3D p0, Vector3D p1, Vector3D p2)
    //        {
    //            List<double> lineVertex = new List<double>();
    //            for (int i = 0; i < 41; i++)
    //            {
    //                double t = i / 40.0;
    //                Vector3D bezierPt = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    //                lineVertex.Add(bezierPt.X);
    //                lineVertex.Add(bezierPt.Y);
    //                lineVertex.Add(bezierPt.Z);
    //            }
    //            return lineVertex;
    //        }
    //        // linestring 绘制模型
    //        void DrawOnPolyline(object lineString)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            //IEnumerable<Point> routePoints = (lineString as ILineString).Points.Cast();
    //            //List<IPosition6> positions = routePoints.Select(p => sgWorld.Creator.CreatePosition(p.X, p.Y, 0, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE)).ToList();
    //            //positions.ForEach(p => p.Altitude = sgWorld.Terrain.GetTerrainAltitude(p.X, p.Y));
    //            List<IPosition6> positions = new List<IPosition6>();
    //            foreach (IPoint point in (lineString as ILineString).Points)
    //            {
    //                 var pos = sgWorld.Creator.CreatePosition(point.X, point.Y, point.Z, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
    //                 positions.Add(pos);
    //            }

    //            for (int i = 0; i < positions.Count - 1; i++)
    //            {
    //                IPosition6 currCoord = positions[i].AimTo(positions[i + 1]);
    //                var SegmentLength = currCoord.DistanceTo(positions[i + 1]);
    //                DrawPipe(currCoord, SegmentLength+0.5);
    //            }
    //        }
    //        // DrawPipe
    //        void DrawPipe(IPosition6 position, double SegmentLength)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            var texture = Application.Current.GetStartupPath() + @"\pipeTexture.bmp";
    //            var radius = 10;

    //            int groupId = sgWorld.ProjectTree.FindOrCreateGroup("石油管道");
    //            //position.Altitude = 0;
    //            var TEObj = sgWorld.Creator.CreateCylinder(position, radius, SegmentLength, sgWorld.Creator.CreateColor(56, 56, 56, 0), sgWorld.Creator.CreateColor(56, 56, 56, 255), 16, groupId, "");
    //            TEObj.Position.Pitch = -90 + position.Pitch;
    //            TEObj.LineStyle.Color.SetAlpha(0);
    //            TEObj.FillStyle.Texture.FileName = texture;
    //            TEObj.Visibility.MaxVisibilityDistance = 50000;
    //            TEObj.FillStyle.Texture.RotateAngle = FixTextureAngle(TEObj.Position.Pitch, TEObj.NumberOfSegments);

    //        }
    //        // FixTextureAngle
    //        double FixTextureAngle(double pitch, double NumberofSegments)
    //        {
    //            var FixAngle = Math.Atan(Math.Sin(Math.PI / NumberofSegments) / Math.Cos(pitch / 180 * Math.PI)) * 180 / Math.PI + 90;
    //            return FixAngle;
    //        }
    //    }
    //}
}
