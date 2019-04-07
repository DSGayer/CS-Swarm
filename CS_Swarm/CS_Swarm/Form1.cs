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
            ushort subMsgId = client.Subscribe(new string[] { "/data", "/scanning" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.MqttMsgPublished += client_MqttMsgPublished;
            ushort pubMsgId = client.Publish("/scanning", // topic
               Encoding.UTF8.GetBytes("start"), // message body
               MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
               false); // retained
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic.Equals("/scanning"))  // if this set of data is said to done by MQTT
            {
                scanning = false;  // done scanning for this pass
            } else
            {
                scanning = true;
                string[] obstacle = Encoding.UTF8.GetString(e.Message).Split(',');  // split the string from MQTT into polar coordinates
                float r = float.Parse(obstacle[0], CultureInfo.InvariantCulture.NumberFormat);  // get the distance
                float theta = float.Parse(obstacle[0], CultureInfo.InvariantCulture.NumberFormat);  // get the angle
                int id = (r <= MAX) ? 0 : 1;  // obstacle if within range, horizon if out of range
                OrderedPair newObstacle = new OrderedPair(r, theta, 0, botLocation);  // create new OrderedPair to add to rawData
                rawData.Add(newObstacle);
            }
        }

        void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Debug.WriteLine("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mapButton_Click(object sender, PaintEventArgs e)
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
                generateNextLocation(borders, frontiers, botLocation);
                pen.Dispose();
                client.Disconnect();
        }

        private void generateNextLocation(ArrayList borders, ArrayList frontiers, PointF botLocation)
        {
            float totalX = 0; float totalY = 0; int numberOfPoints = 0;
            foreach(OrderedPair p in borders)
            {
                numberOfPoints++;
                totalX += p.toCartesian().X;
                totalY += p.toCartesian().Y;
            }
            float avgOppositeObstacleX = -totalX / numberOfPoints; float avgOppositeObstacleY = totalY / numberOfPoints;
            OrderedPair closestFrontier = (OrderedPair) frontiers[0];
            foreach (OrderedPair p in frontiers)
            {
                if(Math.Pow((p.toCartesian().X - avgOppositeObstacleX), 2) + Math.Pow((p.toCartesian().Y - avgOppositeObstacleY), 2) <
                    Math.Pow((closestFrontier.toCartesian().X - avgOppositeObstacleX), 2) + 
                    Math.Pow((closestFrontier.toCartesian().Y - avgOppositeObstacleY), 2))
                {
                    closestFrontier = p;
                }
            }
            PointF closestLine = new PointF(0, 0);
            int closestI = 0;
            int closestH = 0;
            for(int i = 0; i < 360; i++)
            {
                bool checkingLine = true;
                int h = 1;
                while (checkingLine) {
                    bool tooClose = false;
                    foreach (OrderedPair p in borders)
                    {
                        if((Math.Pow((h * Math.Cos(i * Math.PI / 180) - p.toCartesian().X), 2) + Math.Pow((h * Math.Sin(i * Math.PI / 180) - p.toCartesian().Y), 2)) < 10)
                        {
                            tooClose = true;
                        }
                    }
                    if (tooClose)
                    {
                        checkingLine = false;
                    }
                    if((Math.Pow((h * Math.Cos(i * Math.PI / 180) -  closestFrontier.toCartesian().X), 2) + 
                        Math.Pow((h * Math.Sin(i * Math.PI / 180) - closestFrontier.toCartesian().Y), 2)) < 
                        (Math.Pow((closestLine.X - closestFrontier.toCartesian().X), 2) + 
                        Math.Pow((closestLine.Y - closestFrontier.toCartesian().Y), 2)))
                    {
                        closestLine = new PointF((float) (h * Math.Cos(i * Math.PI / 180)), (float)(h * Math.Sin(i * Math.PI / 180)));
                        closestI = i;
                        closestH = h;
                    }
                }
            }
            botLocation = new PointF(botLocation.X + closestLine.X, botLocation.Y + closestLine.Y);
            sendNextLocation(closestH, closestI);
        }

        private void sendNextLocation(int closestH, int closestI)
        {
          ushort msgId = client.Publish("/command", // topic
            Encoding.UTF8.GetBytes(closestH + "," + closestI), // message body
            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
            true); // retained
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
