﻿using System;
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
using System.Windows.Threading;
using DataModels;
using LinqToDB;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para MuestraRegistro.xaml
    /// </summary>
    public partial class MuestraRegistro : Window
    {
        //Id del ingreso o egreso
        private int Id;
        private int TipoConsulta;
        GeneraExcelEspec GeneraExcel = new GeneraExcelEspec();
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        List<ConsultaRegistroInfo> listaRegistros = new List<ConsultaRegistroInfo>();

        //Tipo consulta = 1, consulta de Ingresos. Else, consulta de Egresos
        public MuestraRegistro(int Id, int TipoConsulta)
        {
            InitializeComponent();

            this.Id = Id;
            this.TipoConsulta = TipoConsulta;

            LlenaInfo();

            LlenaListView();
        }

        private void btn_generaExcel_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

            btn_generaExcel.IsEnabled = false;

            dispatcherTimer.Start();

            if (TipoConsulta == 1)
            {
                GeneraExcel.CreaExcel(1, Id);
            }
            if(TipoConsulta == 2)
            {
                GeneraExcel.CreaExcel(2, Id);
            }
            if (TipoConsulta == 3)
            {
                btn_generaExcel.IsEnabled = false;
            }
            if (TipoConsulta == 4)
            {
                GeneraExcel.CreaExcel(4, Id);
            }
        }

        public void LlenaListView()
        {
            if (TipoConsulta == 1)
            {
                var ingresos = db.ProductsReceipts.LoadWith(foo => foo.Product).Where(re => re.ReceiptId.Equals(Id));

                foreach (var item in ingresos)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
            if(TipoConsulta == 2)
            {
                var egresos = db.EgressProducts.LoadWith(foo => foo.Product).Where(re => re.EgressId.Equals(Id));

                foreach (var item in egresos)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
            if (TipoConsulta == 3)
            {
                var traspaso = db.ProductsTransfers.LoadWith(foo => foo.Product).Where(re => re.TransferId.Equals(Id));

                foreach (var item in traspaso)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
            if (TipoConsulta == 4)
            {
                var compra = db.ProductsPurchases.LoadWith(foo => foo.Product).Where(re => re.PurchaseId.Equals(Id));

                foreach (var item in compra)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
        }

        public void LlenaInfo()
        {
            if (TipoConsulta == 1)
            {
                var ingreso = db.Receipts.LoadWith(foo => foo.Store).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "INGRESO EN: " + ingreso.Store.StoreName;
                txt_nRegistro.Text = ingreso.ReceiptCode;
                txt_fechaIngreso.Text = ingreso.ReceiptDate;
                txt_tipoIngreso.Text = ingreso.Type;
                txt_totalIngreso.Text = ingreso.TotalPriceReceipt.ToString();
            }
            if(TipoConsulta == 2)
            {
                var egreso = db.Egresses.LoadWith(foo => foo.Store).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "EGRESO EN: " + egreso.Store.StoreName;
                lbl_fecha.Content = "Fecha de Egreso:";
                lbl_tipo.Content = "Tipo de Egreso:";
                lbl_total.Content = "V. Total Egresado:";

                txt_nRegistro.Text = egreso.EgressCode;
                txt_fechaIngreso.Text = egreso.EgressDate;
                txt_tipoIngreso.Text = egreso.Type;
                txt_totalIngreso.Text = egreso.TotalPriceEgress.ToString();
            }
            if (TipoConsulta == 3)
            {
                var traspaso = db.Transfers.LoadWith(foo => foo.FromStore).LoadWith(foo => foo.ToStore).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "TRASPASO DESDE: " + traspaso.FromStore.StoreName + " - A: " + traspaso.ToStore.StoreName;
                lbl_fecha.Content = "Fecha de Traspaso:";
                lbl_tipo.Content = "";
                lbl_total.Content = "V. Total Traspasado:";

                txt_nRegistro.Text = traspaso.TransferCode;
                txt_fechaIngreso.Text = traspaso.TransferDate;
                txt_tipoIngreso.Visibility = Visibility.Collapsed;
                txt_totalIngreso.Text = traspaso.TotalPriceTransfer.ToString();
            }
            if (TipoConsulta == 4)
            {
                var compra = db.Purchases.LoadWith(foo => foo.Store).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "COMPRA EN: " + compra.Store.StoreName;
                lbl_fecha.Content = "Fecha de Compra:";
                lbl_tipo.Content = "Descuento:";
                lbl_total.Content = "V. Total Compra:";

                txt_nRegistro.Text = compra.PurchaseCode;
                txt_fechaIngreso.Text = compra.PurchaseDate;
                txt_tipoIngreso.Text = compra.Discount.ToString();
                txt_totalIngreso.Text = compra.TotalPrice.ToString();
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Things which happen after 1 timer interval
            btn_generaExcel.IsEnabled = true;
            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }
    }
}
