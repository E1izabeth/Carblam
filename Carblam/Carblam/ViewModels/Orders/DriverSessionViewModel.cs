using Carblam.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Carblam.ViewModels.Orders
{
    public class DriverSessionViewModel : MenuPageViewModel
    {
        private AppViewModel _app;
        private RootAppViewModel _root;

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

        #region OrderItem CurrOrder 

        public OrderItem CurrOrder
        {
            get { return (OrderItem)this.GetValue(CurrOrderProperty); }
            set { this.SetValue(CurrOrderProperty, value); }
        }

        // Using a BindableProperty as the backing store for CurrOrder. This enables animation, styling, binding, etc...
        public static readonly BindableProperty CurrOrderProperty =
            BindableProperty.Create("CurrOrder", typeof(OrderItem), typeof(DriverSessionViewModel), default(OrderItem));

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

        #region string CarDesignation 

        public string CarDesignation
        {
            get { return (string)this.GetValue(CarDesignationProperty); }
            set { this.SetValue(CarDesignationProperty, value); }
        }

        // Using a BindableProperty as the backing store for CarDesignation. This enables animation, styling, binding, etc...
        public static readonly BindableProperty CarDesignationProperty =
            BindableProperty.Create("CarDesignation", typeof(string), typeof(DriverSessionViewModel), default(string));

        #endregion

        #region string CarDescription 

        public string CarDescription
        {
            get { return (string)this.GetValue(CarDescriptionProperty); }
            set { this.SetValue(CarDescriptionProperty, value); }
        }

        // Using a BindableProperty as the backing store for CarDescription. This enables animation, styling, binding, etc...
        public static readonly BindableProperty CarDescriptionProperty =
            BindableProperty.Create("CarDescription", typeof(string), typeof(DriverSessionViewModel), default(string));

        #endregion

        #region bool NotWorking 

        public bool NotWorking
        {
            get { return (bool)this.GetValue(NotWorkingProperty); }
            set { this.SetValue(NotWorkingProperty, value); }
        }

        // Using a BindableProperty as the backing store for NotWorking. This enables animation, styling, binding, etc...
        public static readonly BindableProperty NotWorkingProperty =
            BindableProperty.Create("NotWorking", typeof(bool), typeof(DriverSessionViewModel), default(bool));

        #endregion

        #region bool IsWorking 

        public bool IsWorking
        {
            get { return (bool)this.GetValue(IsWorkingProperty); }
            set { this.SetValue(IsWorkingProperty, value); }
        }

        // Using a BindableProperty as the backing store for IsWorking. This enables animation, styling, binding, etc...
        public static readonly BindableProperty IsWorkingProperty =
            BindableProperty.Create("IsWorking", typeof(bool), typeof(DriverSessionViewModel), default(bool));

        #endregion

        #region bool OpenForOrders 

        public bool OpenForOrders
        {
            get { return (bool)this.GetValue(OpenForOrdersProperty); }
            set { this.SetValue(OpenForOrdersProperty, value); }
        }

        // Using a BindableProperty as the backing store for OpenForOrders. This enables animation, styling, binding, etc...
        public static readonly BindableProperty OpenForOrdersProperty =
            BindableProperty.Create("OpenForOrders", typeof(bool), typeof(DriverSessionViewModel), default(bool));

        #endregion

        #region bool HasOrderInProgress 

        public bool HasOrderInProgress
        {
            get { return (bool)this.GetValue(HasOrderInProgressProperty); }
            set { this.SetValue(HasOrderInProgressProperty, value); }
        }

        // Using a BindableProperty as the backing store for HasOrderInProgress. This enables animation, styling, binding, etc...
        public static readonly BindableProperty HasOrderInProgressProperty =
            BindableProperty.Create("HasOrderInProgress", typeof(bool), typeof(DriverSessionViewModel), default(bool));

        #endregion

        public ICommand StartDriveCommand { get; }
        public ICommand StopDriveCommand { get; }

        public ICommand RefreshCommand { get; }
        public ICommand AcceptCommand { get; }

        public DriverSessionViewModel(AppViewModel app, RootAppViewModel root)
            : base("Driver mode")
        {
            _app = app;
            _root = root;

            this.StartDriveCommand = new Command(() => this.Start());
            this.StopDriveCommand = new Command(() => this.Stop());
            this.RefreshCommand = new Command(() => this.Refresh());

            this.NotWorking = true;

            this.Orders = new ObservableCollection<OrderItem>();
        }

        public async void Setup()
        {
            try
            {
                var car = await _app.Api.GetCarInfo();
                this.CarDesignation = car?.Spec?.Designation ?? string.Empty;
                this.CarDescription = car?.Spec?.Description ?? string.Empty;
            }
            catch (WebApiException ex)
            {
                _app.PostError(ex.Message);
            }
            finally
            {
                _app.OperationInProgress = false;
            }
        }

        public async void Start()
        {
            try
            {
                _app.OperationInProgress = true;
                var loc = await RouteHelper.FindCurrentLocation();

                await _app.Api.StartWork(new StartWorkSpecType()
                {
                    Description = this.CarDescription,
                    Designation = this.CarDesignation,
                    Height = 0,
                    Width = 0,
                    Length = 0,
                    WeightLimit = 0
                });

                if (loc != null)
                    await _app.Api.UpdateLocation(new LocationType() { Latitude = loc.Latitude, Longitude = loc.Longitude });

                this.Refresh();

                this.IsWorking = true;
                this.NotWorking = false;
                this.OpenForOrders = true;
            }
            catch (WebApiException ex)
            {
                _app.PostError(ex.Message);
            }
            finally
            {
                _app.OperationInProgress = false;
            }
        }

        public async void Stop()
        {
            try
            {
                await _app.Api.StopWork();
                this.IsWorking = false;
                this.NotWorking = true;
                this.OpenForOrders = false;
            }
            catch (WebApiException ex)
            {
                _app.PostError(ex.Message);
            }
            finally
            {
                _app.OperationInProgress = false;
            }
        }

        public async void Accept(OrderItem selectedOrder)
        {
            try
            {
                if (this.OpenForOrders && selectedOrder != null)
                {
                    await _app.Api.AcceptOrder(selectedOrder.Info.Id);
                    this.CurrOrder = selectedOrder;
                    this.OpenForOrders = false;
                    this.HasOrderInProgress = true;
                    selectedOrder.ShowDetailsAvailable = true;
                    _root.CurrentPage = new OrderViewModel(_app, this.CurrOrder) { PreviousPage = this };
                   _app.StartLocationReporting();
                }
            }
            catch (WebApiException ex)
            {
                _app.PostError(ex.Message);
            }
            finally
            {
                _app.OperationInProgress = false;
            }
        }

        public void FinishOrder()
        {
            this.CurrOrder = null;
            this.OpenForOrders = true;
            this.HasOrderInProgress = false;
            this.Refresh();
            _app.StopLocationReporting();
        }

        public async void Refresh()
        {
            this.IsBusy = true;
            var empty = new OrderInfoType[0];
            try
            {
                var loc = await RouteHelper.FindCurrentLocation();
                if (loc == null)
                {
                    _app.PostError("Failed to discover nearest deliveries without location services being enabled");
                }
                else
                {
                    var oo = await _app.Api.GetUnacceptedOrdersNear(loc.Latitude, loc.Longitude);
                    var orders = (oo.Items ?? empty).Select(o => new OrderItem(_root.OrdersListPage, o, this) { ShowDetailsAvailable = o.Status != OrderStatusType.Confirmed }).ToArray();
                    if (orders.Length == 1 && orders.First().Info.Status != OrderStatusType.Confirmed)
                    {
                        if (!this.HasOrderInProgress)
                            _app.StartLocationReporting();

                        this.CurrOrder = orders.First();
                        this.OpenForOrders = false;
                        this.HasOrderInProgress = true;
                        this.IsWorking = true;
                    }

                    this.Orders.Clear();
                    foreach (var order in orders)
                    {
                        this.Orders.Add(order);
                    }
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
    }
}
