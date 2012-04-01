using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gears
{
    public partial class GearDrawing : UserControl
    {
        public GearDrawing()
        {
            InitializeComponent();
        }

        public void setInfo(GearInfo info)
        {
            info_ = info;
            recalc();
        }

        void recalc()
        {
            calc_ = calcGears(info_);
            Invalidate();
        }

        GearInfo info_;
        GearCalc[] calc_;
        float phase_;

        private void DrawGearOutline(Graphics g, PointF center, float scale, float radius, 
            float baseRadius, float addendum, float dedendum, int numTeeth, float toothPhase)
        {
            g.DrawArc(Pens.Yellow,
                new RectangleF(
                    center.X - baseRadius * scale,
                    center.Y - baseRadius * scale,
                    baseRadius * 2 * scale,
                    baseRadius * 2 * scale),
                0, 360);
            g.DrawArc(Pens.Black,
                new RectangleF(
                    center.X - radius * scale,
                    center.Y - radius * scale,
                    radius * 2 * scale,
                    radius * 2 * scale),
                0, 360);
            g.DrawArc(Pens.LightGray,
                new RectangleF(
                    center.X - (radius - dedendum) * scale,
                    center.Y - (radius - dedendum) * scale,
                    (radius * 2 - dedendum * 2) * scale,
                    (radius * 2 - dedendum * 2) * scale),
                0, 360);
            g.DrawArc(Pens.LightGray,
                new RectangleF(
                    center.X - (radius + addendum) * scale,
                    center.Y - (radius + addendum) * scale,
                    (radius * 2 + addendum * 2) * scale,
                    (radius * 2 + addendum * 2) * scale),
                0, 360);
            g.DrawLine(Pens.Magenta, new PointF(center.X - 3, center.Y - 3), new PointF(center.X + 3, center.Y + 3));
            g.DrawLine(Pens.Magenta, new PointF(center.X - 3, center.Y + 3), new PointF(center.X + 3, center.Y - 3));
            for (int i = 0; i < numTeeth; ++i)
            {
                double dx = Math.Sin((i + toothPhase) * Math.PI * 2 / numTeeth);
                double dy = -Math.Cos((i + toothPhase) * Math.PI * 2 / numTeeth);
                g.DrawLine(
                    Pens.LightGray, 
                    new PointF(
                        (float)(center.X + dx * (radius - dedendum) * scale),
                        (float)(center.Y + dy * (radius - dedendum) * scale)),
                    new PointF(
                        (float)(center.X + dx * (radius + addendum) * scale),
                        (float)(center.Y + dy * (radius + addendum) * scale)));

            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                Point center = new Point(Width/2, Height/2);
                float extent = (float)Math.Min(Width, Height);
                if (calc_ == null || !calc_[0].ok || !calc_[1].ok)
                {
                    e.Graphics.DrawString("No information", System.Drawing.SystemFonts.CaptionFont, 
                        Brushes.Red, new PointF(20, 30));
                    return;
                }
                float scale = (float)(extent / (calc_[0].radius * 2 + calc_[1].radius * 2 + calc_[0].addendum + calc_[1].addendum));
                DrawGearOutline(e.Graphics, new PointF(center.X, 
                        (float)(calc_[0].addendum + calc_[0].radius) * scale),
                    scale, (float)calc_[0].radius, (float)calc_[0].baseRadius, (float)calc_[0].addendum, (float)calc_[0].dedendum,
                    (int)info_.aNumTeeth, - phase_ + 0.5f * (1 - ((int)info_.aNumTeeth & 1)));
                DrawGearOutline(e.Graphics, new PointF(center.X, 
                        (float)(calc_[0].addendum + calc_[0].radius * 2 + calc_[1].radius) * scale),
                    scale, (float)calc_[1].radius, (float)calc_[1].baseRadius, (float)calc_[1].addendum, (float)calc_[1].dedendum, 
                    (int)info_.bNumTeeth, phase_);

                //  center line
                float topCenter = (float)(calc_[0].addendum + calc_[0].radius);
                float bottomCenter = (float)(calc_[0].addendum + calc_[0].radius * 2 + calc_[1].radius);
                e.Graphics.DrawLine(Pens.Blue, new PointF(extent * 0.05f, topCenter * scale),
                    new PointF(extent * 0.05f, bottomCenter * scale));
                e.Graphics.DrawLine(Pens.LightGray, new PointF(extent * 0.05f, topCenter * scale),
                    new PointF(extent * 0.5f, topCenter * scale));
                e.Graphics.DrawLine(Pens.LightGray, new PointF(extent * 0.05f, bottomCenter * scale),
                    new PointF(extent * 0.5f, bottomCenter * scale));
                e.Graphics.DrawString(String.Format("{0:0.####}", calc_[0].centerDistance), 
                    System.Drawing.SystemFonts.CaptionFont, Brushes.Blue, 
                    new PointF(extent * 0.05f + 3, (topCenter + bottomCenter) * 0.5f * scale));

                float topDiameter = (float)calc_[0].addendum;
                float midDiameter = (float)(topDiameter + 2 * calc_[0].radius);
                float botDiameter = (float)(midDiameter + 2 * calc_[1].radius);
                float diaRight = Width - extent * 0.05f;
                e.Graphics.DrawLine(Pens.Blue, new PointF(diaRight, topDiameter * scale),
                    new PointF(diaRight, botDiameter * scale));
                e.Graphics.DrawLine(Pens.LightGray, new PointF(center.X + extent * 0.05f, topDiameter * scale),
                    new PointF(diaRight, topDiameter * scale));
                e.Graphics.DrawLine(Pens.LightGray, new PointF(center.X + extent * 0.05f, midDiameter * scale),
                    new PointF(diaRight, midDiameter * scale));
                e.Graphics.DrawLine(Pens.LightGray, new PointF(center.X + extent * 0.05f, botDiameter * scale),
                    new PointF(diaRight, botDiameter * scale));
                e.Graphics.DrawString(String.Format("{0:0.####}", calc_[0].radius * 2),
                    System.Drawing.SystemFonts.CaptionFont, Brushes.Blue,
                    new PointF(diaRight - 50, (topDiameter + midDiameter) * 0.5f * scale));
                e.Graphics.DrawString(String.Format("{0:0.####}", calc_[1].radius * 2),
                    System.Drawing.SystemFonts.CaptionFont, Brushes.Blue,
                    new PointF(diaRight - 50, (midDiameter + botDiameter) * 0.5f * scale));

                double rmlen = calc_[0].baseRadius + calc_[1].baseRadius;
                double rhlen = calc_[0].radius + calc_[1].radius;
                double llen = Math.Sqrt(rhlen * rhlen - rmlen * rmlen);
                double angle = Math.Atan(llen / rmlen);
                double asin = Math.Sin(angle);
                double acos = Math.Cos(angle);
                double alx = calc_[0].baseRadius * -asin;
                double aly = calc_[0].baseRadius * acos;
                double blx = calc_[1].baseRadius * asin;
                double bly = calc_[1].baseRadius * -acos;
                e.Graphics.DrawLine(Pens.Crimson,
                    new PointF(center.X + (float)alx * scale, (float)(calc_[0].addendum + calc_[0].radius + aly) * scale),
                    new PointF(center.X + (float)blx * scale, (float)(calc_[0].addendum + calc_[0].radius * 2 + calc_[1].radius + bly) * scale));
                e.Graphics.DrawLine(Pens.Crimson,
                    new PointF(center.X + (float)alx * scale, (float)(calc_[0].addendum + calc_[0].radius + aly) * scale),
                    new PointF(center.X, (float)(calc_[0].addendum + calc_[0].radius) * scale));
                e.Graphics.DrawLine(Pens.Crimson,
                    new PointF(center.X, (float)(calc_[0].addendum + calc_[0].radius * 2 + calc_[1].radius) * scale),
                    new PointF(center.X + (float)blx * scale, (float)(calc_[0].addendum + calc_[0].radius * 2 + calc_[1].radius + bly) * scale));
            }
            catch (Exception x)
            {
                GC.KeepAlive(x);
                e.Graphics.DrawString("Parameters cannot be used.", System.Drawing.SystemFonts.CaptionFont,
                    Brushes.Red, new PointF(20, 30));
                e.Graphics.DrawString(x.Message, System.Drawing.SystemFonts.CaptionFont,
                    Brushes.Red, new PointF(20, 50));
            }
        }

        public static GearCalc[] calcGears(GearInfo info)
        {
            GearCalc[] ret = new GearCalc[2];
            if (!info.ok)
            {
                return ret;
            }
            ret[0].moduleInch = 1.0 / info.pitchGpi;
            ret[1].moduleInch = 1.0 / info.pitchGpi;
            ret[0].radius = (double)info.aNumTeeth * ret[0].moduleInch / 2;
            ret[1].radius = (double)info.bNumTeeth * ret[1].moduleInch / 2;
            ret[0].baseRadius = ret[0].radius * Math.Cos(info.pressureAngle * Math.PI / 180);
            ret[1].baseRadius = ret[1].radius * Math.Cos(info.pressureAngle * Math.PI / 180);
            ret[0].centerDistance = ret[0].radius + ret[1].radius;
            ret[1].centerDistance = ret[0].radius + ret[1].radius;
            ret[0].addendum = ret[0].moduleInch * info.stubRatio * (1 + info.profileShift);
            ret[1].addendum = ret[0].moduleInch * info.stubRatio * (1 - info.profileShift);
            ret[0].dedendum = ret[0].moduleInch * info.stubRatio * (1.25f - info.profileShift);
            ret[1].dedendum = ret[0].moduleInch * info.stubRatio * (1.25f + info.profileShift);
            ret[0].profileShift = info.profileShift;
            ret[1].profileShift = -info.profileShift;
            ret[0].numTeeth = info.aNumTeeth;
            ret[1].numTeeth = info.bNumTeeth;
            ret[0].ok = true;
            ret[1].ok = true;
            return ret;
        }

        private void GearDrawing_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            phase_ += 0.01f;
            if (phase_ >= 1)
            {
                phase_ -= 1;
            }
            Invalidate();
        }
    }

    public struct GearInfo
    {
        public bool ok;
        public int aNumTeeth;
        public int bNumTeeth;
        public double pitchGpi;
        public double pressureAngle;
        public double stubRatio;
        public double profileShift;
    }

    public struct GearCalc
    {
        public bool ok;
        public double centerDistance;
        public double moduleInch;
        public double radius;
        public double baseRadius;
        public double addendum;
        public double dedendum;
        public double profileShift;
        public int numTeeth;
    }
}
