﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Itemdex.Core
{
    public class Itemdex
    {
        public Dictionary<string, Item> ItemsByTextId;
        public string CharacterName;
        public int SaveVersion;
        public bool NoProgress;

        public Itemdex()
        {
            ItemsByTextId = LoadItemList();
            NoProgress = false;
        }
        
        public void LoadProgress(BinaryReader reader)
        {
            var totalItemsResearched = reader.ReadInt32();
            if (totalItemsResearched == 0)
            {
                NoProgress = true;
                return;
            }
            
            for (var i = 0; i < totalItemsResearched; ++i)
            {
                var itemTextId = reader.ReadString();
                var progressCount = reader.ReadInt32();
                ItemsByTextId[itemTextId].ResearchProgress = progressCount;
            }
        }

        private Dictionary<string, Item> LoadItemList()
        {
            var itemDictionary = JsonSerializer.Deserialize<List<Item>>(File.ReadAllText("itemlist.json"));

            return itemDictionary.ToDictionary(item => item.TextId);
        }

        public ProgressReport Evaluate()
        {
            var report = new ProgressReport();
            foreach (var (_, item) in ItemsByTextId)
            {
                switch (item.ResearchProgress)
                {
                    case var x when x == 0:
                        report.NotStarted.Add(item);
                        break;
                    case var x when x < item.ResearchRequired:
                        report.Incomplete.Add(item);
                        break;
                    default:
                        report.Complete.Add(item);
                        break;
                } 
            }

            return report;
        }
    }

    public class ProgressReport
    {
        public List<Item> Complete { get; set; }
        public List<Item> Incomplete { get; set; }
        public List<Item> NotStarted { get; set; }

        public ProgressReport()
        {
            Complete = new List<Item>();
            Incomplete = new List<Item>();
            NotStarted = new List<Item>();
        }
    }
}