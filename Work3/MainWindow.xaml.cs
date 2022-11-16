using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Work3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument xmlDocument = new XmlDocument();
        Window window = new Window();
        string name, path, icon;
        public MainWindow()
        {
            InitializeComponent();

            XmlToPanel();
        }

        public void XmlToPanel()
        {
            MainRoot.Children.Clear();

            xmlDocument.Load(@"..\..\Data.xml");

            AppButton[,] buttons = new AppButton[2, 3];
            string[,] paths = new string[2, 3];
            int i = 0, j = 0;

            // Получим корневой элемент.
            XmlElement xRoot = xmlDocument.DocumentElement;
            if (xRoot != null)
            {
                // Обход всех узлов в корневом элементе.
                foreach (XmlElement xnode in xRoot)
                {
                    // Получаем атрибут name.
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");

                    Image icon = new Image();

                    buttons[i, j] = new AppButton();

                    // Обходим все дочерние узлы элемента app.
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "path")
                        {
                            buttons[i, j].path = childnode.InnerText;
                        }
                        if (childnode.Name == "icon")
                        {
                            icon.Source = new BitmapImage(new Uri(childnode.InnerText));
                        }
                    }

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = attr.Value;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;

                    Grid grid = new Grid();
                    RowDefinition rowDefinition1 = new RowDefinition();
                    RowDefinition rowDefinition2 = new RowDefinition();
                    grid.RowDefinitions.Add(rowDefinition1);
                    grid.RowDefinitions.Add(rowDefinition2);

                    Grid.SetRow(icon, 0);
                    Grid.SetRow(textBlock, 1);
                    grid.Children.Add(icon);
                    grid.Children.Add(textBlock);

                    buttons[i, j].Name = attr.Value;
                    buttons[i, j].Content = grid;
                    buttons[i, j].PreviewMouseDown += Button_PreviewMouseDown;

                    Grid.SetRow(buttons[i, j], i);
                    Grid.SetColumn(buttons[i, j], j);

                    MainRoot.Children.Add(buttons[i, j]);

                    if (j < 2)
                        j++;
                    else if (j >= 2 && i == 0)
                    {
                        j = 0;
                        i++;
                    }
                    else if (j >= 2 && i > 1)
                        break;
                }
            }

            if (j < 3 && i < 2)
            {
                Button button = new Button();
                button.Name = "AddButton";
                button.Content = new Image
                {
                    Source = new BitmapImage(new Uri("icons8-add-new-50.png", UriKind.Relative)),
                    Width = 25.0
                };
                button.Click += addButton_Click;
                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
                MainRoot.Children.Add(button);
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Process.Start((sender as AppButton).path);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                MessageBoxResult result = MessageBox.Show("Удалить программу?", "Удалить",
                                                            MessageBoxButton.YesNo,
                                                            MessageBoxImage.Question,
                                                            MessageBoxResult.Yes);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        XDocument xdoc = XDocument.Load(@"..\..\Data.xml");
                        XElement root = xdoc.Element("apps");

                        if (root != null)
                        {
                            // получим элемент app
                            var app = root.Elements("app")
                                .FirstOrDefault(p => p.Attribute("name").Value == (sender as AppButton).Name);
                            // и удалим его
                            if (app != null)
                            {
                                app.Remove();
                                xdoc.Save(@"..\..\Data.xml");
                            }
                        }

                        XmlToPanel();
                        break;
                }
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            window.Width = 400;
            window.Height = 300;

            Grid grid = new Grid();
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            RowDefinition rowDefinition3 = new RowDefinition();
            RowDefinition rowDefinition4 = new RowDefinition();

            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = GridLength.Auto;
            ColumnDefinition columnDefinition2 = new ColumnDefinition();

            grid.RowDefinitions.Add(rowDefinition1);
            grid.RowDefinitions.Add(rowDefinition2);
            grid.RowDefinitions.Add(rowDefinition3);
            grid.RowDefinitions.Add(rowDefinition4);
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);

            Label label1 = new Label();
            label1.Content = "Название:";
            label1.Margin = new Thickness(10);
            label1.HorizontalAlignment = HorizontalAlignment.Center;
            label1.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(label1, 0);
            Grid.SetColumn(label1, 0);
            grid.Children.Add(label1);
            TextBox textBox = new TextBox();
            textBox.Margin = new Thickness(10);
            textBox.TextChanged += TextBox_TextChanged;
            Grid.SetRow(textBox, 0);
            Grid.SetColumn(textBox, 1);
            grid.Children.Add(textBox);

            Button button1 = new Button();
            button1.Name = "Path";
            button1.Content = "Выбрать путь к программе";
            button1.Margin = new Thickness(10);
            button1.Click += ChooseButton_Click;
            Grid.SetColumnSpan(button1, 2);
            Grid.SetRow(button1, 1);
            Grid.SetColumn(button1, 0);
            grid.Children.Add(button1);

            Button button2 = new Button();
            button2.Name = "Icon";
            button2.Content = "Выбрать иконку";
            button2.Margin = new Thickness(10);
            button2.Click += ChooseButton_Click;
            Grid.SetColumnSpan(button2, 2);
            Grid.SetRow(button2, 2);
            Grid.SetColumn(button2, 0);
            grid.Children.Add(button2);

            Button button3 = new Button();
            button3.Content = "Добавить программу";
            button3.Margin = new Thickness(10);
            button3.Click += addProgram_Click;
            Grid.SetColumnSpan(button3, 2);
            Grid.SetRow(button3, 3);
            Grid.SetColumn(button3, 0);
            grid.Children.Add(button3);

            window.Owner = this;
            window.Content = grid;
            window.ShowDialog();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            name = (sender as TextBox).Text;
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            switch ((sender as Button).Name)
            {
                case "Path":
                    openFileDialog1.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
                    break;
                case "Icon":
                    openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.ico|All Files (*.*)|*.*";
                    break;
            }
            bool? result = openFileDialog1.ShowDialog();
            if(result == true)
            {
                switch ((sender as Button).Name)
                {
                    case "Path":
                        path = openFileDialog1.FileName;
                        break;
                    case "Icon":
                        icon = openFileDialog1.FileName;
                        break;
                }
            }
        }

        private void addProgram_Click(object sender, RoutedEventArgs e)
        {
            XmlElement appElem = xmlDocument.CreateElement("app");
            XmlAttribute nameAttr = xmlDocument.CreateAttribute("name");
            XmlElement pathElem = xmlDocument.CreateElement("path");
            XmlElement iconElem = xmlDocument.CreateElement("icon");

            XmlText nameText = xmlDocument.CreateTextNode(name);
            XmlText pathText = xmlDocument.CreateTextNode(path);
            XmlText iconText = xmlDocument.CreateTextNode(icon);

            nameAttr.AppendChild(nameText);
            pathElem.AppendChild(pathText);
            iconElem.AppendChild(iconText);

            appElem.Attributes.Append(nameAttr);
            appElem.AppendChild(pathElem);
            appElem.AppendChild(iconElem);
            xmlDocument.DocumentElement.AppendChild(appElem);

            xmlDocument.Save(@"..\..\Data.xml");
            window.Close();
            MainRoot.Children.Clear();
            XmlToPanel();
        }
    }

    class AppButton : Button
    {
        public string path { get; set; }
    }
}
