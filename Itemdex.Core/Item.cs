﻿namespace Itemdex.Core
{
    public class Item
    {
        public int Id { get; set; }
        public string TextId { get; set; }
        public int ResearchRequired { get; set; }
        public int ResearchProgress { get; set; }
    }
}