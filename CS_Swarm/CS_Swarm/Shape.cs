using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Swarm
{
    class Shape
    {
        public ArrayList perimeter = new ArrayList();

        public Shape(ArrayList points) { // PLEASE MAKE SHAPES USING POLAR COORDINATES THEN CALL SHAPE_TO_PLOT AFTER
            foreach (OrderedPair p in points)
            {
                perimeter.Add(p);
            }

            OrderedPair temp;
            foreach (OrderedPair p in perimeter)
            {
                int index = perimeter.IndexOf(p);
                if (((OrderedPair)(perimeter[index])).getPoint().Y == ((OrderedPair)(perimeter[index+1])).getPoint().Y)
                {
                    while (Math.Abs(((OrderedPair)(perimeter[index])).getPoint().X - ((OrderedPair)(perimeter[index + 1])).getPoint().X) != 0)
                    {
                        temp = new OrderedPair(((OrderedPair)(perimeter[index + 1])).getPoint().X - 1, ((OrderedPair)(perimeter[index])).getPoint().Y, 1, ((OrderedPair)(perimeter[index])).botLoc); // Create frontier points in polar
                        perimeter.Insert(index + 1, temp);
                    }
                }
            }
        }

        public void shapeToPlot()
        {
            foreach(OrderedPair p in perimeter)
            {
                int index = perimeter.IndexOf(p);
                perimeter[index] = p.toPlot();
            }
        }

        public static Shape operator+ (Shape a, Shape b) // Adds shapes by removing duplicate points and adding perimeter points
        {
            ArrayList points = new ArrayList();

            a.perimeter.TrimToSize();
            a.shapeToPlot();
            b.perimeter.TrimToSize();
            b.shapeToPlot();

            for (int i = 0; i < a.perimeter.Count || i < b.perimeter.Count; i++)
            {
                if (i > a.perimeter.Count)
                {
                    points.Add(b.perimeter[i]);
                }
                else if (i > b.perimeter.Count)
                {
                    points.Add(a.perimeter[i]);
                }
                else
                {
                    points.Add(a.perimeter[i]);
                    points.Add(b.perimeter[i]);
                }
            }

            int j;

            foreach (OrderedPair p in points)
            {
                j = 0;
                while (j < points.Count) // Remove duplicates
                {
                    if (p.getPoint().X == ((OrderedPair)(points[j])).getPoint().X && p.getPoint().Y == ((OrderedPair)(points[j])).getPoint().Y)
                    {
                        if (p.identifier <= ((OrderedPair)(points[j])).identifier) // Keep the point of lowest identity
                        {
                            points.RemoveAt(j);
                        }
                        else
                        {
                            points.Remove(p);
                        }
                    }
                }
            }

            return new Shape(points);
        }

        // clean() goes through and adds all borders to newPerimeter, if all the borders form a closed object, a new shape is created using newPerimeter and returned
        public Shape clean()
        {
            ArrayList newPerimeter = new ArrayList();
            this.shapeToPlot();

            int numCol = 0, numRow = 0;

            for (int row = 0; row < numRow; row++) // Add all borders to newPerimeter
            {
                for (int col = 0; col < numCol; col++)
                {
                    if (((OrderedPair)(perimeter[col])).getPoint().Y == row && ((OrderedPair)(perimeter[col])).getPoint().X == col && ((OrderedPair)(perimeter[col])).identifier == 0)
                    {
                        newPerimeter.Add(perimeter[col]);
                    }
                }
            }

            foreach (OrderedPair p in newPerimeter) // If the maximum distance between each adjacent border is less than 5 centimeters, declares as closed and returns new shape
                                                    // Else, returns the current shape without editing
            {
                int index = newPerimeter.IndexOf(p);
                if (p.identifier == 0)
                {
                    if (Math.Pow((Math.Pow(((OrderedPair)(newPerimeter[index + 1])).getPoint().X, 2) - Math.Pow(((OrderedPair)(newPerimeter[index])).getPoint().X, 2) + Math.Pow(((OrderedPair)(newPerimeter[index + 1])).getPoint().Y, 2) - Math.Pow(((OrderedPair)(newPerimeter[index])).getPoint().Y, 2)), 0.5) > 4)
                    {
                        return this;
                    }
                }
            }

            foreach (OrderedPair p in newPerimeter) // Remove non-borders
            {
                if (p.identifier != 0)
                {
                    newPerimeter.Remove(p);
                }
            }

            return new Shape(newPerimeter);
        }
    }
}
