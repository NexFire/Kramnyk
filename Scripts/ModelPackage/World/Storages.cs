using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;

namespace ModelPackage.World
{
    public class Storage
    {
        public static List<Storage> LoadStorages(string merchantsFile)
        {
            try
            {
                string json = File.ReadAllText(merchantsFile);
                var availableMerchants = JsonConvert.DeserializeObject<List<Storage>>(json);
                return availableMerchants ?? new List<Storage>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Storage>();
            }
        }
        public int Id { get; set; }
        public int Capacity { get; set; }
        public Dictionary<string, string> Name { get; set; }

    }
}