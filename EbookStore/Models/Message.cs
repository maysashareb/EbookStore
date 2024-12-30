﻿namespace EbookStore.Models
{
    public class Message
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
