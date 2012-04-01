using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


/*
 * According to mathworld.wolfram.com:
 * 
 * For a circle of radius r,
 * 
 * (1) x = r cos t
 *     y = r sin t
 *     
 * the parametric equation of the involute is given by
 *
 * (2) xi = r ( cos t + t sin t )
 *     yi = r ( sin t - t cos t )
 *     
 * Given all the complex table-based and hand-wavy descriptions in mechanics 
 * textbooks I had to plow through to find this, this is surprisingly simple!
 * It appears that the "r" in the above is the base diameter -- which is 
 * pitch circle times cosine pressure angle. This is not specifically related 
 * to dedendum -- for large gears, the base diameter is below the dedendum.
 * 
 */

namespace Gears
{
    public partial class ToothForm : UserControl
    {
        public ToothForm()
        {
            InitializeComponent();
        }

        public void setProfile(GearInfo info, bool isA)
        {
            info_ = info;
            calc_ = GearDrawing.calcGears(info)[isA ? 0 : 1];
            recalc();
            Invalidate();
        }

        GearInfo info_;
        GearCalc calc_;
        PointF[] toothProfile;
        PointF[] toothProfile2;
        float scale;

        private void recalc()
        {
            if (!info_.ok || !calc_.ok)
            {
                return;
            }
            List<PointF> profile = new List<PointF>();
            double coef = 2 * Math.PI / calc_.numTeeth / numInvolutePoints;
            double baseRadius = calc_.baseRadius;
            double pitchRadius = calc_.radius;
            double innerRadius = pitchRadius - calc_.dedendum;
            int size = profile.Count;
            bool running = true;
        again:
            bool inPitch = true;
            bool doPitch = false;
            double lastRad = 0;
            PointF interPitch;
            double pitchRad = 0;
            profile.RemoveRange(size, profile.Count);
            for (int i = 0; running; ++i)
            {
                //  involute construction
                double rho = i * coef * (calc_.numTeeth + 9) / 8;
                double inv = Math.Tan(rho) - rho;
                double irad = baseRadius / Math.Cos(rho);
                if (irad < pitchRadius - calc_.dedendum)
                {
                    continue;
                }
                if (inPitch && irad >= pitchRadius)
                {
                    //  this is the intersection with the pitch
                    inPitch = false;
                    doPitch = true;
                }
                if (irad > pitchRadius + calc_.addendum)
                {
                    if (profile.Count - size < numInvolutePoints * 0.75f)
                    {
                        //  going too fast -- let's look for more points within the profile
                        //  before giving up
                        coef = coef * 0.75f;
                        goto again;
                    }
                    if (profile.Count - size > numInvolutePoints * 1.25f)
                    {
                        //  note: 1.25 is NOT 1 / 0.75 -- this is necessary to make
                        //  sure that the search terminates.
                        coef = coef * 1.25f;
                        goto again;
                    }
                    irad = pitchRadius + calc_.addendum;
                    //  done!
                    running = false;
                }
                PointF p = new PointF((float)(irad * Math.Cos(inv)), (float)(irad * Math.Sin(inv)));
                if (doPitch)
                {
                    pitchRad = inv;
                    doPitch = false;
                    double d = irad - lastRad;
                    double t = irad - pitchRadius;
                    double wt = t / d;
                    interPitch = new PointF(
                        (float)(profile[profile.Count-1].X * wt + p.X * (1 - wt)), 
                        (float)(profile[profile.Count-1].Y * wt + p.Y * (1 - wt)));
                }
                lastRad = irad;
                profile.Add(p);
                /*
                // raw circular involute follows exactly the same trajectory
                toothProfile2[i].X = (float)(baseRadius * (Math.Cos(rho) + rho * Math.Sin(rho)));
                toothProfile2[i].Y = (float)(baseRadius * (Math.Sin(rho) - rho * Math.Cos(rho)));
                 */
            }
            List<PointF> rootProfile = new List<PointF>();
            if (baseRadius > innerRadius)
            {
                /* From some unnamed book someone randomly copied onto the Internet:
                 * 
                double paramU = -(Math.PI / 4 + 0.75 * Math.Tan(info_.pressureAngle * Math.PI / 180) + 0.25 / Math.Cos(info_.pressureAngle * Math.PI / 180));
                double paramV = -0.75;
                double thetaMin = 2 / calc_.numTeeth * (paramU + (paramV + calc_.profileShift) / Math.Tan(info_.pressureAngle * Math.PI / 180));
                double thetaMax = 2 * paramU / calc_.numTeeth;
                for (int i = 0; i <= numFilletPoints; ++i)
                {
                    double theta = (thetaMin * (numFilletPoints - i) + thetaMax * i) / numFilletPoints;
                    double tmp = (paramV + calc_.profileShift) / (2 * paramU - calc_.numTeeth * theta);
                    double paramL = Math.Sqrt(1 + 4 * tmp * tmp);
                    double paramP = 0.25 / paramL + (paramU - calc_.numTeeth * theta);
                    double paramQ = 2 * 0.25 / paramL * tmp + paramV + calc_.numTeeth / 2 + calc_.profileShift;
                    double x = calc_.moduleInch * (paramP * Math.Cos(theta) + paramQ * Math.Sin(theta));
                    double y = calc_.moduleInch * (-paramP * Math.Sin(theta) + paramQ * Math.Cos(theta));
                    profile.Add(new PointF((float)(baseRadius + x), (float)(baseRadius - y)));
                }
                 * 
                 * I discovered that this doesn't precisely mate up with the involute spline, and has 
                 * sign errors, so I would treat it with caution if used.
                 */
                // I'm using a hack, instead. Inscribe a circle with the radius of 1.5 times clearance,
                // hitting the bottom point of the involute (bpi) and the base diameter circle (bdc).
                // Circle center (cc) is always circle radius (rc) up from bdc.
                // length(circle center - bpi) == rc.
                // So, find intersection between circle with center in bpi and radius rc, 
                // and circle with center in origin and radius bdcr+rc.
                // but only do this if there's some real clearance to worry about, else just project
                // down to the bottom and draw the circle.
                if (baseRadius > innerRadius + calc_.moduleInch * 0.05f)
                {
                    PointF bpi = profile[0];
                    double ccx = 0;
                    double ccy = 0;
                    for (double fRad = 0.25f; fRad <= 1.0f; fRad += 0.0675f)
                    {
                        double rc = fRad * calc_.moduleInch;
                        if (rc < (baseRadius - innerRadius) * 0.55f)
                        {
                            continue;
                        }
                        double rcInner = innerRadius + rc;
                        //  I know:
                        //  1. (cc.X - bpi.X)^2 + (cc.Y - bpi.Y)^2 == rc^2
                        //  2. cc.X^2 + cc.Y^2 == (bdcr+rc)^2
                        //  3. bpi.Y == 0
                        //  So,
                        //  4. (cc.X - bpi.X)^2 + cc.Y^2 == rc^2
                        //  5. (cc.X - bpi.X)^2 + ((bdcr + rc)^2 - cc.X^2) == rc^2
                        //  6. cc.X^2 - 2 * cc.X * bpi.X + bpi.X^2 + (bdcr + rc)^2 - cc.X^2 == rc^2
                        //  7. -2 * cc.X * bpi.X + bpi.X^2 + (bdcr + rc)^2 == rc^2
                        //  8. -2 * cc.X * bpi.X == rc^2 - bpi.X^2 - (bdcr + rc)^2
                        //  9. cc.X == (rc^2 - bpi.X^2 - (bdcr + rc)^2) / (-2 * bpi.X)
                        ccx = (rc * rc - bpi.X * bpi.X - rcInner * rcInner) / (-2 * bpi.X);
                        //  10. cc.Y = Sqrt((bdcr + rc))^2 - cc.X^2)
                        double ccxi = rcInner * rcInner - ccx * ccx;
                        if (ccxi > 1e-2 * calc_.moduleInch * calc_.moduleInch)
                        {
                            //  I know I want the negative root
                            ccy = -Math.Sqrt(ccxi);
                            //  now, rotate the base point until it reaches base circle
                            double ddx = bpi.X - ccx;
                            double ddy = bpi.Y - ccy;
                            //  rotate counter-clockwise
                            double rcr = Math.Cos(-Math.PI / numFilletPoints * 0.5f);
                            double rsr = Math.Sin(-Math.PI / numFilletPoints * 0.5f);
                            double pr = baseRadius * baseRadius;
                            while (true)
                            {
                                double ndx = rcr * ddx + rsr * ddy;
                                double ndy = -rsr * ddx + rcr * ddy;
                                double px = ccx + ndx;
                                double py = ccy + ndy;
                                double ls = px * px + py * py;
                                if (ls > pr)
                                {
                                    //  oops -- started moving away again -- reached the bottom point
                                    break;
                                }
                                rootProfile.Add(new PointF((float)px, (float)py));
                                ddx = ndx;
                                ddy = ndy;
                                pr = ls;
                            }
                            // done searching for a fillet radius!
                            break;
                        }
                    }
                }
                if (rootProfile.Count == 0)
                {
                    rootProfile.Add(new PointF((float)innerRadius, 0));
                }
            }
            profile.InsertRange(0, rootProfile);
            //  swap the winding order of this segment
            for (int i = 0, n = rootProfile.Count / 2, c = rootProfile.Count; i != n; ++i)
            {
                PointF a = profile[i];
                profile[i] = profile[c - i - 1];
                profile[c - i - 1] = a;
            }
            //  Now, mirror the profile. Specifically, mirror it around X (Y = -Y), 
            //  then rotate - 2 * pitchRad - pi * 2 / numteeth / 2. Here, I can add 
            //  some small delta to rotate slightly less than 1/2 tooth size, to 
            //  give some room for play/backlash
            float sr = (float)Math.Sin(-2 * pitchRad - Math.PI / calc_.numTeeth);
            float cr = (float)Math.Cos(-2 * pitchRad - Math.PI / calc_.numTeeth);
            for (int i = 0, n = profile.Count; i != n; ++i)
            {
                PointF p = profile[n - i - 1];
                p.Y = -p.Y;
                PointF q = new PointF();
                q.X = cr * p.X + sr * p.Y;
                q.Y = -sr * p.X + cr * p.Y;
                profile.Add(q);
            }
            //
            toothProfile = profile.ToArray<PointF>();
            toothProfile2 = new PointF[toothProfile.Length];
            //  generate profile for second tooth by rotating profile for first tooth
            sr = (float)Math.Sin(Math.PI * 2 / calc_.numTeeth);
            cr = (float)Math.Cos(Math.PI * 2 / calc_.numTeeth);
            for (int i = 0; i < toothProfile2.Length; ++i)
            {
                //  rotate by next tooth
                toothProfile2[i].X = (float)(cr * toothProfile[i].X + sr * toothProfile[i].Y);
                toothProfile2[i].Y = (float)(-sr * toothProfile[i].X + cr * toothProfile[i].Y);
            }
            float mm = 0.0f;
            foreach (PointF pf in toothProfile)
            {
                float a = Math.Abs(pf.X);
                if (a > mm) mm = a;
                a = Math.Abs(pf.Y);
                if (a > mm) mm = a;
            }
            if (Width < Height)
            {
                scale = Width / mm;
            }
            else
            {
                scale = Height / mm;
            }
        }
        
