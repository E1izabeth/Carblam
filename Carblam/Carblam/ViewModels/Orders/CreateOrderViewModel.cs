using BruTile.Wmts.Generated;
using Carblam.Models;
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
    class CreateOrderViewModel : MenuPageViewModel
    {
        private readonly AppViewModel _app;
        private readonly RootAppViewModel _root;

        public LocationSelectorViewModel From { get; }
        public LocationSelectorViewModel To { get; }

        #region MapLocInfo SelectedLoc 

        public MapLocInfo SelectedLoc
        {
            get { return (MapLocInfo)this.GetValue(SelectedLocProperty); }
            set { this.SetValue(SelectedLocProperty, value); }
        }

        // Using a BindableProperty as the backing store for SelectedLoc. This enables animation, styling, binding, etc...
        public static readonly BindableProperty SelectedLocProperty =
            BindableProperty.Create("SelectedLoc", typeof(MapLocInfo), typeof(CreateOrderViewModel), default(MapLocInfo));

        #endregion

        #region bool PreSelectingLoc 

        public bool PreSelectingLoc
        {
            get { return (bool)this.GetValue(PreSelectingLocProperty); }
            set { this.SetValue(PreSelectingLocProperty, value); }
        }

        // Using a BindableProperty as the backing store for PreSelectingLoc. This enables animation, styling, binding, etc...
        public static readonly BindableProperty PreSelectingLocProperty =
            BindableProperty.Create("PreSelectingLoc", typeof(bool), typeof(CreateOrderViewModel), default(bool));

        #endregion

        #region bool SelectingLoc 

        public bool SelectingLoc
        {
            get { return (bool)this.GetValue(SelectingLocProperty); }
            set { this.SetValue(SelectingLocProperty, value); }
        }

        // Using a BindableProperty as the backing store for SelectingLoc. This enables animation, styling, binding, etc...
        public static readonly BindableProperty SelectingLocProperty =
            BindableProperty.Create("SelectingLoc", typeof(bool), typeof(CreateOrderViewModel), default(bool));

        #endregion

        #region bool LocsList 

        public bool LocsList
        {
            get { return (bool)this.GetValue(LocsListProperty); }
            set { this.SetValue(LocsListProperty, value); }
        }

        // Using a BindableProperty as the backing store for LocsList. This enables animation, styling, binding, etc...
        public static readonly BindableProperty LocsListProperty =
            BindableProperty.Create("LocsList", typeof(bool), typeof(CreateOrderViewModel), default(bool));

        #endregion

        #region bool OrderForm 

        public bool OrderForm
        {
            get { return (bool)this.GetValue(OrderFormProperty); }
            set { this.SetValue(OrderFormProperty, value); }
        }

        // Using a BindableProperty as the backing store for OrderForm. This enables animation, styling, binding, etc...
        public static readonly BindableProperty OrderFormProperty =
            BindableProperty.Create("OrderForm", typeof(bool), typeof(CreateOrderViewModel), default(bool));

        #endregion

        #region LocationSelectorViewModel CurrentLoc 

        public LocationSelectorViewModel CurrentLoc
        {
            get { return (LocationSelectorViewModel)this.GetValue(CurrentLocProperty); }
            set { this.SetValue(CurrentLocProperty, value); }
        }

        // Using a BindableProperty as the backing store for CurrentLoc. This enables animation, styling, binding, etc...
        public static readonly BindableProperty CurrentLocProperty =
            BindableProperty.Create("CurrentLoc", typeof(LocationSelectorViewModel), typeof(CreateOrderViewModel), default(LocationSelectorViewModel));

        #endregion

        #region string ConfirmationMessage 

        public string ConfirmationMessage
        {
            get { return (string)this.GetValue(ConfirmationMessageProperty); }
            set { this.SetValue(ConfirmationMessageProperty, value); }
        }

        // Using a BindableProperty as the backing store for ConfirmationMessage. This enables animation, styling, binding, etc...
        public static readonly BindableProperty ConfirmationMessageProperty =
            BindableProperty.Create("ConfirmationMessage", typeof(string), typeof(CreateOrderViewModel), default(string));

        #endregion

        #region string ReceiverLogin 

        public string ReceiverLogin
        {
            get { return (string)this.GetValue(ReceiverLoginProperty); }
            set { this.SetValue(ReceiverLoginProperty, value); }
        }

        // Using a BindableProperty as the backing store for ReceiverLogin. This enables animation, styling, binding, etc...
        public static readonly BindableProperty ReceiverLoginProperty =
            BindableProperty.Create("ReceiverLogin", typeof(string), typeof(CreateOrderViewModel), default(string));

        #endregion

        #region string ParcelDescription 

        public string ParcelDescription
        {
            get { return (string)this.GetValue(ParcelDescriptionProperty); }
            set { this.SetValue(ParcelDescriptionProperty, value); }
        }

        // Using a BindableProperty as the backing store for ParcelDescription. This enables animation, styling, binding, etc...
        public static readonly BindableProperty ParcelDescriptionProperty =
            BindableProperty.Create("ParcelDescription", typeof(string), typeof(CreateOrderViewModel), default(string));

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

        #region bool IsMapCollapsed 

        public bool IsMapCollapsed
        {
            get { return (bool)this.GetValue(IsMapCollapsedProperty); }
            set { this.SetValue(IsMapCollapsedProperty, value); }
        }

        // Using a BindableProperty as the backing store for IsMapCollapsed. This enables animation, styling, binding, etc...
        public static readonly BindableProperty IsMapCollapsedProperty =
            BindableProperty.Create("IsMapCollapsed", typeof(bool), typeof(CreateOrderViewModel), default(bool));

        #endregion
             
        public ICommand LookupFromCommand { get; }
        public ICommand LookupToCommand { get; }

        public ICommand CancelLocSelectionCommand { get; }
        public ICommand PreSelectLocCommand { get; }
        public ICommand SelectLocCommand { get; }

        public ICommand BringLocsIntoViewMapCommand { get; set; }
        public ICommand BringPositionIntoViewMapCommand { get; set; }

        public ICommand CreateOrderCommand { get; }

        public CreateOrderViewModel(AppViewModel app, RootAppViewModel root)
            : base("New delivery")
        {
            _app = app;
            _root = root;

            this.From = new LocationSelectorViewModel();
            this.To = new LocationSelectorViewModel();

            this.DisplayedMapLocs = new ObservableCollection<MapLocInfo>();
            this.DisplayedMapRoutes = new ObservableCollection<MapRouteInfo>();

            this.LookupFromCommand = new Command(() => this.LookupAddressImpl(this.From));
            this.LookupToCommand = new Command(() => this.LookupAddressImpl(this.To));
            this.CancelLocSelectionCommand = new Command(() => this.CancelLocSelectionImpl());
            this.PreSelectLocCommand = new Command(() => this.ChooseLocImpl());
            this.SelectLocCommand = new Command(() => this.ConfirmAddressImpl());

            this.CreateOrderCommand = new Command(() => this.CreateOrderImpl());

            this.IsMapCollapsed = true;
            this.OrderForm = true;
            this.LocsList = false;
        }

        private void CancelLocSelectionImpl()
        {
            this.CurrentLoc = null;
            this.PreSelectingLoc = false;
            this.SelectingLoc = false;
            this.LocsList = false;
            this.OrderForm = true;
            this.IsMapCollapsed = true;
            this.BringLocsIntoViewIfAny();
        }

        private void ChooseLocImpl()
        { 
            if (this.PreSelectingLoc && this.SelectedLoc != null && this.CurrentLoc != null)
            {
                this.PreSelectingLoc = false;
                this.SelectingLoc = true;
            }
        }

        private async void ConfirmAddressImpl()
        {
            if (this.SelectingLoc && this.SelectedLoc != null && this.CurrentLoc != null)
            {
                this.CurrentLoc.Location = this.SelectedLoc;
                this.CurrentLoc.Address = this.SelectedLoc.Label;
                this.CurrentLoc = null;
                this.SelectingLoc = false;
                this.DisplayedMapLocs.Clear();

                if (this.From.Location != null && this.To.Location != null)
                {
                    var route = await RouteHelper.FindRoute(this.From.Location.Location, this.To.Location.Location);
                    if (route != null)
                    {
                        this.DisplayedMapRoutes.Add(new MapRouteInfo(Xamarin.Forms.Color.Blue, 3, route.ToList()));
                    }
                }

                if (this.From.Location != null)
                    this.DisplayedMapLocs.Add(this.From.Location);
                if (this.To.Location != null)
                    this.DisplayedMapLocs.Add(this.To.Location);

                this.LocsList = false;
                this.OrderForm = true;
                this.IsMapCollapsed = true;
                this.BringLocsIntoViewIfAny();
            }
        }

        private async void LookupAddressImpl(LocationSelectorViewModel locSelector)
        {
            if (!string.IsNullOrWhiteSpace(locSelector.Address))
            {
                this.CurrentLoc = locSelector;
                this.DisplayedMapLocs.Clear();
                this.DisplayedMapRoutes.Clear();

                var places = await RouteHelper.ResolveAddressToPlaces(locSelector.Address);
                var color = locSelector == this.From ? Xamarin.Forms.Color.Green : Xamarin.Forms.Color.Red;
                var locs = places.SelectMany(p => p).Select(p => p.ToLocInfo(color)).ToList();

                locs.ForEach(this.DisplayedMapLocs.Add);

                this.LocsList = true;
                this.OrderForm = false;
                this.PreSelectingLoc = true;

                if (locs.Any())
                {
                    var loc = locs.FirstOrDefault(); 
                    this.SelectedLoc = loc;
                    this.PreSelectingLoc = false;
                    this.SelectingLoc = true;
                    this.InvokeAction(() => this.BringPositionIntoViewMapCommand?.Execute(loc.Location));
                }
                else
                {
                    this.BringLocsIntoViewIfAny();
                }
            }
        }

        private void BringLocsIntoViewIfAny()
        {
            if (this.DisplayedMapLocs.Count > 0 && this.BringLocsIntoViewMapCommand != null)
            {
                this.InvokeAction(() => this.BringLocsIntoViewMapCommand?.Execute(null));
            }
        }

        private async void CreateOrderImpl()
        {
            if (this.From.Location == null || this.To.Location == null)
            {
                _app.PostError("Route not specified");
                return;
            }

            try
            {
                _app.OperationInProgress = true;

                var order = await _app.Api.CreateOrder(new OrderSpecType()
                {
                    FromAddress = this.From.Address,
                    FromLocation = new LocationType()
                    {
                        Latitude = this.From.Location.Location.Latitude,
                        Longitude = this.From.Location.Location.Longitude,
                    },
                    ToAddress = this.To.Address,
                    ToLocation = new LocationType()
                    {
                        Latitude = this.To.Location.Location.Latitude,
                        Longitude = this.To.Location.Location.Longitude,
                    },
                    ConfirmMessage = this.ConfirmationMessage,
                    ReceiverLogin = this.ReceiverLogin,
                    Parcel = new ParcelInfoType()
                    {
                        Description = this.ParcelDescription,
                        Height = 0,
                        Length = 0,
                        Width = 0,
                        Weight = 0,
                    }
                });

                _root.CurrentPage = new OrderViewModel(_app, new OrderItem(_root.OrdersListPage, order));
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
    }

    class LocationSelectorViewModel : BindableObject
    {
        #region string Address 

        public string Address
        {
            get { return (string)this.GetValue(AddressProperty); }
            set { this.SetValue(AddressProperty, value); }
        }

        // Using a BindableProperty as the backing store for Address. This enables animation, styling, binding, etc...
        public static readonly BindableProperty AddressProperty =
            BindableProperty.Create("Address", typeof(string), typeof(LocationSelectorViewModel), default(string));

        #endregion

        #region MapLocInfo Location 

        public MapLocInfo Location
        {
            get { return (MapLocInfo)this.GetValue(LocationProperty); }
            set { this.SetValue(LocationProperty, value); }
        }

        // Using a BindableProperty as the backing store for Location. This enables animation, styling, binding, etc...
        public static readonly BindableProperty LocationProperty =
            BindableProperty.Create("Location", typeof(MapLocInfo), typeof(LocationSelectorViewModel), default(MapLocInfo));

        #endregion

        public ICommand ClearCommand { get; }

        public LocationSelectorViewModel()
        {
            this.ClearCommand = new Command(() =>
            {
                this.Address = "";
                this.Location = null;
            });
        }
    }

}
