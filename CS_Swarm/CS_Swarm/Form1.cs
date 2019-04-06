using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS_Swarm
{
    public partial class Form1 : Form
    {
        const int MAX = 500; // Maximum read distance of the sensors
        ArrayList rawData = new ArrayList();
        ArrayList frontiers = new ArrayList();
        ArrayList shapes = new ArrayList();

        PointF botLocation = new PointF(200, 200); // Value is dependent on Ian's Algorithm

        public Form1()
        {
            InitializeComponent();
        }

        // This method will take the MQTT data stream and add the points as ordered pairs into the raw data array list
        private void InputStream( /* MQTT DATA STREAM r , theta , and botLocation */)
        {
            /*while ( MQTT STREAM IS NOT TERMINATED )
            {
                int identifier;
                if (r == MAX) {
                    identifier = 1;
                }
                else {
                    OrderedPair horizon = new OrderedPair( MAX ,  MQTT STREAM theta , 2 , botLocation);
                    rawData.Add(horizon);
                }
                OrderedPair inputs = new OrderedPair( MQTT STREAM r ,  MQTT STREAM theta , 0 , botLocation);
                rawData.Add(inputs);
            }*/
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mapButton_Click(object sender, PaintEventArgs e)
        {
            InputStream();
            ArrayList borders = new ArrayList();
            foreach (OrderedPair p in rawData)
            {
                int index = rawData.IndexOf(p);

                if (p.identifier == 1)
                {
                    frontiers.Add(p);
                }
                else if (p.identifier == 0)
                {
                    borders.Add(p);
                }
            }

            // Create the shapes and combine if there are common points
            int i = rawData.IndexOf(borders[1]);
            ArrayList perimeter = new ArrayList();
            
            foreach (OrderedPair q in borders)
            {
                perimeter.Add(q);
                perimeter.Add(rawData[i - 1]);
            }

            shapes.Add(new Shape(perimeter));

            foreach(Shape s in shapes)
            {
                foreach (OrderedPair x in s.perimeter)
                {
                    if (x.identifier == 1 && !frontiers.Contains(x))
                    {
                        frontiers.Add(x);
                    }
                }
            }

            // Actually do the thing
            Pen pen = new Pen(Color.DarkSlateGray, 2);
            foreach (Shape r in shapes)
            {
                PointF[] per = new PointF[r.perimeter.Count - 1];
                for (int j = 0; j < r.perimeter.Count; j++)
                {
                    per[j] = ((OrderedPair)(r.perimeter[j])).toPlot();
                }
                e.Graphics.DrawClosedCurve(pen, per);
            }
            pen.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
