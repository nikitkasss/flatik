﻿using System.Collections.Generic;

namespace Kuzya.Monitoring.Settings
{
    public class MonitoringSettings
    {
        /// <summary>
        /// Defines collection of sites that will be monitored.
        /// </summary>
        public List<SiteSettings> Sites { get; set; }
    }
}
