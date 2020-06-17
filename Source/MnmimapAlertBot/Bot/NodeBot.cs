using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
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
        public static ILog logger = LogManager.GetLogger("NodeBot");

        private INodeFinder NodeFinder;
        private bool isEnabled;
        private Stopwatch stopwatch = new Stopwatch();
        private static Random random = new Random();

        public NodeBot(INodeFinder bobberFinder)
        {
            this.NodeFinder = bobberFinder;

            logger.Info("NodeBot Created.");
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
                    logger.Error(e.ToString());
                    Sleep(2000);
                }
            }

            logger.Error("Bot has Stopped.");
        }

        public void Stop()
        {
            isEnabled = false;
            logger.Error("Bot is Stopping...");
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
                    logger.Info("Node found");
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
                FlushBuffers();
                Thread.Sleep(100);
            }
        }

        public static void FlushBuffers()
        {
            ILog log = LogManager.GetLogger("NodeBot");
            var logger = log.Logger as Logger;
            if (logger != null)
            {
                foreach (IAppender appender in logger.Appenders)
                {
                    var buffered = appender as BufferingAppenderSkeleton;
                    if (buffered != null)
                    {
                        buffered.Flush();
                    }
                }
            }
        }
    }
}