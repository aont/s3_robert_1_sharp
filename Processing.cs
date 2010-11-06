using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProcessingEmulator
{
    partial class ProcessingForm
    {


        public class Processing
        {
            internal static void setup(Graphics graphics)
            {
                Processing.graphics = graphics;
                Global.setup();
                processing.timer1.Tick += new EventHandler(timer1_Tick);
            }

            private static void timer1_Tick(object sender, EventArgs e)
            {
                processing.draw(Processing.loop);
            }

            private static void loop(Graphics graphics)
            {
                Processing.graphics = graphics;
                Global.loop();
            }

            #region Math

            protected static double abs(double x)
            {
                return Math.Abs(x);
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            private static Random rnd = new Random();

            protected static double random(double p1, double p2)
            {
                return p1 + rnd.NextDouble() * (p2 - p1);

            }
            protected static int random(int p)
            {
                return rnd.Next(p);
            }
            protected static double random(double d)
            {
                return rnd.NextDouble() * d;
            }

            protected const double PI = Math.PI;
            protected const double TWO_PI = 2 * Math.PI;

            protected static double cos(double theta)
            {
                return Math.Cos(theta);
            }

            protected static double sin(double theta)
            {
                return Math.Sin(theta);
            }

            protected static double atan2(double y, double x)
            {
                return Math.Atan2(y, x);
            }

            protected static double sqrt(double p)
            {
                return Math.Sqrt(p);
            }
            #endregion


            protected static void beginShape()
            {
                path = new List<Point>();
            }

            static List<Point> path;
            protected static void endShape()
            {
                Graphics g = graphics;
                g.DrawLines(new Pen(color), path.ToArray());

                //g.Dispose();
                path = null;
            }

            protected static void vertex(double x, double y)
            {
                path.Add(new Point((int)x, (int)y));
            }

           static bool _Stroke = true;
            protected static void noStroke()
            {
                _Stroke = false;
            }



            protected static void ellipse(double x, double y, double rx, double ry)
            {
                Graphics g = graphics;
                g.FillEllipse(new SolidBrush(FillColor), (float)(x - rx), (float)(y - ry), (float)(rx * 2), (float)(ry * 2));
            }
            static Color FillColor;
            protected static void fill(int gray, int alpha)
            {
                normalizebyte(ref alpha);
                normalizebyte(ref gray);
                FillColor = Color.FromArgb(alpha, gray, gray, gray);
            }
            protected static void fill(double gray, double alpha)
            {
                fill((int)gray, (int)alpha);
            }

            protected static void fill(double gray)
            {
                int igray = (int)gray;
                normalizebyte(ref igray);
                FillColor = Color.FromArgb(0, igray, igray, igray);
            }

            #region Form
            protected static void background(int argb)
            {
                background(Color.FromArgb(argb));
            }
            protected static void background(Color color)
            {

                Graphics g = graphics;
                g.Clear(color);
                //g.Dispose();
            }

            protected static void size(int width, int height)
            {
                Size size = new Size(width, height);
                processing.ClientSize = size;
            }
            protected static bool mousePressed
            {
                get
                {

                    if (MouseButtons == System.Windows.Forms.MouseButtons.Left
                        && processing.ClientRectangle.Contains(processing.PointToClient(MousePosition)))
                    {
                        return true;
                    }

                    return false;
                }
            }

            protected static void point(double x, double y)
            {
                point((int)x, (int)y);
            }

            protected static void point(int x, int y)
            {
                if (x >= 0 && x <= width && y >= 0 && y <= height)
                {
                    Graphics g = graphics;

                    g.FillRectangle(new SolidBrush(color), x, y, 2, 2);

                    //g.Dispose();
                }
            }

            static Color color;
            protected static void stroke(int gray, int alpha)
            {
                normalizebyte(ref gray);
                normalizebyte(ref alpha);
                color = Color.FromArgb(alpha, gray, gray, gray);
            }

            protected static void stroke(int p, double p_2)
            {
                stroke(p, (int)p_2);
            }

            private static void normalizebyte(ref int p)
            {
                if (p < 0)
                {
                    p = 0;
                }
                else if(p>255)
                {
                    p = 255;
                }
            }
            protected static int height
            {
                get
                {
                    return processing.ClientSize.Height;
                }
            }
            protected static int width
            {
                get
                {
                    return processing.ClientSize.Width;
                }
            }
            #endregion

            protected static int @int(double p)
            {
                return (int)p;
            }
            private static Graphics _graphics;
            private static Graphics graphics
            {
                get { return _graphics; }
                set
                {
                    _graphics = value;
                    value.CompositingMode = CompositingMode.SourceOver;
                }
            }

            protected static void smooth()
            {
                _graphics.SmoothingMode = SmoothingMode.Default;
            }
            protected static void line(int x1, int y1, int x2, int y2)
            {
                Graphics g = graphics;
                g.DrawLine(new Pen(color), x1, y1, x2, y2);
                //g.Dispose();
            }

            protected static int mouseX
            {
                get { return processing.PointToClient(MousePosition).X; }
            }
            protected static int mouseY
            {
                get { return processing.PointToClient(MousePosition).Y; }
            }


        }

    }
}
