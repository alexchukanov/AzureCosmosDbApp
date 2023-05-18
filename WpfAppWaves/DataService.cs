using Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        static Microsoft.Azure.Cosmos.Container con;
        static DataService()
        {
            client = new CosmosClient(
               accountEndpoint: "https://8acb9bae-0ee0-4-231-b9ee.documents.azure.com:443/",
               authKeyOrResourceToken: "pzHs0F4QZJxEtapuzenQi2Fneq660Y27fmkLt3NANo26e89YHNVrIls59ibl7BxeJ1zGqankugbZACDbfD8wpg=="
               );

            db = client.GetDatabase("SampleDB");
            con = db.GetContainer("SampleContainer");
        }

        public static async Task<List<Item>> FilterItemData(Item filter)
        {          
            List<Item> lists = new List<Item>();

            IOrderedQueryable<Item> queryable = con.GetItemLinqQueryable<Item>();

            // Construct LINQ query
            var matches = queryable
                .Where(p => p.categoryName.Contains(filter.categoryName)
                 && p.name.Contains(filter.name)
                 && p.categoryId.StartsWith(filter.categoryId)                
                 && p.description.Contains(filter.description)
                 && p.id.StartsWith(filter.id)               
                 );
            
            // Convert to feed iterator
            using FeedIterator<Item> linqFeed = matches.ToFeedIterator();

            while (linqFeed.HasMoreResults)
            {
                var response = await linqFeed.ReadNextAsync();

                foreach (Item item in response)
                {
                    lists.Add(item);
                }
            }
          
            return lists;
        }

        public static async Task<List<Item>> LoadItemData(string query)
        {
            var queryDef = new QueryDefinition(query);

            FeedIterator<Item> resp = con.GetItemQueryIterator<Item>(queryDefinition: queryDef);

            var item = await resp.ReadNextAsync();

            List<Item> lists = item.ToList<Item>();

            return lists;
        }

        public static async Task<string> AddItemData(Item item)
        {            
            var resp = await con.CreateItemAsync(item);

            return resp.StatusCode.ToString();            
        }

        public static async Task<string> UpdateItemData(Item item)
        {
            var resp = await con.UpsertItemAsync(item);

            return resp.StatusCode.ToString();
        }

        public static async Task<string> DeleteItemData(Item item)
        {
            string res = "";

            Microsoft.Azure.Cosmos.PartitionKey partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(item.categoryId);
            try 
            {
                var resp = await con.DeleteItemAsync<Item>(item.id, partitionKey);
                res = resp.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                res = "Resource Not Found";
            }

            return res;                    
        }
    }
}
