using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PointF = System.Drawing.PointF;

namespace Gears
{
    public class GCodeGenerator
    {
        public GCodeGenerator(GCodeWorkOrder wo)
        {
            this.work = wo;
        }
        GCodeWorkOrder work;
        double tabHeight = 0.15;
        double tabWidth = 0.2;
        double cutThroughMargin = 0.05f;
        
        public string generateMainMill(bool includeTooth)
        {
            IGeometryOutput output = MakeOutput();
            generatePrefix(output, work.mainMill);
            generateHoleMilling(output);
            generateRidgeTopMilling(output);
            generateFaceMilling(output);
            generateToothTopMilling(output);
            generateToothOutlineMilling(output);
            if (includeTooth)
            {
                generateToothProfileMilling(output, work.numTeeth, work.mainMill);
            }
            generateSuffix(output);
            return output.Finish();
        }

        public string generateToothMill()
        {
            if (!work.useToothMill)
            {
                return null;
            }
            IGeometryOutput output = MakeOutput();
            generatePrefix(output, work.toothMill);
            generateToothProfileMilling(output, work.numTeeth, work.toothMill);
            generateSuffix(output);
            return output.Finish();
        }

        private IGeometryOutput MakeOutput()
        {
            if (work.fileInfo.fileFormat == FileFormat.svg)
            {
                return new SVGGeometryOutput(new StringBuilder());
            }
            return new GCodeGeometryOutput(new StringBuilder());
        }

