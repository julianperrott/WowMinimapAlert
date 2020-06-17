﻿using System.IO;
using System.Windows;

namespace MinimapAlert
{
    public partial class App : Application
    {
        public App()
        {
            log4net.Config.XmlConfigurator.Configure(new FileStream("log4net.config", FileMode.Open));
        }
    }
}