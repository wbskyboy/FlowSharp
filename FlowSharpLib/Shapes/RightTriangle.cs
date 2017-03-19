﻿/* 
* Copyright (c) Marc Clifton
* The Code Project Open License (CPOL) 1.02
* http://www.codeproject.com/info/cpol10.aspx
*/

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlowSharpLib
{
    [ToolboxOrder(5)]
    public class RightTriangle : GraphicElement
    {
        protected Point[] path;

        public RightTriangle(Canvas canvas) : base(canvas)
        {
        }

        public override List<ConnectionPoint> GetConnectionPoints()
        {
            List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
            connectionPoints.Add(new ConnectionPoint(GripType.LeftMiddle, ZoomRectangle.LeftMiddle()));
            connectionPoints.Add(new ConnectionPoint(GripType.RightMiddle, ZoomRectangle.RightMiddle()));
            connectionPoints.Add(new ConnectionPoint(GripType.TopLeft, ZoomRectangle.TopLeftCorner()));
            connectionPoints.Add(new ConnectionPoint(GripType.BottomLeft, ZoomRectangle.BottomLeftCorner()));

            return connectionPoints;
        }

        public override void UpdatePath()
        {
            Rectangle r = ZoomRectangle;
            path = new Point[]
            {
                new Point(r.X + r.Width, r.Y + r.Height/2),        // right, middle
                new Point(r.X,           r.Y),                              // left, top
                new Point(r.X,           r.Y + r.Height),          // left, bottom
                new Point(r.X + r.Width, r.Y + r.Height/2),        // right, middle
            };
        }

        protected Point[] ZPath()
        {
            Rectangle r = ZoomRectangle; 
            r.X = 0;
            r.Y = 0;
            int adjust = (int)((BorderPen.Width + 0) / 2);
            Point[] path = new Point[]
            {
                new Point(r.X + r.Width - adjust, r.Y + r.Height/2),        // right, middle
                new Point(r.X + adjust,           r.Y + adjust),                              // left, top
                new Point(r.X + adjust,           r.Y + r.Height - adjust),          // left, bottom
                new Point(r.X + r.Width - adjust, r.Y + r.Height/2),        // right, middle
            };

            return path;
        }

        public override void Draw(Graphics gr, bool showSelection = true)
        {
            // While this clips the region, the lines are no longer antialiased.
            /*
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(path);
            Region region = new Region(gp);
            gr.SetClip(region, CombineMode.Replace);
            gr.IntersectClip(ZoomRectangle);
            ...
            gr.ResetClip();
            */

            // Drawing onto a bitmap that constrains the drawing area fixes the trail problem
            // but still has issues with larger pen widths (try 10) as triangle points are clipped.
            Rectangle r = ZoomRectangle.Grow(2);
            Bitmap bitmap = new Bitmap(r.Width, r.Height);
            Graphics g2 = Graphics.FromImage(bitmap);
            g2.SmoothingMode = SmoothingMode.AntiAlias;
            Point[] path = ZPath();
            g2.FillPolygon(FillBrush, path);
            g2.DrawPolygon(BorderPen, path);
            gr.DrawImage(bitmap, ZoomRectangle.X, ZoomRectangle.Y);
            bitmap.Dispose();
            g2.Dispose();
            base.Draw(gr, showSelection);
        }
    }
}
