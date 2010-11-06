using System;
using System.Collections.Generic;
using System.Text;
using Processing = ProcessingEmulator.ProcessingForm.Processing;
using System.Timers;
using System.Drawing;
/* 
   
   A surface filled with one hundred medium to small sized circles. 
   Each circle has a different size and direction, but moves at the same slow rate. 
   Display: 
   >>> A. The instantaneous intersections of the circles 
   B. The aggregate intersections of the circles 
 
   Implemented by Robert Hodgin <http://flight404.com> 
   6 April 2004 
   Processing v.68 <http://processing.org> 
 
*/
namespace ProcessingEmulator
{
    class Global : Processing
    {


        // ******************************************************************************** 
        // INITIALIZE VARIABLES 
        // ******************************************************************************** 

        static int xStage { get { return width; } }            // x dimension of applet 
        static int yStage { get { return height; } }             // y dimension of applet 

        static int xMid { get { return width / 2; } }        // x midpoint of applet 
        static int yMid { get { return height / 2; } }        // y midpoint of applet 

        static int totalCircles;            // total number of circles 
        static Circle[] circle;                      // Circle object array 


        static double gravity;                      // Strength of gravitational pull 
        //static double xGrav;                         // x point of center of gravity 
        //static double yGrav;                          // y point of center of gravity 
        //static double xGravOffset;

        static double angleOffset;
        static int initRadius;
        static int maxDistance;

        static Color bgColor;

        // ******************************************************************************** 
        // SETUP FUNCTION 
        // ******************************************************************************** 


        internal static void setup()
        {
            bgColor = Color.FromArgb(255, 255, 255);
            size(600, 600);
            background(bgColor);
            smooth();
            //colorMode(RGB, 255);
            //ellipseMode(CENTER_RADIUS);
            //noStroke();
            //framerate(30);
            createCircles();
        }






        // ******************************************************************************** 
        // MAIN LOOP FUNCTION 
        // ******************************************************************************** 

        internal static void loop()
        {
            if (mousePressed)
            {
                createCircles();
            }
            background(bgColor);
            tellCirclesToBehave();
        }



        static void createCircles()
        {
            gravity = .075;
            int width = Processing.width;
            int height = Processing.height;
            initRadius = maxDistance = (width + height) / 8;
            angleOffset = random(360);
            totalCircles = 3 * (int)Math.Sqrt(width + height);
            circle = new Circle[totalCircles];
            for (int i = 0; i < totalCircles; i++)
            {
                double initAngle = i * 360 / totalCircles + angleOffset + random(10);
                double initTheta = (-((initAngle) * PI)) / 180;

                double initxv = cos(initTheta) * initRadius;
                double inityv = sin(initTheta) * initRadius;
                double xPos = xMid + initxv;
                double yPos = yMid + inityv;
                circle[i] = new Circle(xPos, yPos, 0, 0, i);
            }
        }

        static void tellCirclesToBehave()
        {
            for (int i = 0; i < totalCircles; i++)
            {
                circle[i].behave();
            }
        }

        class Circle
        {
            int index;                    // Circle global ID 

            double x;                      // Circle x position 
            double y;                      // Circle y position 
            double r;                      // Circle radius 
            //double rBase;                  // Circle original radius 

            //double angle;                  // Angle of movement in degrees 
            //double theta;                  // Angle of movement in radians 
            //double speed;                  // Speed of movement 

            double xv;                     // Current velocity along x axis 
            double yv;                     // Current velocity along y axis 

            bool[] mightCollide;       // Collision might happen 
            bool[] hasCollided;        // Collision is happening 

            double[] distances;            // Storage for the distance between circles 
            double[] angles;               // Storage for the angle between two connected circles 
            double[] thetas;               // Storage for the radians between two connected circles 
            int numCollisions;            // Number of collisions in one frame 
            int numConnections;           // Total number of collisions 

            double xd;                     // Distance to target along x axis 
            double yd;                     // Distance to target along y axis 
            //double d;                      // Distance to target 

            double alphaVar;               // Late addition variable for alpha modification 

            //double cAngle;                 // Angle of collision in degrees 
            //double cTheta;                 // Angle of collision in radians 
            double cxv;                    // Collision velocity along x axis 
            double cyv;                    // Collision velocity along y axis 

            double gAngle;                 // Angle to gravity center in degrees 
            double gTheta;                 // Angle to gravity center in radians 
            double gxv;                    // Gravity velocity along x axis 
            double gyv;                    // Gravity velocity along y axis 

            internal Circle(double xSent, double ySent, double xvSent, double yvSent, int indexSent)
            {
                x = xSent;
                y = ySent;
                r = 2;

                index = indexSent;

                xv = xvSent;
                yv = yvSent;

                alphaVar = random(35);

                mightCollide = new bool[totalCircles];
                hasCollided = new bool[totalCircles];
                distances = new double[totalCircles];
                angles = new double[totalCircles];
                thetas = new double[totalCircles];
            }