        private void generatePrefix(IGeometryOutput sb, MillInfo mill)
        {
            sb.BeginGroup("generatePrefix");
            sb.Comment(String.Format("Diameter {0} Cut Depth {1} Pass Depth {2} Feed {3} Speed {4}",
                mill.diameter, mill.cuttingDepth, mill.passDepth, mill.feed, mill.speed));
            sb.SetFeed((float)mill.feed);
            sb.SetSpeed((float)mill.speed);
            sb.SetDepth(0.5f);
            sb.MoveTo(new PointF(0, 0));
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generateSuffix(IGeometryOutput sb)
        {
            sb.BeginGroup("generateSuffix");
            sb.SetDepth(0.5f);
            sb.EndGroup();
        }

        private void generateHoleMilling(IGeometryOutput sb)
        {
            sb.BeginGroup("generateHoleMilling");
            float dia = (float)(work.mainMill.diameter * 0.5);
            sb.MoveTo(new PointF(0, dia));
            sb.SetDepth(0);
            double pd = 0;
            bool running = true;
            while (running)
            {
                pd += work.mainMill.passDepth;
                if (pd >= work.dimensions.stockThickness + cutThroughMargin)
                {
                    pd = work.dimensions.stockThickness;
                    running = false;
                }
                sb.SetDepth((float)-pd);
                sb.ArcTo(new PointF(0, dia), new PointF(0, 0), false);
                sb.SetDepth(0);
            }
            sb.SetDepth((float)-(work.dimensions.stockThickness + cutThroughMargin));
            sb.ArcTo(new PointF(0, dia), new PointF(0, 0), false);
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generateRidgeTopMilling(IGeometryOutput sb)
        {
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness -
                work.dimensions.toothThickness - work.dimensions.ridgeThickness;
            if (cDep <= 0)
            {
                return;
            }
            sb.BeginGroup("generateRidgeTopMilling");
            double iRad = work.calc.radius - work.calc.dedendum - work.dimensions.ridgeMargin -
                    work.dimensions.ridgeThickness + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin +
                    work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.SetDepth(0.1f);
            sb.MoveTo(new PointF(0, (float)iRad));
            sb.SetDepth(0);
            for (double d = 0; d < cDep;)
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.SetDepth((float)-d);
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    bool clockwise = true;
                    if (first)
                    {
                        clockwise = false;
                        first = false;
                    }
                    sb.LineTo(new PointF(0, (float)r));
                    sb.ArcTo(new PointF(0, (float)r), new PointF(0, 0), clockwise);
                }
            }
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generateFaceMilling(IGeometryOutput sb)
        {
            sb.BeginGroup("generateFaceMilling");
            sb.Comment("TODO: implement face milling");
            sb.EndGroup();
        }

        private void generateToothTopMilling(IGeometryOutput sb)
        {
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness -
                work.dimensions.toothThickness;
            if (cDep <= 0)
            {
                return;
            }
            sb.BeginGroup("generateToothTopMilling");
            double iRad = work.calc.radius - work.calc.dedendum - work.dimensions.ridgeMargin
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin +
                    work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.SetDepth(0.1f);
            sb.MoveTo(new PointF(0, (float)iRad));
            double rDep = cDep - work.dimensions.ridgeThickness;
            sb.SetDepth((float)-rDep);
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.SetDepth((float)-d);
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    bool clockwise = true;
                    if (first)
                    {
                        clockwise = false;
                        first = false;
                    }
                    sb.LineTo(new PointF(0, (float)r));
                    sb.ArcTo(new PointF(0, (float)r), new PointF(0, 0), clockwise);
                }
            }
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generateToothOutlineMilling(IGeometryOutput sb)
        {
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness;
            if (cDep <= 0)
            {
                sb.Comment("cDep " + cDep.ToString("G") + "");
                return;
            }
            sb.BeginGroup("generateToothOutlineMilling");
            double iRad = work.calc.radius + work.calc.dedendum
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin
                    + work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.SetDepth(0.1f);
            sb.MoveTo(new PointF(0, (float)iRad));
            double rDep = cDep - work.dimensions.toothThickness;
            sb.SetDepth((float)-rDep);
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.SetDepth(-(float)d);
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    bool clockwise = true;
                    if (first)
                    {
                        clockwise = false;
                        first = false;
                    }
                    if (work.dimensions.stockThickness - tabHeight < d
                        && r > oRad - work.mainMill.stepOver)
                    {
                        //  mill with tabs
                        sb.Comment("todo: mill with tabs");
                    }
                    sb.LineTo(new PointF(0, (float)r));
                    sb.ArcTo(new PointF(0, (float)r), new PointF(0, 0), clockwise);
                }
            }
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generatePlateMilling(IGeometryOutput sb)
        {
            if (work.dimensions.plateThickness <= 0)
            {
                return;
            }
            double cDep = work.dimensions.stockThickness;
            if (cDep <= 0)
            {
                return;
            }
            sb.BeginGroup("generatePlateMilling");
            double iRad = work.calc.radius + work.calc.addendum
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin
                    + work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.SetDepth(0.1f);
            sb.MoveTo(new PointF(0, (float)iRad));
            double rDep = cDep - work.dimensions.plateThickness;
            sb.SetDepth((float)-rDep);
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.SetDepth((float)-d);
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    bool clockwise = true;
                    if (first)
                    {
                        clockwise = false;
                        first = false;
                    }
                    if (work.dimensions.stockThickness - tabHeight < d
                        && r > oRad - work.mainMill.stepOver)
                    {
                        //  mill with tabs
                        sb.Comment("todo: mill with tabs");
                    }
                    sb.LineTo(new PointF(0, (float)r));
                    sb.ArcTo(new PointF(0, (float)r), new PointF(0, 0), clockwise);
                }
            }
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        private void generateToothProfileMilling(IGeometryOutput sb, int teeth, MillInfo mill)
        {
            sb.BeginGroup("generateToothProfileMilling");
            Outline nuProfile = GenerateOutline(work.profile, teeth, mill.diameter);
            PointF start = nuProfile.points[0];
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness;
            double rDep = cDep - work.dimensions.toothThickness;
            sb.MoveTo(start);
            sb.SetDepth((float)-rDep);
            PointF curPos = start;
            double endDepth = cDep;
            if (work.dimensions.plateThickness == 0)
            {
                endDepth += cutThroughMargin;
            }
            sb.Comment(String.Format("startDepth {0} endDepth {1} passDepth {2}",
                rDep, endDepth, mill.passDepth));
            for (double d = rDep; d < endDepth; )
            {
                d += mill.passDepth;
                if (d > endDepth)
                {
                    d = endDepth;
                }
                //  plunge slowly
                sb.SetDepth((float)-d);
                foreach (PointF p in nuProfile.points)
                {
                    curPos = LineOrArc(sb, p, curPos);
                }
                LineOrArc(sb, start, curPos);
                curPos = start;
            }
            sb.SetDepth(0);
            sb.SetDepth(0.1f);
            sb.EndGroup();
        }

        PointF LineOrArc(IGeometryOutput sb, PointF p, PointF curPos)
        {
            float sz = Distance2(p, curPos);
            if (sz < 9e-8)
            {
                return curPos;
            }
            float dl = Math.Abs(Length2(p) - Length2(curPos));
            double a1 = Math.Atan2(p.Y, p.X);
            double a2 = Math.Atan2(curPos.Y, curPos.X);
            if (a1 < a2 - Math.PI)
            {
                a1 += 2 * Math.PI;
            }
            else if (a1 > a2 + Math.PI)
            {
                a1 -= 2 * Math.PI;
            }
            bool clockwise = false;
            if (a1 < a2)
            {
                clockwise = true;
            }
            if (dl < 1e-3 && dl < sz * 0.125f && sz > 4e-6)
            {
                sb.ArcTo(p, new PointF(0, 0), clockwise);
            }
            else
            {
                sb.LineTo(p);
            }
            return p;
        }

        private PointF[] MultiplyProfile(PointF[] profile, int num)
        {
            List<PointF> ret = new List<PointF>();
            for (int i = 0; i < num; ++i)
            {
                double inc = Math.PI * 2 * i / num;
                double sin = Math.Sin(inc);
                double cos = Math.Cos(inc);
                foreach (PointF pf in profile)
                {
                    PointF nup = new PointF((float)(pf.X * cos - pf.Y * sin), (float)(pf.Y * cos + pf.X * sin));
                    ret.Add(nup);
                }
            }
            return ret.ToArray<PointF>();
        }

        private Outline GenerateOutline(PointF[] profile, int numTeeth, double diameter)
        {
            PointF[] merged = MultiplyProfile(profile, numTeeth);
            Outline ret = new Outline();
#if !REAL
            List<PointF> pts = new List<PointF>();
            int start = FindInnermostSegment(merged);
            int plusy, minusy, plusx, minusx;
            FindExtremaPoints(merged, out plusy, out minusy, out plusx, out minusx);

            pts.Add(CalculateOutsidePoint(new PointF(0, 0),
                merged[start % merged.Length], merged[(start + 1) % merged.Length], diameter));
            for (int i = 0, n = merged.Length; i != n; ++i)
            {
                int a = (i + start) % n;
                int b = (i + start + 1) % n;
                int c = (i + start + 2) % n;
                pts.Add(NavigateLineSegment(pts[pts.Count - 1], merged[a], merged[b], merged[c], diameter));
            }
            pts.Add(pts[0]);

            ret.points = pts.ToArray<PointF>();
            ret.extrema = new PointF[] {
                merged[plusy],
                merged[minusy],
                merged[plusx],
                merged[minusx]
            };
#else
            ret.points = profile;
#endif
            return ret;
        }

        
        private PointF CalculateOutsidePoint(
            PointF center,
            PointF a,
            PointF b,
            double diameter)
        {
            PointF ret = new PointF((a.X + b.X) * 0.5f - center.X, (a.Y + b.Y) * 0.5f - center.X);
            float len = (float)Math.Sqrt(ret.X * ret.X + ret.Y * ret.Y);
            return new PointF(
                ret.X + (float)(ret.X / len * diameter), 
                ret.Y + (float)(ret.Y / len * diameter));
        }

        private PointF NavigateLineSegment(
            PointF from,
            PointF a,
            PointF b,
            PointF c,
            double diameter)
        {
#if !REAL
            //  I want the point that is "diameter" out from the intersection, and
            //  in line with both the planes through the lines and the Z vector.
            PointF na = Normalize(new PointF(b.Y - a.Y, a.X - b.X));   //  generate normal A by 90 degree rotation
            PointF nb = Normalize(new PointF(c.Y - b.Y, b.X - c.X));   //  generate normal B by 90 degree rotation
            PointF nn = Normalize(new PointF(na.X + nb.X, na.Y + nb.Y));
            float dm = (float)(diameter / Dot(nn, na));
            PointF dst = new PointF(b.X + nn.X * dm, b.Y + nn.Y * dm);
            return dst;
#else
            return b;
#endif
        }

        internal float Dot(PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        internal PointF Normalize(PointF x)
        {
            double ls = x.X * x.X + x.Y * x.Y;
            if (ls < 1e-20)
            {
                throw new InvalidOperationException("Zero-length vector in Normalize()");
            }
            float len = 1.0f / (float)Math.Sqrt(ls);
            x.X *= len;
            x.Y *= len;
            return x;
        }

        internal struct Outline
        {
            //  points, closed loop, for outline milling
            internal PointF[] points;
            //  needed for generating tabs
            internal PointF[] extrema;
        }

        private float Distance2(PointF a, PointF b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        private int FindLongestSegment(PointF[] pts)
        {
            int n = pts.Length;
            PointF pf = pts[n-1];
            float len = Distance2(pf, pts[0]);
            int found = n;
            for (int i = 1; i < n; ++i)
            {
                float l = Distance2(pts[i-1], pts[i]);
                if (l > len)
                {
                    len = l;
                    found = i - 1;
                }
            }
            return found;
        }

        float Length2(PointF p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        private int FindInnermostSegment(PointF[] pts)
        {
            int n = pts.Length;
            PointF pf = pts[n - 1];
            float len = Length2(new PointF(pf.X + pts[0].X, pf.Y + pts[0].Y));
            int found = n;
            for (int i = 1; i < n; ++i)
            {
                float l = Length2(new PointF(pts[i-1].X + pts[i].X, pts[i-1].Y + pts[i].Y));
                if (l < len)
                {
                    len = l;
                    found = i - 1;
                }
            }
            return found;
        }

        private void FindExtremaPoints(PointF[] pts, out int plusy, out int minusy, out int plusx, out int minusx)
        {
            plusy = minusy = plusx = minusx = 0;
            PointF px = pts[0];
            PointF mx = pts[0];
            PointF py = pts[0];
            PointF my = pts[0];
            for (int i = 0, n = pts.Length; i != n; ++i)
            {
                if (pts[i].X > px.X)
                {
                    plusx = i;
                    px = pts[i];
                }
                if (pts[i].X < mx.X)
                {
                    minusx = i;
                    mx = pts[i];
                }
                if (pts[i].Y > py.Y)
                {
                    plusy = i;
                    py = pts[i];
                }
                if (pts[i].Y < my.Y)
                {
                    minusy = i;
                    my = pts[i];
                }
            }
        }
    }

    public static class StringBuilderExtensions
    {
        public static void WriteLine(this StringBuilder sb, string text)
        {
            sb.Append(text);
            sb.Append("\n");
        }

        public static void WriteLine(this StringBuilder sb, string text, params object[] args)
        {
            sb.AppendFormat(text, args);
            sb.AppendFormat("\n");
        }

    }

    public interface IGeometryOutput
    {
        void Comment(string s);
        void BeginGroup(string s);
        void SetDepth(float d);
        void MoveTo(PointF pt);
        void LineTo(PointF pt);
        void ArcTo(PointF pt, PointF origin, bool clockwise);
        void SetFeed(float f);
        void SetSpeed(float s);
        void EndGroup();
        string Finish();
    }

    public class GCodeGeometryOutput : IGeometryOutput
    {
        public GCodeGeometryOutput(StringBuilder stringOut)
        {
            feed = 1;
            sb = stringOut;
            sb.WriteLine("G20");        //  inches
            sb.WriteLine("G90");        //  absolute
            sb.WriteLine("G17");        //  XY plane
            sb.WriteLine("M3");         //  start spindle
        }
        StringBuilder sb;
        PointF curPos;
        float curDepth;
        float feed;

        public string Finish()
        {
            sb.WriteLine("M5");         //  stop spindle
            sb.WriteLine("X0Y0");       //  home position
            sb.WriteLine("M2");         //  end of program
            return sb.ToString();
        }

        public void Comment(string s)
        {
            sb.WriteLine("({0})", s);
        }

        public void BeginGroup(string s)
        {
            sb.WriteLine("({0})", s);
        }

        public void SetDepth(float d)
        {
            //  moving downwards into material means move cautiously!
            float f = (d > curDepth || (d >= 0 && curDepth >= 0)) ? feed : feed * 0.1f;
            sb.WriteLine("G{0}Z{1:0.0000}F{2:G}", (d > 0 && curDepth >= 0) ? 0 : 1, d, f);
            curDepth = d;
            if (f != feed)
            {
                sb.WriteLine("F{0:G}", feed);
            }
        }

        public void MoveTo(PointF pt)
        {
            sb.WriteLine("G0X{0:0.0000}Y{1:0.0000}", pt.X, pt.Y);
            curPos = pt;
        }

        public void LineTo(PointF pt)
        {
            sb.WriteLine("G1X{0:0.0000}Y{1:0.0000}", pt.X, pt.Y);
            curPos = pt;
        }

        public void ArcTo(PointF pt, PointF origin, bool clockwise)
        {
            sb.WriteLine("G{0}X{1:0.0000}Y{2:0.0000}I{3:0.0000}J{4:0.0000}",
                clockwise ? 2 : 3, pt.X, pt.Y, origin.X - curPos.X, origin.Y - curPos.Y);
            curPos = pt;
        }

        public void SetFeed(float f)
        {
            sb.WriteLine("F{0:G}", f);
            feed = f;
        }

        public void SetSpeed(float s)
        {
            sb.WriteLine("S{0:G}", s);
        }

        public void EndGroup()
        {
            //  nothing
        }
    }

    public class SVGGeometryOutput : IGeometryOutput
    {
        public SVGGeometryOutput(StringBuilder stringOut)
        {
            sb = stringOut;
            sb.WriteLine("<?xml version='1.0' encoding='UTF-8' standalone='no'?>");
            sb.WriteLine("<!-- created with Enchanted Gears (http://www.enchantedage.com/gears) -->");
            sb.WriteLine("<svg xmlns:dc='http://purl.org/dc/elements/1.1/'");
            sb.WriteLine(" xmlns:cc='http://creativecommons.org/ns#'");
            sb.WriteLine(" xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'");
            sb.WriteLine(" xmlns:svg='http://www.w3.org/2000/svg'");
            sb.WriteLine(" xmlns='http://www.w3.org/2000/svg'");
            sb.WriteLine(" version='1.1'");
            sb.WriteLine(" width='1000'");
            sb.WriteLine(" height='1000'");
            sb.WriteLine(" id='gears-svg'>");
            sb.WriteLine("<defs id='defs-dummy' />");
            sb.WriteLine("<circle id='center' fill='none' stroke='#808080' cx='500' cy='500' r='10' />");
        }
        StringBuilder sb;
        StringBuilder line;
        bool inGroup;
        string name = "n";
        int nid = 0;
        PointF curPos;

        void Commit()
        {
            if (line != null)
            {
                GenerateLine();
                line = null;
            }
        }

        void GenerateLine()
        {
            sb.WriteLine("{0}", "<path style='fill:none;stroke:#000000;stroke-width:1px' d='" +
                line.ToString() + "' id='" + name + "-" + nid.ToString() + "' />");
            ++nid;
        }
        
        void AddToLine(string s, params PointF[] pts)
        {
            if (line == null)
            {
                line = new StringBuilder();
                PointF cp = Xform(curPos);
                line.AppendFormat("M{0:0.0000} {1:0.0000}", cp.X, cp.Y);
            }
            line.Append(s);
            foreach (PointF p in pts)
            {
                PointF q = Xform(p);
                line.AppendFormat(" {0:0.000} {1:0.000}", q.X, q.Y);
            }
            if (line.Length > 1000)
            {
                Commit();
            }
        }

        PointF Xform(PointF p)
        {
            return new PointF(p.X * 96 + 500, p.Y * 96 + 500);
        }

        float XformScale(float f)
        {
            return f * 96;
        }

        void CalcCubic(PointF from, out PointF ca, out PointF cb, PointF to, PointF center, bool clockwise)
        {
            ca = from;
            cb = to;
            PointF dfrom = from;
            float len = (float)Math.Sqrt(dfrom.X * dfrom.X + dfrom.Y * dfrom.Y);
            dfrom.X /= len; dfrom.Y /= len;
            PointF dto = to;
            len = (float)Math.Sqrt(dto.X * dto.X + dto.Y * dto.Y);
            dto.X /= len; dto.Y /= len;
            if (clockwise)
            {
                float t = dfrom.X;
                dfrom.X = dfrom.Y;
                dfrom.Y = -t;
                t = dto.X;
                dto.X = -dto.Y;
                dto.Y = t;
            }
            else
            {
                float t = dfrom.X;
                dfrom.X = -dfrom.Y;
                dfrom.Y = t;
                t = dto.X;
                dto.X = dto.Y;
                dto.Y = -t;
            }
            len = (float)Math.Sqrt((to.X - from.X) * (to.X - from.X) + (to.Y - from.Y) * (to.Y - from.Y));
            //  a very approximate circle arc...
            ca.X = from.X + dfrom.X * len * 0.28f;
            ca.Y = from.Y + dfrom.Y * len * 0.28f;
            cb.X = to.X + dto.X * len * 0.28f;
            cb.Y = to.Y + dto.Y * len * 0.28f;
        }

        public string Finish()
        {
            Commit();
            sb.WriteLine("</svg>");
            return sb.ToString();
        }

        public void BeginGroup(string s)
        {
            if (inGroup)
            {
                throw new InvalidOperationException("Internal error -- recursive BeginGroup().");
            }
            inGroup = true;
            Commit();
            sb.WriteLine("<g id='" + s + "'>");
        }

        public void MoveTo(PointF pt)
        {
            AddToLine("M", pt);
            curPos = pt;
        }

        public void LineTo(PointF pt)
        {
            AddToLine("L", pt);
            curPos = pt;
        }

        public void ArcTo(PointF pt, PointF origin, bool clockwise)
        {
            if (pt == curPos)
            {
                //  circle!
                Commit();
                PointF o = Xform(origin);
                sb.WriteLine("<circle fill='none' stroke='#000000' stroke-width='1px' cx='{0:0.0000}' cy='{1:0.0000}' r='{2:0.0000}' id='{3}' />",
                    o.X, o.Y, XformScale((float)Math.Sqrt(pt.X*pt.X + pt.Y * pt.Y)), name + "-" + nid.ToString());
                nid = nid + 1;
            }
            else
            {
                //  ignore clockwise, and just draw closest path
                PointF pta, ptb;
                CalcCubic(curPos, out pta, out ptb, pt, origin, clockwise);
                AddToLine("C", pta, ptb, pt);
                curPos = pt;
            }
        }

        public void SetFeed(float f)
        {
        }

        public void SetSpeed(float s)
        {
        }

        public void SetDepth(float d)
        {
            Commit();
            name = "depth" + d.ToString("0.0000");
        }

        public void Comment(string s)
        {
            sb.WriteLine("<!-- " + s + " -->");
        }

        public void EndGroup()
        {
            if (!inGroup)
            {
                throw new InvalidOperationException("Internal error -- calling EndGroup() when not in group.");
            }
            inGroup = false;
            Commit();
            sb.WriteLine("</g>");
        }
    }
}
