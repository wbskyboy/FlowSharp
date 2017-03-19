﻿/* 
* Copyright (c) Marc Clifton
* The Code Project Open License (CPOL) 1.02
* http://www.codeproject.com/info/cpol10.aspx
*/

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Clifton.Core.ExtensionMethods;

using FlowSharpLib;

namespace FlowSharpWindowsControlShapes
{
    [ExcludeFromToolbox]
    public class TrackbarShape : ControlShape
    {
        public string ValueChangedName { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public TrackbarShape(Canvas canvas) : base(canvas)
        {
            TrackBar tb = new TrackBar();
            control = tb;
            canvas.Controls.Add(control);
            tb.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, System.EventArgs e)
        {
            Send(ValueChangedName);
            //string url = "http://localhost:8002/" + ValueChangedName + "?ShapeName=" + Name;
            //url = AppendData(url);
            //Http.Get(url);
        }

        public override ElementProperties CreateProperties()
        {
            return new TrackbarShapeProperties(this);
        }

        public override void Serialize(ElementPropertyBag epb, IEnumerable<GraphicElement> elementsBeingSerialized)
        {
            Json["ValueChangedName"] = ValueChangedName;
            Json["Minimum"] = Minimum.ToString();
            Json["Maximum"] = Maximum.ToString();
            base.Serialize(epb, elementsBeingSerialized);
        }

        public override void Deserialize(ElementPropertyBag epb)
        {
            base.Deserialize(epb);
            string name;

            if (Json.TryGetValue("ValueChangedName", out name))
            {
                ValueChangedName = name;
            }

            string data;

            if (Json.TryGetValue("Minimum", out data))
            {
                Minimum = data.to_i();
            }

            if (Json.TryGetValue("Maximum", out data))
            {
                Maximum = data.to_i();
            }
        }

        protected override string AppendData(string url)
        {
            base.AppendData(url);
            url += "&Value=" + ((TrackBar)control).Value;

            return url;
        }

        public override void Draw(Graphics gr, bool showSelection = true)
        {
            base.Draw(gr, showSelection);
            Rectangle r = ZoomRectangle.Grow(-4);
            ((TrackBar)control).Minimum = Minimum;
            ((TrackBar)control).Maximum = Maximum;
            control.Location = r.Location;
            control.Size = r.Size;
            control.Text = Text;
            control.Enabled = Enabled;
            control.Visible = Visible;
        }
    }

    [ToolboxShape]
    public class ToolboxTrackbarShape : GraphicElement
    {
        public const string TOOLBOX_TEXT = "trckbar";

        protected Brush brush = new SolidBrush(Color.Black);

        public ToolboxTrackbarShape(Canvas canvas) : base(canvas)
        {
            TextFont.Dispose();
            TextFont = new Font(FontFamily.GenericSansSerif, 8);
        }

        public override GraphicElement CloneDefault(Canvas canvas)
        {
            return CloneDefault(canvas, Point.Empty);
        }

        public override GraphicElement CloneDefault(Canvas canvas, Point offset)
        {
            TrackbarShape shape = new TrackbarShape(canvas);
            shape.DisplayRectangle = shape.DefaultRectangle().Move(offset);
            shape.UpdateProperties();
            shape.UpdatePath();

            return shape;
        }

        public override void Draw(Graphics gr, bool showSelection = true)
        {
            SizeF size = gr.MeasureString(TOOLBOX_TEXT, TextFont);
            Point textpos = DisplayRectangle.Center().Move((int)(-size.Width / 2), (int)(-size.Height / 2));
            gr.DrawString(TOOLBOX_TEXT, TextFont, brush, textpos);
            base.Draw(gr, showSelection);
        }
    }
}

