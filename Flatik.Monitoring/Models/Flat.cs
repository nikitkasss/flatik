﻿using System;

namespace Flatik.Monitoring.Models
{
    public class Flat
    {
        public int Id { get; set; }
        public string Site { get; set; }
        public int Rooms { get; set; }
        public bool IsOwner { get; set; }
        public int UsdPrice { get; set; }
        public int BynPrice { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpAt { get; set; }
    }
}