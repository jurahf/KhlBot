﻿namespace KhlBot.Model
{
    public class Club
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
