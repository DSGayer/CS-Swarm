using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace CS_Swarm
{
    public partial class Form1 : Form
    {
        const int MAX = 200; // Maximum read distance of the sensors
        ArrayList rawData = new ArrayList();
        ArrayList frontiers = new ArrayList();
        ArrayList shapes = new ArrayList();
        MqttClient client = new MqttClient("192.168.1.59");
        bool scanning = false;

        PointF botLocation = new PointF(200, 200); // Value is dependent on Ian's Algorithm

        public Form1()
        {
            InitializeComponent();
        }

        private void ConnectToDataSet()
        {
            scanning = true;
            byte code = client.Connect(Guid.NewGuid().ToString());
            ushort msgId = client.Subscribe(new string[] { "/obstacle", "/dataset" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic.Equals("/dataset"))  // if this set of data is said to done by MQTT
            {
                scanning = false;  // done scanning for this pass
            } else
            {
                scanning = true;
                string[] obstacle = Encoding.UTF8.GetString(e.Message).Split(',');  // split the string from MQTT into polar coordinates
                float r = float.Parse(obstacle[0], CultureInfo.InvariantCulture.NumberFormat);  // get the distance
                float theta = float.Parse(obstacle[0], CultureInfo.InvariantCulture.NumberFormat);  // get the angle
                int id = (r <= MAX) ? 0 : 2;  // obstacle if within range, horizon if out of range
                OrderedPair newObstacle = new OrderedPair(r, theta, 0, botLocation);  // create new OrderedPair to add to rawData
                rawData.Add(newObstacle);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mapButton_Click(object sender, PaintEventArgs e)
        {
            while (true)  // purposefully making this never-ending TODO: maybe end when no frontiers exist
            {
                ConnectToDataSet();
                while (scanning)
                {
                    Debug.Write("scanning");
                }

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

                foreach (Shape s in shapes)
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
                client.Disconnect();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
