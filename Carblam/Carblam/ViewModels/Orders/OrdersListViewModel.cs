using Carblam.Models;
using Carblam.Views.Orders;
using Carblam.Views.Util;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Carblam.ViewModels.Orders
{
    public class OrdersListViewModel : MenuPageViewModel
    {
        #region ObservableCollection<OrderItem> Orders 

        public ObservableCollection<OrderItem> Orders
        {
            get { return (ObservableCollection<OrderItem>)this.GetValue(OrdersProperty); }
            set { this.SetValue(OrdersProperty, value); }
        }

        // Using a BindableProperty as the backing store for Orders. This enables animation, styling, binding, etc...
        public static readonly BindableProperty OrdersProperty =
            BindableProperty.Create("Orders", typeof(ObservableCollection<OrderItem>), typeof(OrdersListViewModel), default(ObservableCollection<OrderItem>));

        #endregion

        #region bool IsBusy 

        public bool IsBusy
        {
            get { return (bool)this.GetValue(IsBusyProperty); }
            set { this.SetValue(IsBusyProperty, value); }
        }

        // Using a BindableProperty as the backing store for IsBusy. This enables animation, styling, binding, etc...
        public static readonly BindableProperty IsBusyProperty =
            BindableProperty.Create("IsBusy", typeof(bool), typeof(OrdersListViewModel), default(bool));

        #endregion

        private readonly AppViewModel _app;
        private readonly RootAppViewModel _parent;

        public ICommand RefreshCommand { get; }

        public OrdersListViewModel(AppViewModel app, RootAppViewModel parent)
            : base("My deliveries")
        {
            _app = app;
            _parent = parent;

            this.Orders = new ObservableCollection<OrderItem>();
            this.RefreshCommand = new Command(this.Refresh);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.Refresh();
        }

        public async void Refresh()
        {
            this.IsBusy = true;
            var empty = new OrderInfoType[0];
            try
            {
                var o1 = await _app.Api.GetMyOutcomeOrders();
                var o2 = await _app.Api.GetMyIncomeOrders();

                this.Orders.Clear();
                var orders = (o1.Items ?? empty).Concat(o2.Items ?? empty).DistinctBy(o => o.Id).OrderByDescending(o => o.History.Created).ToArray();
                foreach (var order in orders)
                {
                    this.Orders.Add(new OrderItem(this, order));
                }
            }
            catch (WebApiException ex)
            {
                _app.PostError(ex.Message);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        internal void ShowDetails(OrderItem orderItem)
        {
            _parent.CurrentPage = new OrderViewModel(_app, orderItem) { PreviousPage = this };
        }
    }

    public class OrderItem : BindableObject
    {
        #region OrderInfoType Info 

        public OrderInfoType Info
        {
            get { return (OrderInfoType)this.GetValue(InfoProperty); }
            set { this.SetValue(InfoProperty, value); }
        }

        // Using a BindableProperty as the backing store for Info. This enables animation, styling, binding, etc...
        public static readonly BindableProperty InfoProperty =
            BindableProperty.Create("Info", typeof(OrderInfoType), typeof(OrderItem), default(OrderInfoType),
                propertyChanged: OnInfoPropertyChanged);

        #endregion

        static void OnInfoPropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (obj is OrderItem item)
                item.OnInfoPropertyChanged(oldValue, newValue);
        }

        public ICommand ShowDetailsCommand { get; private set; }
        public ICommand AcceptDeliveryCommand { get; private set; }

        #region bool ShowDetailsAvailable 

        public bool ShowDetailsAvailable
        {
            get { return (bool)this.GetValue(ShowDetailsAvailableProperty); }
            set { this.SetValue(ShowDetailsAvailableProperty, value); }
        }

        // Using a BindableProperty as the backing store for ShowDetailsAvailable. This enables animation, styling, binding, etc...
        public static readonly BindableProperty ShowDetailsAvailableProperty =
            BindableProperty.Create("ShowDetailsAvailable", typeof(bool), typeof(OrderItem), default(bool));

        #endregion

        #region bool AcceptDeliveryAvailable 

        public bool AcceptDeliveryAvailable
        {
            get { return (bool)this.GetValue(AcceptDeliveryAvailableProperty); }
            set { this.SetValue(AcceptDeliveryAvailableProperty, value); }
        }

        // Using a BindableProperty as the backing store for AcceptDeliveryAvailable. This enables animation, styling, binding, etc...
        public static readonly BindableProperty AcceptDeliveryAvailableProperty =
            BindableProperty.Create("AcceptDeliveryAvailable", typeof(bool), typeof(OrderItem), default(bool));

        #endregion

        #region string StatusString 

        public string StatusString
        {
            get { return (string)this.GetValue(StatusStringProperty); }
            set { this.SetValue(StatusStringProperty, value); }
        }

        // Using a BindableProperty as the backing store for StatusString. This enables animation, styling, binding, etc...
        public static readonly BindableProperty StatusStringProperty =
            BindableProperty.Create("StatusString", typeof(string), typeof(OrderItem), default(string));

        #endregion

        readonly DriverSessionViewModel _driver;

        public OrderItem(OrdersListViewModel owner, OrderInfoType info, DriverSessionViewModel driver = null)
        {
            _driver = driver;
            this.Info = info;
            // TODO: sender login
            this.ShowDetailsCommand = new Command(() => owner.ShowDetails(this));

            this.AcceptDeliveryCommand = new Command(() => driver.Accept(this));
            //this.Info.Spec.ConfirmMessage;

            this.ShowDetailsAvailable = true;
            this.AcceptDeliveryAvailable = driver != null && info.Status == OrderStatusType.Confirmed;
        }

        private void OnInfoPropertyChanged(object oldValue, object newValue)
        {
            if (newValue is OrderInfoType info)
            {
                this.StatusString = info.Status.ToString();

                if (_driver != null && 
                    _driver.CurrOrder != null && 
                    _driver.CurrOrder.Info.Id == this.Info.Id && info.Status == OrderStatusType.Done)
                    _driver.FinishOrder();
            }
        }

    }
}
