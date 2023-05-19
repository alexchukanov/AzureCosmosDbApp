﻿using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfAppWaves
{
	public class CosmosDbViewModel : INotifyPropertyChanged
	{
		string baseUrl = @"https://query1.finance.yahoo.com/v7/finance";
		
		public ObservableCollection<Item> ItemList { get; set; } = new();		

		public CosmosDbViewModel()
		{	
			AddCommand = new Command(OnAdd);
            DeleteCommand = new Command(OnDelete);
            FilterCommand = new Command(OnFilter);
            LoadCommand = new Command(OnLoad);
            UpdateCommand = new Command(OnUpdate);
            GridCommand = new Command(OnGrid);
            ClearCommand = new Command(OnClear);
            LoadImageCommand = new Command(OnLoadImage);

            Id = Guid.NewGuid().ToString();
            CategoryId = Guid.NewGuid().ToString();
            CategoryName = "myCategoryName";
            Sku = "mySku";
            Name = "myName"; 
            Description = "myDescription";
            Price = 0;
        }

        
        public async Task DeleteItemData()
        {
            Status = "Deleting...";                    
            
            Item item = new Item()
            {
                id = Id,
                categoryId = categoryId
             };

            if (ValidateItem(item))
            { 
                string res = await DataService.DeleteItemData(item);

                if (res == "NoContent") 
                {
                    Status = $"Deleted, press Load to refresh grid";
                }
                else
                {
                    Status = $"Error: {res}";
                }
            }
        }

        public async Task AddItemData()
        {
            Status = "Adding...";

            Item item = new Item()
            {
                id = Guid.NewGuid().ToString(),
                categoryId = Guid.NewGuid().ToString(),
                categoryName = CategoryName,
                sku = Sku,
                name = Name,
                description = Description,
                price = Price,
                image = Image
            };

            if (ValidateItem(item))
            {
                Status = await DataService.AddItemData(item);
                Status += ", press Load to refresh grid";
            }
        }

        public async Task UpdateItemData()
        {
            Status = "Updating...";

            Item item = new Item()
            {
                id = Id,
                categoryId = CategoryId,
                categoryName = CategoryName,
                sku = Sku,
                name = Name,
                description = Description,
                price = Price,
                image = Image
            };

            if (ValidateItem(item))
            {
                Status = await DataService.UpdateItemData(item);
                Status += ", press Load to refresh grid";
            }
        }

        public async Task LoadItemData()
		{
            Status = $"Loading...";

            ItemList.Clear();

            string query = "SELECT * FROM c ";            

            var itemList = await DataService.LoadItemData(query);

			foreach (Item item in itemList) 
			{
                ItemList.Add(item);
            }

            Status = $"Loaded: {ItemList.Count} items";
        }

        public async Task FilterItemData()
        {
            Status = $"Filtering...";

            ItemList.Clear();

            Item filter = new Item()
            {
                id = Id,
                categoryId = CategoryId,
                categoryName = CategoryName,
                sku = Sku,
                name = Name,
                description = Description                
            };

            var itemList = await DataService.FilterItemData(filter);

            foreach (Item item in itemList)
            {
                ItemList.Add(item);
            }

            Status = $"Filtered: {ItemList.Count} items";
        }

        public void SetSelectedItem(Item item)
        {
                Id = item.id;
                CategoryId = item.categoryId;
                CategoryName = item.categoryName;
                Sku = item.sku;
                Name = item.name;
                Description = item.description;
                Price = item.price;
                Image = item.image;
        }

        public void ClearSelectedItem()
        {
            Id = "";
            CategoryId = "";
            CategoryName = "";
            Sku = "";
            Name = "";
            Description = "";
            Price = 0;
            Image = "";
        }

        public bool ValidateItem(Item item)
        {            
            Guid guid;
            bool res = false;

            if (string.IsNullOrEmpty(Id))
            {
                Status = "id field is empty";
            }
            else if (string.IsNullOrEmpty(CategoryId))
            {
                Status = "CategoryId field is empty";
            }
            else if (!Guid.TryParse(Id, out guid))
            {
                Status = $"Wrong Id guid-format{guid}";
            }
            else if (!Guid.TryParse(CategoryId, out guid))
            {
                Status = $"Wrong CategoryId guid-format{CategoryId}";
            }
            else 
            { 
                res = true;
            }
            return res;
        }


        public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}


        #region Properties

        private string image = "";
        public string Image
        {
            get
            {
                return image;
            }

            set
            {
                if (image != value)
                {
                    image = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        private string id = "";
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private string categoryId = "";
        public string CategoryId
        {
            get
            {
                return categoryId;
            }

            set
            {
                if (categoryId != value)
                {
                    categoryId = value;
                    RaisePropertyChanged("CategoryId");
                }
            }
        }


        private string categoryName = "";
        public string CategoryName
        {
            get
            {
                return categoryName;
            }

            set
            {
                if (categoryName != value)
                {
                    categoryName = value;
                    RaisePropertyChanged("CategoryName");
                }
            }
        }

        private string sku = "";
        public string Sku
        {
            get
            {
                return sku;
            }

            set
            {
                if (sku != value)
                {
                    sku = value;
                    RaisePropertyChanged("Sku");
                }
            }
        }

        private string name = "";
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string description = "";
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        private double price = 0;
        public double Price
        {
            get
            {
               return price;
            }

            set
            {
                if (price != value)
                {
                    price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        private string status = "";
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        private bool isSave = false;
        public bool IsSave
        {
            get
            {
                return isSave;
            }

            set
            {
                if (isSave != value)
                {
                    isSave = value;
                    RaisePropertyChanged("isSave");
                }
            }
        }
        #endregion

        #region Commands
        //Command Add
        public Command AddCommand
        {
            get; set;
        }

        private async void OnAdd()
        {
            await AddItemData();
        }

        //Command Delete
        public Command DeleteCommand
        {
            get; set;
        }

        private async void OnDelete()
        {
            await DeleteItemData();
        }

        //Command Filter
        public Command FilterCommand
        {
            get; set;
        }

        private async void OnFilter()
        {
            await FilterItemData();
        }

        //Command Load
        public Command LoadCommand
        {
            get; set;
        }

        private async void OnLoad()
        {
            await LoadItemData();
        }

        //Command Update
        public Command UpdateCommand
        {
            get; set;
        }

        private async void OnUpdate()
        {
            await UpdateItemData();
        }

        //Command Grid
        public Command GridCommand
        {
            get; set;
        }

        private async void OnGrid()
        {

        }


        //Command Clear
        public Command ClearCommand
        {
            get; set;
        }

        private async void OnClear()
        {
            ClearSelectedItem();
        }

        //Command LoadImage
        public Command LoadImageCommand
        {
            get; set;
        }

        private async void OnLoadImage()
        {
            //LoadImage();
        }
        #endregion
    }
}
