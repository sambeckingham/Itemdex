﻿using System;
using System.IO;
using System.Text.Json;

namespace Itemdex.Core
{
    class Program
    {
        static void Main()
        {
            var itemdex = new Itemdex();
            Reader.ExtractJourneyModeProgressFromFile("./Roadman.plr", itemdex);
            if (itemdex.SaveVersion < 218)
            {
                Console.WriteLine(
                    $"Oh no, save version needs to be higher than 218, this one is {itemdex.SaveVersion}");
                return;
            }

            if (itemdex.NoProgress)
            {
                Console.WriteLine("No progress has been made! (Is this even a journey mode character?)");
                return;
            }

            var progressReport = itemdex.Evaluate();
            Console.WriteLine($"Completed: {progressReport.Complete.Count}");
            Console.WriteLine($"Incomplete: {progressReport.Incomplete.Count}");
            Console.WriteLine($"Not Started :(: {progressReport.NotStarted.Count}");
            
            File.WriteAllText("progessreport.json", JsonSerializer.Serialize(progressReport));
        }
    }
}