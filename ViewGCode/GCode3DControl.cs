using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using Color4 = OpenTK.Graphics.Color4;

namespace ViewGCode
{
    class GCode3DControl : GLControl
    {
        bool loaded = false;
        System.Timers.Timer timer;
        float rx = 0;
        float ry = 0;
        int hiliteLineStart = -1;
        int hiliteLineEnd = -1;
        bool pending = false;
        float percent = 0;
        float zoom = 1;
        int zoompos = 0;
        float mx = 0;
        float my = 0;

        public GCode3DControl()
        {
            timer = new System.Timers.Timer(1);
            timer.Enabled = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (this)
            {
                if (lines == null)
                {
                    return;
                }
                if (curGenLine < lines.Length)
                {
                    percent = (float)curGenLine / (float)lines.Length;
                    pending = true;
                    int oldLine = curGenLine;
                    curGenLine++;
                    ParseLine(lines[oldLine]);
                }
                else
                {
                    percent = 100;
                    scene = parsing.ToArray<LineVertex>();
                    timer.Enabled = false;
                    pending = false;
                }
                Invalidate();
            }
        }

        Vector3[] colors = {
            new Vector3(0, 1, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 1),
            new Vector3(0.5f, 0.5f, 0.5f)
        };

        void ParseLine(string s)
        {
            Registers newRegs = registers;
            Console.WriteLine("Parsing: {0}", s);
            if (ParseLine(s, ref newRegs))
            {
                if (newRegs.relative)
                {
                    newRegs.X += registers.X;
                    newRegs.Y += registers.Y;
                    newRegs.Z += registers.Z;
                }
                Console.WriteLine("{0} {1} {2} -> {3} {4} {5}", registers.X, registers.Y, registers.Z, newRegs.X, newRegs.Y, newRegs.Z);
                //  do movement
                LineVertex lv = new LineVertex();
                lv.pos1.X = registers.X;
                lv.pos1.Y = registers.Y;
                lv.pos1.Z = registers.Z;
                lv.pos2.X = newRegs.X;
                lv.pos2.Y = newRegs.Y;
                lv.pos2.Z = newRegs.Z;
                lv.line = curGenLine - 1;
                switch (newRegs.G)
                {
                    case 0:     //  rapid move
                        lv.color1 = colors[0];
                        break;
                    case 1:     //  cut move
                        lv.color1 = colors[1];
                        break;
                    case 2:     //  clockwise arc
                        Circle(-1, colors[2], registers.X, registers.Y, registers.Z, newRegs.I, newRegs.J, newRegs.K,
                            newRegs.X, newRegs.Y, newRegs.Z);
                        //  origin
                        lv.color1 = colors[4];
                        lv.pos1.X = registers.X + newRegs.I;
                        lv.pos1.Y = registers.Y + newRegs.J;
                        lv.pos1.Z = registers.Z + newRegs.K;
                        break;
                    case 3:     //  counterclockwise arc
                        Circle(1, colors[3], registers.X, registers.Y, registers.Z, newRegs.I, newRegs.J, newRegs.K,
                            newRegs.X, newRegs.Y, newRegs.Z);
                        //  origin
                        lv.color1 = colors[4];
                        lv.pos2.X = registers.X + newRegs.I;
                        lv.pos2.Y = registers.Y + newRegs.J;
                        lv.pos2.Z = registers.Z + newRegs.K;
                        break;
                    default:
                        lv.color1 = new Vector3(1, 0, 0);
                        Console.WriteLine("Got movement command with unknown G mode {0}", newRegs.G);
                        break;
                }
                parsing.Add(lv);
            }
            Console.WriteLine("Did {0}", s);
            registers = newRegs;
            registers.I = 0;
            registers.J = 0;
            registers.K = 0;
        }

