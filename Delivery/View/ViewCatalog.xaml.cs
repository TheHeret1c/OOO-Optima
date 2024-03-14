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
using System.IO;

namespace Delivery.View
{
    /// <summary>
    /// Логика взаимодействия для ViewCatalog.xaml
    /// </summary>
    public partial class ViewCatalog : Window
    {
        public ViewCatalog()
        {
            InitializeComponent();
        }

        List<Model.Product> productsInOrder;

        int filterDiscount, filterCategory; 		//Фильтр по скидке и категории
        string sort;					//Направление сортировки
        int countAll, countFilter;			//Количество всех записей и после фильтрации
        string searchProduct;			//Строка поиска
        //Массив диапазонов скидок
        double[,] arrayDiscount = new double[,] { { 0, 100 }, { 0, 10 }, { 11, 14 }, { 15, 100 } };

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Формирование списка категорий из таблицы БД + строка "Все категории"
            var category = Helper.DB.Category.Select(x => x.CategoryName).ToList();	//Все категории
            category.Insert(0, "Все категории");		//Добавить первой «Все категории»
            cbSortCategory.ItemsSource = category;		//Занести построенный список
            //Настроить видимость первой строки списка
            cbSortCategory.Text = cbSortCategory.Items[0].ToString();
            cbSortDiscount.Text = cbSortDiscount.Items[0].ToString();
            cbSortCost.Text = cbSortCost.Items[0].ToString();
            
            
            //Начальные данные для фильтрации
            filterDiscount = 0;
            filterCategory = 0;
            sort = "ASC";

            //Видимость кнопки для добавления товара (роль Администратора)
            btnAddProduct.Visibility = Visibility.Hidden;
            btnViewOrder.Visibility = Visibility.Hidden; 	//Недоступность кнопки "Сделать заказ"
            cmAddInOrder.Visibility = Visibility.Hidden;
            btnDeleteProduct.Visibility = Visibility.Hidden;
            btnEditOrder.Visibility = Visibility.Hidden;
            cmAddInOrder.Visibility = Visibility.Hidden;
            miEditProduct.Visibility = Visibility.Hidden;
            miAddInOrder.Visibility = Visibility.Hidden;
            if (Helper.user != null && Helper.user.UserRole == 2)
            {
                cmAddInOrder.Visibility = Visibility.Visible;
                btnAddProduct.Visibility = Visibility.Visible;
                btnDeleteProduct.Visibility = Visibility.Visible;
                btnEditOrder.Visibility = Visibility.Visible;
                miEditProduct.Visibility = Visibility.Visible;
            }
            //Возможность сделать заказ для клиента или менеджера
            if (Helper.user != null && Helper.user.UserRole == 1)	//Локальное меню для гостя и клиента
            {
                cmAddInOrder.Visibility = Visibility.Visible;
                btnViewOrder.Visibility = Visibility.Visible;
                miAddInOrder.Visibility = Visibility.Visible;
            }

            productsInOrder = new List<Model.Product>();

            ShowProduct();

        }

        private void cbSortCost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSortCost.SelectedIndex == 0) sort = "ASC";
            else sort = "DESC";
            ShowProduct();

        }

        private void cbSortDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterDiscount = cbSortDiscount.SelectedIndex;
            ShowProduct();
        }

        private void cbSortCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterCategory = cbSortCategory.SelectedIndex;
            ShowProduct();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchProduct = tbSearch.Text;
            ShowProduct();
        }

        List<Model.Product> products; 		//Результат обращения к БД

        /// Получить данные из БД по условия фильтра и отобразить их
        private void ShowProduct()
        {
            products = Helper.DB.Product.ToList();

            //Общее количество товаров в таблице товаров
            countAll = products.Count();
            //***********Создание сложного запроса из отдельных элементов
            //Фильтрация по скидке
            products = products.Where(x => (x.ProductDiscount >= arrayDiscount[filterDiscount, 0]
                                            && x.ProductDiscount <= arrayDiscount[filterDiscount, 1])).ToList();
            //Фильтрация по категории
            if (filterCategory != 0)
            {
                products = products.Where(x => x.ProductCategory == filterCategory).ToList();
            }
            //Поиск по названию
            if (!String.IsNullOrEmpty(searchProduct))
            {
                products = products.Where(x => x.ProductName.Contains(searchProduct)).ToList();
            }
            //Сортировка
            if (sort == "ASC")
            {
                products = products.OrderBy(x => x.ProductCost).ToList();
            }
            else
            {
                products = products.OrderByDescending(x => x.ProductCost).ToList();
            }
            //Количество записей посел фильтрации
            countFilter = products.Count();
            tbCount.Text = "Количество: " + countFilter + " из " + countAll;

            //Отобразить все продукты в ListView

            foreach (Model.Product product in products)
            {
                product.ProductCostWithDiscount = product.ProductCost - (product.ProductCost * (product.ProductDiscount / 100));
            }

            lbProducts.ItemsSource = products;            //Все продукты без слежения

        }
        private void miAddInOrder_Click(object sender, RoutedEventArgs e)
        {
            // Получаем MenuItem, на котором было совершено нажатие
            MenuItem menuItem = (MenuItem)sender;

            // Получаем контекстное меню, к которому принадлежит MenuItem
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;

            // Получаем ListBox, к которому принадлежит контекстное меню
            ListBox listBox = (ListBox)contextMenu.PlacementTarget;

            // Получаем выбранный элемент ListBox (ваш объект Product)
            Model.Product selectedProduct = (Model.Product)listBox.SelectedItem;
            
            if (productsInOrder.Find(p => p.ProductArticle == selectedProduct.ProductArticle) != null)
            {
                selectedProduct.ProductCount++;
            } else
            {
                selectedProduct.ProductCount = 1;
                productsInOrder.Add(selectedProduct);
            }

            
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Model.Product delProduct = lbProducts.SelectedItem as Model.Product; // Получаем выбранный продукт

            if (Helper.DB.OrderProduct.ToList().Find(p => p.ProductArticle == delProduct.ProductArticle) != null) // Проверяем на отсутствие в заказах
            {
                MessageBox.Show("Данный товар находится в заказах! Не удалю");
                return;
            }

            // Удаляем
            Helper.DB.Product.Remove(delProduct);
            
            try
            {
                Helper.DB.SaveChanges();
                MessageBox.Show("Удаление успешно");
            }
            catch
            {
                MessageBox.Show("Ошибка удаления");
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна для добавления товара
            View.ViewAddProduct viewAddProduct = new ViewAddProduct();
            this.Hide();
            viewAddProduct.ShowDialog();
            this.Show();
        }

        private void miEditProduct_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна для редактирования товара
            View.ViewAddProduct viewAddProduct = new ViewAddProduct(lbProducts.SelectedItem as Model.Product);
            this.Hide();
            viewAddProduct.ShowDialog();
            this.Show();
        }

        private void btnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            // Переход в окно оформления заказа
            if (productsInOrder.Count == 0)
            {
                MessageBox.Show("Корзина пуста, рано оформляться!");
                return;
            }
            View.ViewMakeOrder viewMakeOrder = new ViewMakeOrder(productsInOrder);
            this.Hide();
            viewMakeOrder.ShowDialog();
            this.Show();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
