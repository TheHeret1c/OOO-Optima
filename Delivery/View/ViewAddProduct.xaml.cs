using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Delivery.Classes;
using Microsoft.Win32;
using System.IO;

namespace Delivery.View
{
    /// <summary>
    /// Логика взаимодействия для ViewAddProduct.xaml
    /// </summary>
    public partial class ViewAddProduct : Window
    {
        Model.Product product;
        bool flag; // Флаг добавления/редактирования товара

        /// <summary>
        /// Конструктор добавления товара
        /// </summary>
        public ViewAddProduct()
        {
            InitializeComponent();

            flag = true;

            product = new Model.Product();

            tbHeader.Text = "Добавление товара";
        }

        /// <summary>
        /// Конструктор редактирования товара
        /// </summary>
        /// <param name="product"></param>
        public ViewAddProduct(Model.Product product)
        {
            InitializeComponent();
            
            flag = false;

            this.product = product;
            tbHeader.Text = "Редактирование товара";
            tbCost.Text = product.ProductCost.ToString();
            tbDescription.Text = product.ProductDescription;
            tbDiscount.Text = product.ProductDiscount.ToString();
            tbName.Text = product.ProductName;
        }

        /// <summary>
        /// Заполнение комбобоксов данными из бд
        /// </summary>
        private void SetComboBoxes()
        {
            List<Model.Category> categories = Helper.DB.Category.ToList();
            List<Model.Provider> providers = Helper.DB.Provider.ToList();
            List<Model.Unit> units = Helper.DB.Unit.ToList();

            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "CategoryID";
            cbCategory.ItemsSource = categories;

            cbProvider.DisplayMemberPath = "ProviderName";
            cbProvider.SelectedValuePath = "ProviderID";
            cbProvider.ItemsSource = providers;

            cbUnit.DisplayMemberPath = "UnitName";
            cbUnit.SelectedValuePath = "UnitID";
            cbUnit.ItemsSource = units;

            if (product != null)
            {
                cbCategory.SelectedIndex = product.ProductCategory - 1;
                cbProvider.SelectedIndex = product.ProductProvider - 1;
                cbUnit.SelectedIndex = product.ProductUnit - 1;
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
                AddProduct();
            else
                UpdateProduct();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetComboBoxes();
        }

        /// <summary>
        /// Добавление нового товара в БД
        /// </summary>
        private void AddProduct()
        {
            product.ProductName = tbName.Text;
            product.ProductUnit = (cbUnit.SelectedItem as Model.Unit).UnitID;
            product.ProductCost = Convert.ToDouble(tbCost.Text);
            product.ProductDiscount = Convert.ToDouble(tbDiscount.Text);
            product.ProductCategory = (cbCategory.SelectedItem as Model.Category).CategoryID;
            product.ProductDescription = tbDescription.Text;
            product.ProductProvider = (cbProvider.SelectedItem as Model.Provider).ProviderID;

            MessageBox.Show("Записываю");

            Helper.DB.Product.Add(product);

            try
            {
                Helper.DB.SaveChanges();
                MessageBox.Show("Добавление успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Редактирование товара в БД
        /// </summary>
        private void UpdateProduct()
        {
            Model.Product editProduct = Helper.DB.Product.FirstOrDefault(x => x.ProductArticle == product.ProductArticle);

            if (editProduct != null)
            {
                editProduct.ProductCategory = (cbCategory.SelectedItem as Model.Category).CategoryID;
                editProduct.ProductCost = Convert.ToDouble(tbCost.Text);
                editProduct.ProductName = tbName.Text;
                editProduct.ProductDiscount = Convert.ToDouble(tbDiscount.Text);
                editProduct.ProductDescription = tbDescription.Text;
                editProduct.ProductProvider = (cbProvider.SelectedItem as Model.Provider).ProviderID;
                editProduct.ProductUnit = (cbUnit.SelectedItem as Model.Unit).UnitID;

                try
                {
                    Helper.DB.SaveChanges();
                    MessageBox.Show("Запись изменена!");
                }
                catch
                {
                    MessageBox.Show("Ошибка сохранения");
                }
            }
            else
            {
                MessageBox.Show("Запись не найдена");
            }
        }

        /// <summary>
        /// Загрузка изображения для товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.PNG)|*.PNG";

            bool? result = openFileDialog.ShowDialog();

            // Проверяем, был ли выбран файл
            if (result == true)
            {
                // Получаем путь к выбранному файлу
                string filePath = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName.Replace(".png", "");

                product.ProductImage = fileName;

                // Создаем новый путь и новое имя файла
                string newFilePath = Environment.CurrentDirectory + @"\..\..\Resources\Images\" + product.ProductImage + ".png";
                
                if (File.Exists(newFilePath))
                {
                    MessageBox.Show("Изображение с таким названием уже существует!");
                    return;
                }

                try
                {
                    // Копируем файл в новое место с новым именем
                    File.Copy(filePath, newFilePath);

                    // Выводим сообщение об успешном сохранении
                    Console.WriteLine("Файл успешно сохранен как: " + newFilePath);
                }
                catch (Exception ex)
                {
                    // Выводим сообщение об ошибке, если что-то пошло не так
                    Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
                }
            }
        }
    }
}