        void Circle(float stepScale, Vector3 color, float fx, float fy, float fz, float dx, float dy, float dz,
            float tx, float ty, float tz)
        {
            //  I don't pay attention to dz
            double a1 = Math.Atan2(-dy, -dx);
            double a2 = Math.Atan2(ty - (fy + dy), tx - (fx + dx));
            if (stepScale > 0 && a2 < a1 + 1e-5)
            {
                a2 += Math.PI * 2;
            }
            if (stepScale < 0 && a2 > a1 - 1e-5)
            {
                a1 += Math.PI * 2;
            }
            double len = Math.Sqrt(dx*dx + dy*dy);
            int nSteps = (int)Math.Floor(len * 40 * (a2 - a1) * stepScale);
            if (nSteps < 8)
            {
                nSteps = 8;
            }
            LineVertex lv = new LineVertex();
            lv.pos2.X = fx;
            lv.pos2.Y = fy;
            lv.pos2.Z = fz;
            lv.color1 = color;
            lv.line = curGenLine - 1;
            for (int i = 1; i <= nSteps; ++i)
            {
                lv.pos1 = lv.pos2;
                lv.pos2 = new Vector3(
                    (float)((fx+dx) + len * Math.Cos(a1 + (a2 - a1) * i / nSteps)),
                    (float)((fy+dy) + len * Math.Sin(a1 + (a2 - a1) * i / nSteps)),
                    (float)(fz + (tz - fz) * (float)i / (float)nSteps));
                parsing.Add(lv);
            }
        }

        string[] pCmds = new string[100];

        //  returns true if a movement command -- X, Y, Z or G are set
        bool ParseLine(string s, ref Registers regs)
        {
            int nCmds = 0;
            bool isDig = false;
            int start = 0;
            int i = 0;
            for (int n = s.Length; i != n; ++i)
            {
                char ch = s[i];
                if (ch == ';' || ch == '(')
                {
                    break;
                }
                bool dig = ((ch >= '0' && ch <= '9') || (ch == '.'));
                if (dig != isDig)
                {
                    if (isDig)
                    {
                        pCmds[nCmds++] = s.Substring(start, i - start);
                        start = i;
                    }
                    isDig = dig;
                }
            }
            if (isDig)
            {
                pCmds[nCmds++] = s.Substring(start, i - start);
            }
            bool move = false;
            for (int j = 0; j != nCmds; ++j)
            {
                string q = pCmds[j];
                move = ParseCmd(q, ref regs) || move;
            }
            return move;
        }

        static bool IsSpace(char ch)
        {
            return ch == (char)9 || ch == (char)10 || ch == (char)13 || ch == (char)32;
        }

        bool ParseCmd(string s, ref Registers regs)
        {
            int charPos = s.Length;
            char theChar = (char)0;
            for (int i = 0, n = s.Length; i != n; ++i)
            {
                char ch = s[i];
                if (!IsSpace(ch))
                {
                    charPos = i;
                    theChar = ch;
                    break;
                }
            }
            if (theChar == (char)0)
            {
                return false;
            }
            string ss = s.Substring(charPos + 1);
            switch (theChar)
            {
                case 'X':
                    ConvertReg(theChar, ss, ref regs.X);
                    return true;
                case 'Y':
                    ConvertReg(theChar, ss, ref regs.Y);
                    return true;
                case 'Z':
                    ConvertReg(theChar, ss, ref regs.Z);
                    return true;
                case 'G':
                    return ConvertG(ss, ref regs);
                case 'M':
                    return ConvertM(ss, ref regs);
                case 'T':
                    ConvertReg(theChar, ss, ref regs.T);
                    return false;
                case 'F':
                    ConvertReg(theChar, ss, ref regs.F);
                    return false;
                case 'S':
                    ConvertReg(theChar, ss, ref regs.S);
                    return false;
                case 'I':
                    ConvertReg(theChar, ss, ref regs.I);
                    return false;
                case 'J':
                    ConvertReg(theChar, ss, ref regs.J);
                    return false;
                case 'K':
                    ConvertReg(theChar, ss, ref regs.K);
                    return false;
                case 'P':
                    ConvertReg(theChar, ss, ref regs.P);
                    return false;
                default:
                    Console.WriteLine("Skipping unknown command '{0}'", s);
                    return false;
            }
        }

