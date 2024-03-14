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

namespace Delivery.View
{
    /// <summary>
    /// Логика взаимодействия для ViewMakeOrder.xaml
    /// </summary>
    public partial class ViewMakeOrder : Window
    {
        List<Model.Product> products;
        public ViewMakeOrder(List<Model.Product> products)
        {
            InitializeComponent();
            this.products = products;

            // Получаем банковские карты пользователя
            List<Model.BankCard> bankCards = Helper.DB.BankCard.ToList();

            bankCards = bankCards.Where(b => b.BankCardOwner == Helper.user.UserID).ToList();

            // Отображаем банковские карты пользователя
            cbCard.DisplayMemberPath = "BankCardNumber";
            cbCard.SelectedValuePath = "BankCardID";
            cbCard.ItemsSource = bankCards;

            ShowProducts();
        }

        private void ShowProducts()
        {
            // Обновление списка выбранных продуктов
            int count = 0;
            double amount = 0, amountWithDiscount = 0, discount = 0;
            foreach (Model.Product product in products)
            {
                count += product.ProductCount;
                amount += product.ProductCost * product.ProductCount;
                amountWithDiscount += product.ProductCostWithDiscount * product.ProductCount;
            }
            discount = amount - amountWithDiscount;

            tbCountPozInOrder.Text = "Количество позиций в заказе: " + products.Count;
            tbCoundProductsInOrder.Text = "Количество товаров в заказе: " + count;
            tbAmountOrder.Text = "Общая сумма за весь товар: " + amount;
            tbAmountDiscount.Text = "Общая сумма скидки: " + discount;
            tbAmountWitnDiscount.Text = "Общая сумма заказа со скидкой: " + amountWithDiscount;

            lbOrder.ItemsSource = null;
            lbOrder.ItemsSource = products;
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Model.Point> points = Helper.DB.Point.ToList();
            cbPoint.DisplayMemberPath = "PointAddress";
            cbPoint.SelectedValuePath = "PointID";
            cbPoint.ItemsSource = points;
            lbOrder.ItemsSource = products;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного продукта и увеличение количества
            Model.Product product = (sender as Button).DataContext as Model.Product;
            products[products.IndexOf(product)].ProductCount++;
            MessageBox.Show("Новое кол-во: " + products[products.IndexOf(product)].ProductCount);
            lbOrder.UpdateLayout();
            ShowProducts();
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного продукта и уменьшение количества
            Model.Product product = (sender as Button).DataContext as Model.Product;
            if (product.ProductCount > 1)
            {
                products[products.IndexOf(product)].ProductCount--;
                MessageBox.Show("Новое кол-во: " + products[products.IndexOf(product)].ProductCount);
            }
            ShowProducts();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного продукта и удаление продукта
            Model.Product product = (sender as Button).DataContext as Model.Product;
            products.Remove(product);
            MessageBox.Show("Продукт удалён!");
            ShowProducts();
        }

        private void btnMakeOrder_Click(object sender, RoutedEventArgs e)
        {
            // Если не все поля заполнены
            if (cbCard.SelectedItem == null || cbPoint.SelectedItem == null)
            {
                MessageBox.Show("Не все данные указаны!");
                return;
            }

            // Получение последнего номера заказа для формирования правильной связки с таблицей OrderProducts
            int order_id;
            Model.Order order = new Model.Order();
            List<Model.Order> orders = Helper.DB.Order.ToList();
            if (orders.Count == 0)
            {
                order.OrderCode = 10001;
                order_id = 1;
            }
            else
            {
                order.OrderCode = orders.Last().OrderCode + 1;
                order_id = orders.Last().OrderID + 1;
            }

            //Создание заказа
            order.OrderClient = Helper.user.UserID;
            order.OrderDate = DateTime.Now;
            order.OrderPoint = (int)cbPoint.SelectedValue;
            order.OrderStatus = 1;

            Helper.DB.Order.Add(order);

            orders = Helper.DB.Order.ToList();


            // Добавление товаров в таблицу OrderProduct
            foreach (Model.Product product in products)
            {
                Model.OrderProduct orderProduct = new Model.OrderProduct();
                orderProduct.OrderID = order_id;
                orderProduct.ProductArticle = product.ProductArticle;
                orderProduct.ProductCount = product.ProductCount;

                Helper.DB.OrderProduct.Add(orderProduct);
            }

            try
            {
                Helper.DB.SaveChanges();
                MessageBox.Show("Заказ успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

        }
    }
}
