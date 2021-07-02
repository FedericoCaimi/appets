using System;
using Appets.Domain;
using System.Collections.Generic;

namespace Appets.WebApi.Models
{
    public class NotificationMessage
    {
        public string To { get; set; }
        public Notification Notification { get; set; }
        public object Data { get; set; }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}