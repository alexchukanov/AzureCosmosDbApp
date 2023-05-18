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
		public ObservableCollection<string> ValidRanges{ get; set; } = new();
		public ObservableCollection<string> StepIntervals { get; set; } = new();

		public CosmosDbViewModel()
		{	
			AddCommand = new Command(OnAdd);
            DeleteCommand = new Command(OnDelete);
            FilterCommand = new Command(OnFilter);
            LoadCommand = new Command(OnLoad);
            UpdateCommand = new Command(OnUpdate);

            Id = Guid.NewGuid().ToString();
            CategoryId = Guid.NewGuid().ToString();
            CategoryName = "myCategoryName";
            Sku = "mySku";
            Name = "myName"; 
            Description = "myDescription";
            Price = 0;
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

        private void OnDelete()
        {

        }

        //Command Filter
        public Command FilterCommand
        {
            get; set;
        }

        private void OnFilter()
        {

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

        public async Task AddItemData()
        {
            Status = $"Adding";

            Item item = new Item()
            {
                id = Guid.NewGuid().ToString(),
                categoryId = Guid.NewGuid().ToString(),
                categoryName = CategoryName,
                sku = Sku,
                name = Name,
                description = Description,
                price = Price
            };
            
            Status = await DataService.AddItemData(item);
        }

        public async Task UpdateItemData()
        {
            Status = $"Updating...";

            Item item = new Item()
            {
                id = Id,
                categoryId = CategoryId,
                categoryName = CategoryName,
                sku = Sku,
                name = Name,
                description = Description,
                price = Price
            };

            Status = await DataService.UpdateItemData(item);
        }


        public async Task LoadItemData()
		{
            Status = $"Loading...";

            ItemList.Clear();

            string query = "SELECT * FROM c ";            

            var itemList = await DataService.GetItemData(query);

			foreach (Item item in itemList) 
			{
                ItemList.Add(item);
            }

            Status = $"Loaded: {ItemList.Count} items";
        }
				
		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}
