using Microsoft.AspNetCore.Mvc;
using System;

namespace TorneosBasketBall.Models
{
    public class AdminCardModel
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string IconClass { get; set; } = "";
        public string Title { get; set; } = "";
        public string BtnClass { get; set; } = "";
        public string Description { get; set; } = "";
    }
}