        const int numFilletPoints = 20;
        const int numInvolutePoints = 20;
        
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (!info_.ok || !calc_.ok)
                {
                    throw new InvalidOperationException("Don't have valid gear data.");
                }
                base.OnPaint(e);
                Graphics g = e.Graphics;
                PointF center = new PointF(Width / 2, Height / 2);
                float baseRadius = (float)calc_.baseRadius;
                float radius = (float)calc_.radius;
                float dRadius = radius - (float)calc_.dedendum;
                float aRadius = radius + (float)calc_.addendum;
                g.DrawArc(Pens.Yellow, new RectangleF((float)(baseRadius * 0.5 + baseRadius) * -scale, center.Y - baseRadius * scale,
                    baseRadius * 2 * scale, baseRadius * 2 * scale), -30, 60);
                g.DrawArc(Pens.Black, new RectangleF((float)(baseRadius * 0.5 + radius) * -scale, center.Y - radius * scale,
                    radius * 2 * scale, radius * 2 * scale), -30, 60);
                g.DrawArc(Pens.Gray, new RectangleF((float)(baseRadius * 0.5 + dRadius) * -scale, center.Y - dRadius * scale,
                    dRadius * 2 * scale, dRadius * 2 * scale), -30, 60);
                g.DrawArc(Pens.Gray, new RectangleF((float)(baseRadius * 0.5 + aRadius) * -scale, center.Y - aRadius * scale,
                    aRadius * 2 * scale, aRadius * 2 * scale), -30, 60);
                for (int i = 0; i < toothProfile.Length - 1; ++i)
                {
                    g.DrawLine(Pens.Black, new PointF((toothProfile[i].X - baseRadius * 0.5f) * scale, center.Y + toothProfile[i].Y * scale),
                        new PointF((toothProfile[i + 1].X - baseRadius * 0.5f) * scale, center.Y + toothProfile[i + 1].Y * scale));
                    g.DrawLine(Pens.Red, new PointF((toothProfile2[i].X - baseRadius * 0.5f) * scale, center.Y + toothProfile2[i].Y * scale),
                        new PointF((toothProfile2[i + 1].X - baseRadius * 0.5f) * scale, center.Y + toothProfile2[i + 1].Y * scale));
                }
            }
            catch (Exception x)
            {
                GC.KeepAlive(x);
                e.Graphics.DrawString("Cannot show tooth profile",
                    System.Drawing.SystemFonts.CaptionFont, Brushes.Red, new PointF(20, 30));
                e.Graphics.DrawString(x.Message,
                    System.Drawing.SystemFonts.CaptionFont, Brushes.Red, new PointF(20, 50));
            }
        }

        public bool IsValid { get { return info_.ok && calc_.ok; } }
        public GearInfo Info { get { return info_; } }
        public GearCalc Calc { get { return calc_; } }
        public PointF[] Profile { get { return toothProfile; } }
    }
}
