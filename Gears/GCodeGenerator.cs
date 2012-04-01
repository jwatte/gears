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
            StringBuilder sb = new StringBuilder();
            generatePrefix(sb, work.mainMill);
            generateHoleMilling(sb);
            generateRidgeTopMilling(sb);
            generateFaceMilling(sb);
            generateToothTopMilling(sb);
            generateToothOutlineMilling(sb);
            if (includeTooth)
            {
                generateToothProfileMilling(sb, work.numTeeth, work.mainMill);
            }
            generateSuffix(sb);
            return sb.ToString();
        }

        public string generateToothMill()
        {
            if (!work.useToothMill)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            generatePrefix(sb, work.toothMill);
            generateToothProfileMilling(sb, work.numTeeth, work.toothMill);
            generateSuffix(sb);
            return sb.ToString();
        }

        private string generatePrefix(StringBuilder sb, MillInfo mill)
        {
            sb.WriteLine("(generatePrefix)");
            sb.WriteLine("(Diameter {0} Cut Depth {1} Pass Depth {2} Feed {3} Speed {4})",
                mill.diameter, mill.cuttingDepth, mill.passDepth, mill.feed, mill.speed);
            sb.WriteLine("G20");        //  inches
            sb.WriteLine("G90");        //  absolute
            sb.WriteLine("G17");        //  XY plane
            sb.WriteLine("F{0:G}", mill.feed);
            sb.WriteLine("S{0:G}", mill.speed);
            sb.WriteLine("G0Z0.5");     //  safe height
            sb.WriteLine("X0Y0");       //  home position
            sb.WriteLine("M3");         //  start spindle
            sb.WriteLine("Z0.1");       //  close to work
            sb.WriteLine("G1");         //  mill speed
            return sb.ToString();
        }

        private string generateSuffix(StringBuilder sb)
        {
            sb.WriteLine("(generateSuffix)");
            sb.WriteLine("G0Z0.5");     //  safe height
            sb.WriteLine("M5");         //  stop spindle
            sb.WriteLine("X0Y0");       //  home position
            sb.WriteLine("M2");         //  end of program
            return sb.ToString();
        }

        private void generateHoleMilling(StringBuilder sb)
        {
            sb.WriteLine("(generateHoleMilling)");
            string dia = (work.mainMill.diameter * 0.5).ToString("0.0000");
            sb.WriteLine("G0Z0.1X0Y" + dia);
            sb.WriteLine("G1Z0");
            double plungeFeed = work.mainMill.feed * 0.1f;
            double helixLength = (work.dimensions.holeDiameter - work.mainMill.diameter) * Math.PI;
            double helixTime = work.mainMill.feed / work.mainMill.feed * helixLength;
            double plungeTime = work.mainMill.feed / plungeFeed;
            double actualFeed = 0;
            if (helixTime >= plungeTime)
            {
                actualFeed = work.mainMill.feed;
            }
            else
            {
                actualFeed = work.mainMill.feed * helixTime / plungeTime;
                if (actualFeed < plungeFeed)
                {
                    actualFeed = plungeFeed;
                }
            }
            sb.WriteLine("F{0:0.0000}", actualFeed);
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
                string pds = pd.ToString("0.0000");
                sb.WriteLine("G3I0J-" + dia + "Z-" + pd);
            }
            sb.WriteLine("G1Z-" + work.dimensions.stockThickness.ToString("0.0000"));
            sb.WriteLine("G3I0J-" + dia);
            sb.WriteLine("F{0:0.0000}", work.mainMill.feed);
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        private void generateRidgeTopMilling(StringBuilder sb)
        {
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness -
                work.dimensions.toothThickness - work.dimensions.ridgeThickness;
            if (cDep <= 0)
            {
                return;
            }
            sb.WriteLine("(generateRidgeTopMilling)");
            double iRad = work.calc.radius - work.calc.dedendum - work.dimensions.ridgeMargin -
                    work.dimensions.ridgeThickness + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin +
                    work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.WriteLine("G0Z0.1");
            sb.WriteLine("G0X0Y" + iRad.ToString("0.0000"));
            sb.WriteLine("G1Z0");
            for (double d = 0; d < cDep;)
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.WriteLine("G1Z-" + d.ToString("0.0000") + "F" + (work.mainMill.feed * 0.1).ToString("0.0000"));
                sb.WriteLine("F" + work.mainMill.feed.ToString("0.0000"));
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    string rDim = r.ToString("0.0000");
                    string mode = "G2";
                    if (first)
                    {
                        mode = "G3";
                        first = false;
                    }
                    sb.WriteLine("G1X0Y" + rDim);
                    sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                }
            }
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        private void generateFaceMilling(StringBuilder sb)
        {
            sb.WriteLine("(generateFaceMilling)");
        }

        private void generateToothTopMilling(StringBuilder sb)
        {
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness -
                work.dimensions.toothThickness;
            if (cDep <= 0)
            {
                return;
            }
            sb.WriteLine("(generateToothTopMilling)");
            double iRad = work.calc.radius - work.calc.dedendum - work.dimensions.ridgeMargin
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin +
                    work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.WriteLine("G0Z0.1");
            sb.WriteLine("G0X0Y" + iRad.ToString("0.0000"));
            double rDep = cDep - work.dimensions.ridgeThickness;
            sb.WriteLine("G1Z-" + rDep.ToString("0.0000"));
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.WriteLine("G1Z-" + d.ToString("0.0000") + "F" + (work.mainMill.feed * 0.1).ToString("0.0000"));
                sb.WriteLine("F" + work.mainMill.feed.ToString("0.0000"));
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    string rDim = r.ToString("0.0000");
                    string mode = "G2";
                    if (first)
                    {
                        mode = "G3";
                        first = false;
                    }
                    sb.WriteLine("G1X0Y" + rDim);
                    sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                }
            }
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        private void generateToothOutlineMilling(StringBuilder sb)
        {
            sb.WriteLine("(generateToothOutlineMilling)");
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness;
            if (cDep <= 0)
            {
                sb.WriteLine("(cDep " + cDep.ToString("G") + ")");
                return;
            }
            double iRad = work.calc.radius + work.calc.dedendum
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin
                    + work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.WriteLine("G0Z0.1");
            sb.WriteLine("G0X0Y" + iRad.ToString("0.0000"));
            double rDep = cDep - work.dimensions.toothThickness;
            sb.WriteLine("G1Z-" + rDep.ToString("0.0000"));
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.WriteLine("G1Z-" + d.ToString("0.0000") + "F" + (work.mainMill.feed * 0.1).ToString("0.0000"));
                sb.WriteLine("F" + work.mainMill.feed.ToString("0.0000"));
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    string rDim = r.ToString("0.0000");
                    string mode = "G2";
                    if (first)
                    {
                        mode = "G3";
                        first = false;
                    }
                    if (work.dimensions.stockThickness - tabHeight < d
                        && r > oRad - work.mainMill.stepOver)
                    {
                        //  mill with tabs
                        sb.WriteLine("(todo: mill with tabs)");
                        sb.WriteLine("G1X0Y" + rDim);
                        sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                    }
                    else
                    {
                        sb.WriteLine("G1X0Y" + rDim);
                        sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                    }
                }
            }
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        private void generatePlateMilling(StringBuilder sb)
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
            sb.WriteLine("(generatePlateMilling)");
            double iRad = work.calc.radius + work.calc.addendum
                    + work.mainMill.diameter * 0.5f;
            double oRad = work.calc.radius + work.calc.addendum + work.dimensions.plateMargin
                    + work.mainMill.diameter * 0.5f;
            if (oRad < iRad)
            {
                oRad = iRad;
            }
            sb.WriteLine("G0Z0.1");
            sb.WriteLine("G0X0Y" + iRad.ToString("0.0000"));
            double rDep = cDep - work.dimensions.plateThickness;
            sb.WriteLine("G1Z-" + rDep.ToString("0.0000"));
            for (double d = rDep; d < cDep; )
            {
                d += work.mainMill.passDepth;
                if (d > cDep)
                {
                    d = cDep;
                }
                //  plunge slowly
                sb.WriteLine("G1Z-" + d.ToString("0.0000") + "F" + (work.mainMill.feed * 0.1).ToString("0.0000"));
                sb.WriteLine("F" + work.mainMill.feed.ToString("0.0000"));
                bool running = true;
                bool first = true;
                for (double r = iRad; running; r += work.mainMill.stepOver)
                {
                    if (r >= oRad)
                    {
                        r = oRad;
                        running = false;
                    }
                    string rDim = r.ToString("0.0000");
                    string mode = "G2";
                    if (first)
                    {
                        mode = "G3";
                        first = false;
                    }
                    if (work.dimensions.stockThickness - tabHeight < d
                        && r > oRad - work.mainMill.stepOver)
                    {
                        //  mill with tabs
                        sb.WriteLine("(todo: mill with tabs)");
                        sb.WriteLine("G1X0Y" + rDim);
                        sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                    }
                    else
                    {
                        sb.WriteLine("G1X0Y" + rDim);
                        sb.WriteLine(mode + "X0Y" + rDim + "I0J-" + rDim);
                    }
                }
            }
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        private void generateToothProfileMilling(StringBuilder sb, int teeth, MillInfo mill)
        {
            sb.WriteLine("(generateToothProfileMilling)");
            Outline nuProfile = GenerateOutline(work.profile, teeth, mill.diameter);
            PointF start = nuProfile.points[0];
            double cDep = work.dimensions.stockThickness - work.dimensions.plateThickness;
            double rDep = cDep - work.dimensions.toothThickness;
            sb.WriteLine("G0X" + start.X.ToString("0.0000") + "Y" + start.Y.ToString("0.0000"));
            sb.WriteLine("G1Z-" + rDep.ToString("0.0000") + "F" + (mill.feed * 0.1).ToString("0.0000"));
            PointF curPos = start;
            double endDepth = cDep;
            if (work.dimensions.plateThickness == 0)
            {
                endDepth += cutThroughMargin;
            }
            sb.WriteLine("(startDepth {0} endDepth {1} passDepth {2})",
                rDep, endDepth, mill.passDepth);
            for (double d = rDep; d < endDepth; )
            {
                d += mill.passDepth;
                if (d > endDepth)
                {
                    d = endDepth;
                }
                //  plunge slowly
                sb.WriteLine("G1Z-" + d.ToString("0.0000") + "F" + (mill.feed * 0.1).ToString("0.0000"));
                sb.WriteLine("F" + mill.feed.ToString("0.0000"));
                foreach (PointF p in nuProfile.points)
                {
                    curPos = LineOrArc(sb, p, curPos);
                }
                LineOrArc(sb, start, curPos);
                curPos = start;
            }
            sb.WriteLine("G1Z0");
            sb.WriteLine("G0Z0.1");
        }

        PointF LineOrArc(StringBuilder sb, PointF p, PointF curPos)
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
            string dir = "G3";
            if (a1 < a2)
            {
                dir = "G2";
            }
            if (dl < 1e-3 && dl < sz * 0.125f && sz > 4e-6)
            {
                sb.WriteLine(dir + "I" + (-curPos.X).ToString("0.0000") + "J" + 
                    (-curPos.Y).ToString("0.0000") + "X" + p.X.ToString("0.0000") + 
                    "Y" + p.Y.ToString("0.0000"));
                sb.WriteLine("G1");
            }
            else
            {
                sb.WriteLine("X" + p.X.ToString("0.0000") + "Y" + p.Y.ToString("0.0000"));
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

}
