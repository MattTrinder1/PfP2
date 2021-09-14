﻿using System;
using System.Collections.Generic;

namespace Common.Models.Business
{
    public class PocketNotebook : EntityBase
    {
        public PocketNotebook()
        {
            Photos = new List<Photo>();
        }

        public List<Photo> Photos { get; set; }

        public string Notes { get; set; }

        public DateTime NoteDateAndTime { get; set; }

        public byte[] Sketch { get; set; }

        public byte[] Signature { get; set; }

        
        public DateTime? SignatureDateandTime { get; set; }

        public string SignatoryName { get; set; }
    }
}
