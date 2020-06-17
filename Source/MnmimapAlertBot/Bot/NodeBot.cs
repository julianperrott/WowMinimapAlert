using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Media;

namespace MinimapAlert
{
    public class NodeBot
    {
        private INodeFinder NodeFinder;
        private bool isEnabled;
        private Stopwatch stopwatch = new Stopwatch();
        private static Random random = new Random();

        public NodeBot(INodeFinder bobberFinder)
        {
            this.NodeFinder = bobberFinder;
        }

        public void Start()
        {
            isEnabled = true;

            while (isEnabled)
            {
                try
                {
                    WaitForNode();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    Sleep(2000);
                }
            }

        }

        public void Stop()
        {
            isEnabled = false;
        }

        private void WaitForNode()
        {
            bool lastFound = false;

            // Wait for a node
            while (isEnabled)
            {

                if (this.NodeFinder.Find(lastFound) != null)
                {
                    lastFound = !lastFound;
                }
                else
                {
                    lastFound = false;
                    Thread.Sleep(100);
                }
            }
        }

        private DateTime StartTime = DateTime.Now;

        public static void Sleep(int ms)
        {
            ms+=random.Next(0, 225);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < ms)
            {
                Thread.Sleep(100);
            }
        }
    }
}