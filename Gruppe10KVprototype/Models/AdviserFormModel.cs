﻿namespace Gruppe10KVprototype.Models
{ 
    public class AdviserFormModel
    {
        public int Id { get; set; }  // Ny Id-egenskap
        public string Subject { get; set; }
        public bool Uttrykning { get; set; }
        public bool Something { get; set; }
        public bool AttachFile { get; set; }
        public string Description { get; set; }

        public string GeoJson { get; set; }
    }
}