        bool ConvertG(string s, ref Registers regs)
        {
            ConvertReg('G', s, ref regs.G);
            switch (regs.G)
            {
            case 0:
                return true;
            case 1:
                return true;
            case 2:
                return true;
            case 3:
                return true;
            case 4:
                //  dwell
                return false;
            case 17:
                //  XY plane
                return false;
            case 20:
                //  inches
                return false;
            case 21:
                //  millimeters
                return false;
            case 90:
                regs.relative = false;
                return false;
            case 91:
                regs.relative = true;
                return false;
            default:
                Console.WriteLine("Unknown G command '{0}'", regs.G);
                return false;
            }
        }

        bool ConvertM(string s, ref Registers regs)
        {
            int m = 0;
            ConvertReg('G', s, ref m);
            switch (m)
            {
                case 0:
                    //  compulsory stop
                    Console.WriteLine("Compulsory stop -- continuing");
                    return false;
                case 1:
                    //  optional stop
                    Console.WriteLine("Optional stop -- continuing");
                    return false;
                case 2:
                    //  program stop
                    Console.WriteLine("Program stop -- ending");
                    curGenLine = lines.Length;
                    return false;
                case 3:
                    //  spindle clockwise
                    regs.spindle = true;
                    regs.ccw = false;
                    return false;
                case 4:
                    //  spindle counterclockwise
                    regs.spindle = true;
                    regs.ccw = true;
                    return false;
                case 5:
                    regs.spindle = false;
                    return false;
                case 7:
                    regs.coolant = false;
                    regs.air = true;
                    return false;
                case 8:
                    regs.coolant = true;
                    regs.air = false;
                    return false;
                case 9:
                    regs.coolant = false;
                    regs.air = false;
                    return false;
                case 13:
                    regs.coolant = true;
                    regs.air = false;
                    regs.spindle = true;
                    regs.ccw = false;
                    return false;
                case 30:
                    Console.WriteLine("End of program -- return to top");
                    curGenLine = lines.Length;
                    return false;
                default:
                    Console.WriteLine("Unknown M code {0}", m);
                    return false;
            }
        }

        void ConvertReg(char name, string s, ref float f)
        {
            for (int i = 0, n = s.Length; i != n; ++i)
            {
                if (!IsSpace(s[i]))
                {
                    f = float.Parse(i == 0 ? s : s.Substring(i));
                    return;
                }
            }
            throw new InvalidOperationException(String.Format("Missing parameter value for {0}", name));
        }

        void ConvertReg(char name, string s, ref int o)
        {
            for (int i = 0, n = s.Length; i != n; ++i)
            {
                if (!IsSpace(s[i]))
                {
                    float f = float.Parse(i == 0 ? s : s.Substring(i));
                    o = (int)Math.Floor(f);
                    return;
                }
            }
            throw new InvalidOperationException(String.Format("Missing parameter value for {0}", name));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loaded = true;
            Invalidate();
        }