            internal void behave()
            {
                move();
                areWeClose();
                areWeColliding();
                areWeConnected();
                applyGravity();
                //move(); 
                render();
                reset();
            }

            void areWeClose()
            {
                for (int i = 0; i < totalCircles; i++)
                {
                    if (i != index)
                    {
                        if (abs(x - circle[i].x) < 50 && abs(y - circle[i].y) < 50)
                        {
                            mightCollide[i] = true;
                        }
                        else
                        {
                            mightCollide[i] = false;
                        }
                    }
                }
            }

            void areWeColliding()
            {
                for (int i = 0; i < totalCircles; i++)
                {
                    if (mightCollide[i] && i != index)
                    {
                        distances[i] = findDistance(x, y, circle[i].x, circle[i].y);
                        if (distances[i] < (r + circle[i].r) * 1.1)
                        {
                            hasCollided[i] = true;
                            circle[i].hasCollided[index] = true;

                            angles[i] = findAngle(x, y, circle[i].x, circle[i].y);
                            thetas[i] = (-(angles[i] * PI)) / 180.0;
                            cxv += cos(thetas[i]) * ((circle[i].r + r) / 2.0);
                            cyv += sin(thetas[i]) * ((circle[i].r + r) / 2.0);
                            numCollisions += 1;
                        }
                    }
                }

                if (numCollisions > 0)
                {
                    xv = -cxv / numCollisions;
                    yv = -cyv / numCollisions;
                }

                cxv = 0.0;
                cyv = 0.0;

            }

            void areWeConnected()
            {
                for (int i = 0; i < totalCircles; i++)
                {
                    if (hasCollided[i] && i != index)
                    {
                        distances[i] = findDistance(x, y, circle[i].x, circle[i].y);
                        if (distances[i] < maxDistance)
                        {
                            angles[i] = findAngle(x, y, circle[i].x, circle[i].y);
                            thetas[i] = (-(angles[i] * PI)) / 180.0;
                            cxv += cos(thetas[i]) * (circle[i].r / 8.0);
                            cyv += sin(thetas[i]) * (circle[i].r / 8.0);
                            numConnections += 1;
                        }
                        else
                        {
                            hasCollided[i] = false;
                            circle[i].hasCollided[index] = false;
                        }
                    }
                }
                if (numConnections > 0)
                {
                    xv += (cxv / numConnections) / 4.0;
                    yv += (cyv / numConnections) / 4.0;
                }

                cxv = 0.0;
                cyv = 0.0;

                r = numConnections * .85 + 2;
            }


            void applyGravity()
            {
                gAngle = findAngle(x, y, xMid, yMid);
                gTheta = (-(gAngle * PI)) / 180;
                gxv = cos(gTheta) * gravity;
                gyv = sin(gTheta) * gravity;
                xv += gxv;
                yv += gyv;
            }

            void move()
            {
                x += xv;
                y += yv;
            }

            void render()
            {

                //noStroke();
                fill(0, 25);
                ellipse(x, y, r, r);
                fill(0 + r * 10, 50);
                ellipse(x, y, r * .5, r * .5);
                fill(0 + r * 10);
                ellipse(x, y, r * .3, r * .3);

                if (numCollisions > 0)
                {
                    noStroke();
                    fill(0, 25);
                    ellipse(x, y, r, r);

                    fill(0, 55);
                    ellipse(x, y, r * .85, r * .85);
                    fill(0);
                    ellipse(x, y, r * .7, r * .7);
                }

                for (int i = 0; i < totalCircles; i++)
                {
                    if (hasCollided[i] && i < index)
                    {
                        beginShape();
                        xd = x - circle[i].x;
                        yd = y - circle[i].y;
                        stroke(0, 150 - distances[i] * 2.0);
                        vertex(x, y);
                        vertex(x - xd * .25 + random(-1.0, 1.0), y - yd * .25 + random(-1.0, 1.0));
                        vertex(x - xd * .5 + random(-3.0, 3.0), y - yd * .5 + random(-3.0, 3.0));
                        vertex(x - xd * .75 + random(-1.0, 1.0), y - yd * .75 + random(-1.0, 1.0));
                        vertex(circle[i].x, circle[i].y);
                        endShape();
                        //line (x, y, circle[i].x, circle[i].y); 
                    }
                }
                noStroke();
            }


            void reset()
            {
                numCollisions = 0;
                numConnections = 0;
            }
        }

        static double findDistance(double x1, double y1, double x2, double y2)
        {
            double xd = x1 - x2;
            double yd = y1 - y2;
            double td = sqrt(xd * xd + yd * yd);
            return td;
        }



        static double findAngle(double x1, double y1, double x2, double y2)
        {
            double xd = x1 - x2;
            double yd = y1 - y2;

            double t = atan2(yd, xd);
            double a = (180 + (-(180 * t) / PI));
            return a;
        }



    }
}
