﻿using System;
using System.Collections.Generic;

namespace TrueCareer.BE.Models
{
    public partial class ServerDAO
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
