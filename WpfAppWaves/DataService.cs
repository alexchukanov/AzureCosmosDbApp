using Microsoft.Azure.Cosmos;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WpfAppWaves
{
	public class DataService
	{
        private static CosmosClient client;
        static Database db;
        static Container con;
        static DataService()
        {
            client = new CosmosClient(
               accountEndpoint: "https://8acb9bae-0ee0-4-231-b9ee.documents.azure.com:443/",
               authKeyOrResourceToken: "pzHs0F4QZJxEtapuzenQi2Fneq660Y27fmkLt3NANo26e89YHNVrIls59ibl7BxeJ1zGqankugbZACDbfD8wpg=="
               );

            db = client.GetDatabase("SampleDB");
            con = db.GetContainer("SampleContainer");
        }   

        public static async Task<List<Item>> GetItemData(string query)
        {
            var queryDef = new QueryDefinition(query);

            FeedIterator<Item> resp = con.GetItemQueryIterator<Item>(queryDef);

            var item = await resp.ReadNextAsync();

            List<Item> lists = item.ToList<Item>();

            return lists;
        }

        public static async Task<string> AddItemData(Item item)
        {            
            var resp = await con.CreateItemAsync(item);

            return resp.StatusCode.ToString();            
        }
    }
}
