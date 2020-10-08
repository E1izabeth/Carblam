using Carblam.Models;
using Carblam.Views;
using Carblam.Views.Util;
using Mapsui.Rendering.Skia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Carblam.ViewModels.Orders
{
    class OrderViewModel : MenuPageViewModel
    {
        private readonly AppViewModel _app;

        #region OrderItem Item 

        public OrderItem Item
        {
            get { return (OrderItem)this.GetValue(ItemProperty); }
            set { this.SetValue(ItemProperty, value); }
        }

        // Using a BindableProperty as the backing store for Item. This enables animation, styling, binding, etc...
        public static readonly BindableProperty ItemProperty =
            BindableProperty.Create("Item", typeof(OrderItem), typeof(OrderViewModel), default(OrderItem));

        #endregion

        #region ObservableCollection<MapLocInfo> DisplayedMapLocs 

        public ObservableCollection<MapLocInfo> DisplayedMapLocs
        {
            get { return (ObservableCollection<MapLocInfo>)this.GetValue(DisplayedMapLocsProperty); }
            set { this.SetValue(DisplayedMapLocsProperty, value); }
        }

        // Using a BindableProperty as the backing store for DisplayedMapLocs. This enables animation, styling, binding, etc...
        public static readonly BindableProperty DisplayedMapLocsProperty =
            BindableProperty.Create("DisplayedMapLocs", typeof(ObservableCollection<MapLocInfo>), typeof(CreateOrderViewModel), default(ObservableCollection<MapLocInfo>));

        #endregion

        #region ObservableCollection<MapRouteInfo> DisplayedMapRoutes 

        public ObservableCollection<MapRouteInfo> DisplayedMapRoutes
        {
            get { return (ObservableCollection<MapRouteInfo>)this.GetValue(DisplayedMapRoutesProperty); }
            set { this.SetValue(DisplayedMapRoutesProperty, value); }
        }

        // Using a BindableProperty as the backing store for DisplayedMapRoutes. This enables animation, styling, binding, etc...
        public static readonly BindableProperty DisplayedMapRoutesProperty =
            BindableProperty.Create("DisplayedMapRoutes", typeof(ObservableCollection<MapRouteInfo>), typeof(CreateOrderViewModel), default(ObservableCollection<MapRouteInfo>));

        #endregion

        #region string HistoryText 

        public string HistoryText
        {
            get { return (string)this.GetValue(HistoryTextProperty); }
            set { this.SetValue(HistoryTextProperty, value); }
        }

        // Using a BindableProperty as the backing store for HistoryText. This enables animation, styling, binding, etc...
        public static readonly BindableProperty HistoryTextProperty =
            BindableProperty.Create("HistoryText", typeof(string), typeof(OrderViewModel), default(string));

        #endregion

        #region string CreationStampText 

        public string CreationStampText
        {
            get { return (string)this.GetValue(CreationStampTextProperty); }
            set { this.SetValue(CreationStampTextProperty, value); }
        }

        // Using a BindableProperty as the backing store for CreationStampText. This enables animation, styling, binding, etc...
        public static readonly BindableProperty CreationStampTextProperty =
            BindableProperty.Create("CreationStampText", typeof(string), typeof(OrderViewModel), default(string));

        #endregion

        #region string TillStampText 

        public string TillStampText
        {
            get { return (string)this.GetValue(TillStampTextProperty); }
            set { this.SetValue(TillStampTextProperty, value); }
        }

        // Using a BindableProperty as the backing store for TillStampText. This enables animation, styling, binding, etc...
        public static readonly BindableProperty TillStampTextProperty =
            BindableProperty.Create("TillStampText", typeof(string), typeof(OrderViewModel), default(string));

        #endregion

        public ICommand BringLocsIntoViewMapCommand { get; set; }

        public HidableCommand ConfirmCommand { get; }
        public HidableCommand AcceptCommand { get; }
        public HidableCommand StartLoadCommand { get; }
        public HidableCommand FinishLoadCommand { get; }
        public HidableCommand DeliverCommand { get; }
        public HidableCommand FinishCommand { get; }
        public HidableCommand CancelCommand { get; }

        public ICommand MapLocsChangedCommand { get; }

        DateTime _stamp;

        public OrderViewModel(AppViewModel app, OrderItem item)
            : base("Order details")
        {
            _app = app;

            this.DisplayedMapLocs = new ObservableCollection<MapLocInfo>();
            this.DisplayedMapRoutes = new ObservableCollection<MapRouteInfo>();

            this.ConfirmCommand = new HidableCommand(
                () => this.Perform(() => app.Api.ConfirmOrder(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Created && item.Info.IsMyIncome
            );
            this.CancelCommand = new HidableCommand(
                () => this.Perform(() => app.Api.CancelOrder(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Created && item.Info.IsMyOutcome
            );
            this.AcceptCommand = new HidableCommand(
                () => this.Perform(() => app.Api.AcceptOrder(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Confirmed && item.Info.IsMyBaggage
            );
            this.StartLoadCommand = new HidableCommand(
                () => this.Perform(() => app.Api.ConfirmOrderLoading(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Accepted && item.Info.IsMyBaggage
            );
            this.FinishLoadCommand = new HidableCommand(
                () => this.Perform(() => app.Api.ConfirmOrderMoving(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Loading && item.Info.IsMyBaggage
            );
            this.DeliverCommand = new HidableCommand(
                () => this.Perform(() => app.Api.ConfirmOrderUnloading(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.InProgress && item.Info.IsMyBaggage
            );
            this.FinishCommand = new HidableCommand(
                () => this.Perform(() => app.Api.ConfirmOrderDone(this.Item.Info.Id)),
                () => item.Info.Status == OrderStatusType.Unloading && item.Info.IsMyBaggage
            );
            this.MapLocsChangedCommand = new Command(() => this.BringLocsIntoViewMapCommand?.Execute(null));

            this.FillInfo(item);
        }

        private async void Perform(Func<Task> act)
        {
            try
            {
                _app.OperationInProgress = true;

                await act();
                this.Item.Info = await _app.Api.GetOrder(this.Item.Info.Id);
                this.InvokeAction(() => this.FillInfo(this.Item));
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

        private void FillInfo(OrderItem item)
        {
            this.Item = item;

            this.DisplayedMapLocs.Clear();
            this.DisplayedMapLocs.Add(new MapLocInfo(
                Color.Green,
                item.Info.Spec.FromAddress,
                item.Info.Spec.FromLocation.Latitude,
                item.Info.Spec.FromLocation.Longitude
            ));
            this.DisplayedMapLocs.Add(new MapLocInfo(
                Color.Red,
                item.Info.Spec.ToAddress,
                item.Info.Spec.ToLocation.Latitude,
                item.Info.Spec.ToLocation.Longitude
            ));

            var car = item.Info.Car;
            if (car != null)
            {
                // car.Spec.Designation
                // car.Spec.Description
                this.DisplayedMapLocs.Add(new MapLocInfo(
                    Color.Blue,
                    car.Spec.Designation + Environment.NewLine + car.Spec.Description,
                    car.Location.Latitude,
                    car.Location.Longitude
                ));
            }

            this.DisplayedMapRoutes.Clear();
            // TODO: store routes on the server side

            var historyItems = this.CollectHistory(item.Info).OrderBy(x => x.t).ToArray();
            var historyTail = historyItems.Last();

            if (_stamp < historyTail.t.Value)
                _stamp = historyTail.t.Value;

            this.CreationStampText = item.Info.History.Created.Value.ToLocalTime().ToString();
            this.TillStampText = historyTail.t.Value.ToLocalTime() + historyTail.s;

            this.HistoryText = string.Join(Environment.NewLine, historyItems.Select(x => x.t.Value.ToLocalTime() + " " + x.s));

            this.ConfirmCommand.ChangeCanExecute();
            this.AcceptCommand.ChangeCanExecute();
            this.StartLoadCommand.ChangeCanExecute();
            this.FinishLoadCommand.ChangeCanExecute();
            this.DeliverCommand.ChangeCanExecute();
            this.FinishCommand.ChangeCanExecute();
            this.CancelCommand.ChangeCanExecute();


            this.BringLocsIntoViewMapCommand?.Execute(null);
        }

        private IEnumerable<(string s, DateTime? t)> CollectHistory(OrderInfoType info)
        {
            var h = info.History;

            var items = new (string, DateTime?)[]
            {
                ("Created", h.Created),
                ("Confirmed", h.Confirmed),
                ("Accepted", h.Accepted),
                ("Loading", h.Loading),
                ("Loaded", h.Loaded),
                ("Delivered", h.Delivered),
                ("Finished", h.Finished),
                ("Cancelled", h.Cancelled),
            };

            return items.Where(x => x.Item2.HasValue);
        }

        bool IsFinished()
        {
            return this.Item.Info.Status == OrderStatusType.Canceled || this.Item.Info.Status == OrderStatusType.Done;
        }

        async void WatchNotifyProc()
        {
            System.Diagnostics.Debug.Print("<><><> Starting pooling for notifications about #" + this.Item.Info.Id);
            while (this.IsPresented && !this.IsFinished())
            {
                try
                {
                    var stamp = await _app.Api.WaitForNotify(this.Item.Info.Id, _stamp, TimeSpan.FromSeconds(30));
                    var newStamp = new DateTime(stamp.Value);
                    System.Diagnostics.Debug.Print("<><><> Pooling success about #" + this.Item.Info.Id + ": " + stamp.Value);

                    if (_stamp < newStamp)
                    {
                        _stamp = newStamp;

                        var newOrderInfo = await _app.Api.GetOrder(this.Item.Info.Id);

                        this.Item.Info = null;
                        this.Item.Info = newOrderInfo;
                        this.FillInfo(this.Item);
                    }
                }
                catch (WebApiException ex)
                {
                    if (ex.StatusCode.HasValue && ex.StatusCode.Value == HttpStatusCode.RequestTimeout)
                    {
                        System.Diagnostics.Debug.Print("<><><> Pooling timeout about #" + this.Item.Info.Id);
                        continue;
                    }
                    else
                        break;
                }
            }
            System.Diagnostics.Debug.Print("<><><> Break pooling for notifications about #" + this.Item.Info.Id);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.WatchNotifyProc();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