        public void Blink()
        {
            if (hiliteLineEnd >= 0)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!loaded || DesignMode)
            {
                e.Graphics.FillRectangle(System.Drawing.Brushes.Gray, new System.Drawing.Rectangle(0, 0, Width, Height));
                return;
            }
            GL.Viewport(0, 0, Width, Height);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ClearColor(new Color4(0, 0, 0.5f, 0));
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.StencilBufferBit |
                ClearBufferMask.DepthBufferBit);
            if (pending)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, 1, 0, 1, 0, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Scale(1.0f, 1.0f, -1.0f);
                GL.Begin(BeginMode.TriangleFan);
                GL.Color4(0.5f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.1f, 0.5f, 0.5f);
                GL.Vertex3(0.1f + 0.8f * percent, 0.5f, 0.5f);
                GL.Vertex3(0.1f + 0.8f * percent, 0.6f, 0.5f);
                GL.Vertex3(0.1f, 0.6f, 0.5f);
                GL.End();
                GL.Begin(BeginMode.LineLoop);
                GL.Color4(0.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.1f, 0.5f, 0.5f);
                GL.Vertex3(0.9f, 0.5f, 0.5f);
                GL.Vertex3(0.9f, 0.6f, 0.5f);
                GL.Vertex3(0.1f, 0.6f, 0.5f);
                GL.End();
            }
            else
            {
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 m;
                Matrix4.CreatePerspectiveFieldOfView(0.75f, (float)Width / (float)Height, 0.5f, 1000.0f, out m);
                GL.LoadMatrix(ref m);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Translate(-mx * zoom, -my * zoom, -5.0f * zoom);
                GL.Rotate(ry * 180 / Math.PI, 0, 1, 0);
                GL.Rotate(rx * 180 / Math.PI, 1, 0, 0);
                GL.Rotate(-90, 1, 0, 0); // compensate for Y up
                GL.Begin(BeginMode.Lines);
                if (scene != null)
                {
                    foreach (LineVertex lv in scene)
                    {
                        if (lv.line >= hiliteLineStart && lv.line <= hiliteLineEnd)
                        {
                            double secs = System.Diagnostics.Stopwatch.GetTimestamp() /
                                (double)System.Diagnostics.Stopwatch.Frequency;
                            bool blink = (((int)(secs * 6)) & 1) != 0;
                            if (blink)
                            {
                                GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
                            }
                            else
                            {
                                GL.Color4(0.0f, 0.0f, 0.0f, 1.0f);
                            }
                        }
                        else
                        {
                            GL.Color3(lv.color1);
                        }
                        GL.Vertex3(lv.pos1.X, lv.pos1.Y, lv.pos1.Z);
                        GL.Vertex3(lv.pos2.X, lv.pos2.Y, lv.pos2.Z);
                    }
                }
                GL.End();
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Begin(BeginMode.TriangleFan);
                GL.Color4(0.5f, 0.5f, 0.5f, 0.5f);
                GL.Vertex3(new Vector3(-2, -2, 0));
                GL.Vertex3(new Vector3(-2, 2, 0));
                GL.Vertex3(new Vector3(2, 2, 0));
                GL.Vertex3(new Vector3(2, -2, 0));
                GL.End();
                GL.Disable(EnableCap.Blend);
            }
            SwapBuffers();
        }

        internal struct LineVertex
        {
            internal Vector3 pos1;
            internal Vector3 pos2;
            internal Vector3 color1;
            internal int line;
        };
        List<LineVertex> parsing = new List<LineVertex>();
        LineVertex[] scene;
        string[] lines;
        int curGenLine;
        Registers registers;

        public void ClearScene()
        {
            scene = null;
            parsing.Clear();
        }

        public void StartGeneratingScene()
        {
            ClearScene();
            percent = 0;
            pending = true;
            curGenLine = 0;
            timer.Enabled = true;
            registers = new Registers();
        }

        public string[] Lines
        {
            get
            {
                return lines;
            }
            set
            {
                lines = value;
                StartGeneratingScene();
            }
        }

        public void MouseDelta(int dx, int dy)
        {
            rx += dy / 200.0f;
            if (rx > Math.PI) rx -= 2 * (float)Math.PI;
            if (rx < Math.PI) rx += 2 * (float)Math.PI;
            ry += dx / 200.0f;
            if (ry > Math.PI) ry -= 2 * (float)Math.PI;
            if (ry < Math.PI) ry += 2 * (float)Math.PI;
            Invalidate();
        }

        internal void UnhiliteLines()
        {
            hiliteLineStart = hiliteLineEnd = -1;
            Invalidate();
        }

        internal void HiliteLines(int start, int end)
        {
            hiliteLineStart = start;
            hiliteLineEnd = end;
            Invalidate();
        }

        internal void ZoomDelta(int dz)
        {
            zoompos += dz;
            if (zoompos < -500)
            {
                zoompos = -500;
            }
            else if (zoompos > 500)
            {
                zoompos = 500;
            }
            zoom = (float)Math.Pow(0.99f, zoompos);
            Invalidate();
        }

        internal void TruckDelta(int dx, int dy)
        {
            mx += dx / 100.0f;
            my += dy / 100.0f;
            if (mx > 10) mx = 10;
            if (mx < -10) mx = -10;
            if (my > 10) my = 10;
            if (my < -10) my = -10;
            Invalidate();
        }
    }

    struct Registers
    {
        public bool relative;
        public bool spindle;
        public bool ccw;
        public bool coolant;
        public bool air;

        public float X, Y, Z;
        public float I, J, K;
        public float F;
        public float S;
        public int P;
        public int G;
        public int T;
    }
}
