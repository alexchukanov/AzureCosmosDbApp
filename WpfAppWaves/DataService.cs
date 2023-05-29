using Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using static System.Net.WebRequestMethods;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

/// AzureCosmosDbApp
// This isa a PoC research for DIERS International GmbH which shows how to use AzureCosmos NoSQL DB cloud storage to keep images.
namespace WpfAppWaves
{
	public class DataService
	{
        static CosmosClient client;
        static Database db;
        static Microsoft.Azure.Cosmos.Container con;       
        static string continuation = null;

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
            List<Item> list = new List<Item>();

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

                list.AddRange(response);
            }
          
            return list;
        }
                
        public static async Task<List<Item>> LoadItemData(string query, int maxItemCount)
        {            
            continuation = null;

            List<Item> list = new();

            var queryDef = new QueryDefinition(query);

            if (maxItemCount == 0)
            {
                using FeedIterator<Item> iter = con.GetItemQueryIterator<Item>(queryDefinition: queryDef);
                var response = await iter.ReadNextAsync();
                list.AddRange(response);
            }
            else
            {
                using FeedIterator<Item> iter = con.GetItemQueryIterator<Item>(queryDefinition: queryDef,
                                                  requestOptions: new QueryRequestOptions { MaxItemCount = maxItemCount });

                var response = await iter.ReadNextAsync();
                list.AddRange(response);

                // Get continuation token once we've gotten > 0 results. 
                if (response.Count > 0)
                {
                    continuation = response.ContinuationToken;
                }
            }

            return list;
        }

        public static async Task<List<Item>> LoadMoreItemData(string query, int maxItemCount)
        {
            List<Item> list = new();

            if(continuation == null)
            {
                return list;
            }

            var queryDef = new QueryDefinition(query);

            using FeedIterator<Item> iter = con.GetItemQueryIterator<Item>(queryDefinition: queryDef,
                                              requestOptions: new QueryRequestOptions { MaxItemCount = maxItemCount },
                                              continuationToken: continuation);

            var response = await iter.ReadNextAsync();

            if (response.Count > 0)
            {                
                list.AddRange(response);
            }

            continuation = response.ContinuationToken;

            return list;
        }

        public static async Task<string> AddItemData(Item item)
        {            
            var response = await con.CreateItemAsync(item);

            return response.StatusCode.ToString();            
        }

        public static async Task<string> UpdateItemData(Item item)
        {
            var response = await con.UpsertItemAsync(item);

            return response.StatusCode.ToString();
        }

        public static async Task<string> DeleteItemData(Item item)
        {
            string res = "";

            Microsoft.Azure.Cosmos.PartitionKey partitionKey = new Microsoft.Azure.Cosmos.PartitionKey("item.categoryId");

            try
            {
                var response = await con.DeleteItemAsync<Item>(item.id, partitionKey);
                res = response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                res = "Resource Not Found";
            }

            return res;                    
        }

        public static async Task<int> CountItemData(string query)
        {
            int count = 0; 

            using FeedIterator<int> iter = con.GetItemQueryIterator<int>(
                       queryText: query
                       );

            var resp = await iter.ReadNextAsync();

            if (resp.StatusCode.ToString() == "OK" && resp.Count > 0)
            {
                count = resp.Resource.ToArray<int>()[0];
            }

            return count;
        }
    }
}
