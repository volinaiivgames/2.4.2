using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace P2
{
    public class MyItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public MyItem(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
    public class MyData
    {
        public DateTime Date { get; set; }
        public List<MyItem> MyItems { get; set; }
        public MyData(DateTime date, List<MyItem> myItems)
        {
            Date = date;
            MyItems = myItems;
        }
    }

    public partial class MainWindow : Window
    {
        public static List<MyData> MyItems = new List<MyData>();
        public static string url = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public MainWindow()
        {
            InitializeComponent();
            GetData.SelectedDate = DateTime.Today;
            LoadFile();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            String name = TBname.Text;
            String description = TBdescription.Text;
            if (name.Length != 0 && description.Length != 0)
            {
                int i = MyItems.FindIndex(element => element.Date.Equals(GetData.SelectedDate));
                if (i == -1)
                {
                    MyItems.Add(new MyData(GetData.SelectedDate.Value, new List<MyItem>()));
                    i = MyItems.Count - 1;
                }

                MyItems[i].MyItems.Add(new MyItem(name, description));
                GetMyListBox.Items.Add(name);
                SaveFile();
            }
        }

        private void LBSelect(object sender, SelectionChangedEventArgs e)
        {
            int i = GetMyListBox.SelectedIndex;
            if (i < 0) return;

            MyData item = MyItems.Find(element => element.Date.Equals(GetData.SelectedDate));
            if (item != null)
            {
                TBname.Text = item.MyItems[i].Name;
                TBdescription.Text = item.MyItems[i].Description;
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            int i = GetMyListBox.SelectedIndex;
            if (i >= 0)
            {
                MyData item = MyItems.Find(element => element.Date.Equals(GetData.SelectedDate));
                if (item != null)
                {
                    GetMyListBox.SelectedIndex = -1;
                    item.MyItems.RemoveAt(i);
                    GetMyListBox.Items.RemoveAt(i);
                    SaveFile();
                }

            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            int i = GetMyListBox.SelectedIndex;
            if (i < 0) return;

            MyData item = MyItems.Find(element => element.Date.Equals(GetData.SelectedDate));
            if (item != null)
            {
                item.MyItems[i].Name = TBname.Text;
                item.MyItems[i].Description = TBdescription.Text;

                GetMyListBox.Items[i] = TBname.Text;
                SaveFile();
            }
        }

        private void GetData_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetMyListBox.Items.Clear();
            TBname.Text = "";
            TBdescription.Text = "";

            LoadMyItem();
        }

        public void LoadMyItem()
        {
            MyData item = MyItems.Find(element => element.Date.Equals(GetData.SelectedDate));
            if (item != null) foreach (MyItem element in item.MyItems) GetMyListBox.Items.Add(element.Name);
        }

        public void SaveFile()
        {
            string extension = Path.GetExtension(url + "/" + "data.json");
            if (extension == ".json")
            {
                using (StreamWriter sw = new StreamWriter(url + "/" + "data.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(MyItems));
                }
            }
        }
        public void LoadFile()
        {
            if (Path.IsPathRooted(url + "/" + "data.json"))
            {
                MyItems = JsonConvert.DeserializeObject<List<MyData>>(File.ReadAllText(url + "/" + "data.json"));
                LoadMyItem();
            }
        }
    }
}
