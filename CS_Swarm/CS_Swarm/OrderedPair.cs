using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Swarm
{
    class OrderedPair
    {
        private PointF coordinate = new PointF();
        public int identifier; // 0 = border, 1 = frontier, 2 = horizon
        public Point botLoc;

        public OrderedPair(float x, float y, int id, Point bot)
        {
            coordinate = new PointF(x, y);
            identifier = id;
            botLoc = bot;
        }

        public PointF getPoint()
        {
            return coordinate;
        }

        public void setIdentity(int i)
        {
            identifier = i;
        }

        public PointF toCartesian()
        {
            float x = (float)(coordinate.X * Math.Cos(coordinate.Y));
            float y = (float)(coordinate.X * Math.Sin(coordinate.Y));
            return new PointF(x, y);

        }

        public PointF toPolar()
        {
            float theta = (float)Math.Atan((double)coordinate.Y / (double)coordinate.X) * (float)(180 / Math.PI); // Unit Circle Theta (Right = 0 degrees)
            int r = (int)(Math.Pow((Math.Pow(coordinate.X, 2) + Math.Pow(coordinate.Y, 2)), 0.5));
            return new PointF(r, theta);
        }

        public PointF toPlot()
        {
            PointF plotPoint = new PointF();

            plotPoint = this.toCartesian();
            plotPoint.X += botLoc.X;
            plotPoint.Y += botLoc.Y;

            return plotPoint;
        }
    }
}
