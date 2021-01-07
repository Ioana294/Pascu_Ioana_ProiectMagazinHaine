using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Data;
using MagazinHaineModel;

namespace Pascu_Ioana_ProiectMagazinHaine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        MagazinHaineEntitiesModel ctx = new MagazinHaineEntitiesModel();
        CollectionViewSource produseViewSource;
        CollectionViewSource clientViewSource;
        CollectionViewSource clientComenziViewSource;
        Binding denumireTextBoxBinding = new Binding();
        Binding marimeTextBoxBinding = new Binding();
        Binding numeTextBoxBinding = new Binding();
        Binding prenumeTextBoxBinding = new Binding();
        Binding cmbProduseBinding = new Binding();
        Binding cmbClientBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            denumireTextBoxBinding.Path = new PropertyPath("Denumire");
            marimeTextBoxBinding.Path = new PropertyPath("Marime");
            numeTextBoxBinding.Path = new PropertyPath("Nume");
            prenumeTextBoxBinding.Path = new PropertyPath("Prenume");
            cmbProduseBinding.Path = new PropertyPath("Produse");
            cmbClientBinding.Path = new PropertyPath("Client");
            denumireTextBox.SetBinding(TextBox.TextProperty, denumireTextBoxBinding);
            marimeTextBox.SetBinding(TextBox.TextProperty, marimeTextBoxBinding);
            numeTextBox.SetBinding(TextBox.TextProperty, numeTextBoxBinding);
            prenumeTextBox.SetBinding(TextBox.TextProperty, prenumeTextBoxBinding);
            cmbProduse.SetBinding(ComboBox.TextProperty, cmbProduseBinding);
            cmbClient.SetBinding(ComboBox.TextProperty, cmbClientBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            produseViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("produseViewSource")));
            produseViewSource.Source = ctx.Produses.Local;
            clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            clientViewSource.Source = ctx.Clients.Local;
            clientComenziViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientComenziViewSource")));
            ctx.Produses.Load();
            ctx.Clients.Load();
            ctx.Comenzis.Load();

            cmbProduse.ItemsSource = ctx.Produses.Local;
            cmbProduse.SelectedValuePath = "ProdId";

            cmbClient.ItemsSource = ctx.Clients.Local;
            cmbClient.SelectedValuePath = "ClientId";
            BindDataGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Produse produse = null;
            if (action == ActionState.New)
            {
                try
                {
                    produse = new Produse()
                    {
                        Denumire = denumireTextBox.Text.Trim(),
                        Marime = marimeTextBox.Text.Trim()
                    };
                    ctx.Produses.Add(produse);
                    produseViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                produseDataGrid.IsEnabled = true;
                denumireTextBox.IsEnabled = true;
                marimeTextBox.IsEnabled = true;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    produse = (Produse)produseDataGrid.SelectedItem;
                    produse.Denumire = denumireTextBox.Text.Trim();
                    produse.Marime = marimeTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                produseViewSource.View.Refresh();
                produseViewSource.View.MoveCurrentTo(produse);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                produseDataGrid.IsEnabled = true;
                denumireTextBox.IsEnabled = false;
                marimeTextBox.IsEnabled = false;

                denumireTextBox.SetBinding(TextBox.TextProperty, denumireTextBoxBinding);
                marimeTextBox.SetBinding(TextBox.TextProperty, marimeTextBoxBinding);
                SetValidationBinding();

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    produse = (Produse)produseDataGrid.SelectedItem;
                    ctx.Produses.Remove(produse);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                produseViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                produseDataGrid.IsEnabled = true;
                denumireTextBox.IsEnabled = false;
                marimeTextBox.IsEnabled = false;

                denumireTextBox.SetBinding(TextBox.TextProperty, denumireTextBoxBinding);
                marimeTextBox.SetBinding(TextBox.TextProperty, marimeTextBoxBinding);
                SetValidationBinding();
            }
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            produseViewSource.View.MoveCurrentToNext();
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            produseViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            produseDataGrid.IsEnabled = false;
            denumireTextBox.IsEnabled = true;
            marimeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(denumireTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(marimeTextBox, TextBox.TextProperty);
            denumireTextBox.Text = "";
            marimeTextBox.Text = "";
            Keyboard.Focus(denumireTextBox);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempDenumire = denumireTextBox.Text.ToString();
            string tempMarime = marimeTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            produseDataGrid.IsEnabled = false;
            denumireTextBox.IsEnabled = true;
            marimeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(denumireTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(marimeTextBox, TextBox.TextProperty);
            denumireTextBox.Text = tempDenumire;
            marimeTextBox.Text = tempMarime;
            Keyboard.Focus(denumireTextBox);
            SetValidationBinding();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempDenumire = denumireTextBox.Text.ToString();
            string tempMarime = marimeTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            produseDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(denumireTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(marimeTextBox, TextBox.TextProperty);
            denumireTextBox.Text = tempDenumire;
            marimeTextBox.Text = tempMarime;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            produseDataGrid.IsEnabled = true;
            denumireTextBox.IsEnabled = false;
            marimeTextBox.IsEnabled = false;

            denumireTextBox.SetBinding(TextBox.TextProperty, denumireTextBoxBinding);
            marimeTextBox.SetBinding(TextBox.TextProperty, marimeTextBoxBinding);
        }

        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Client client = null;
            if (action == ActionState.New)
            {
                try
                {
                    client = new Client()
                    {
                        Nume = numeTextBox.Text.Trim(),
                        Prenume = prenumeTextBox.Text.Trim()
                    };
                    ctx.Clients.Add(client);
                    clientViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                clientDataGrid.IsEnabled = true;
                numeTextBox.IsEnabled = false;
                prenumeTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    client.Nume = numeTextBox.Text.Trim();
                    client.Prenume = prenumeTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();
                clientViewSource.View.MoveCurrentTo(client);

                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                clientDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                numeTextBox.IsEnabled = false;
                prenumeTextBox.IsEnabled = false;

                numeTextBox.SetBinding(TextBox.TextProperty, numeTextBoxBinding);
                prenumeTextBox.SetBinding(TextBox.TextProperty, prenumeTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    ctx.Clients.Remove(client);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();

                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                clientDataGrid.IsEnabled = true;
                numeTextBox.IsEnabled = false;
                prenumeTextBox.IsEnabled = false;

                numeTextBox.SetBinding(TextBox.TextProperty, numeTextBoxBinding);
                prenumeTextBox.SetBinding(TextBox.TextProperty, prenumeTextBoxBinding);
            }
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToNext();
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            clientDataGrid.IsEnabled = false;
            numeTextBox.IsEnabled = true;
            prenumeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(numeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(prenumeTextBox, TextBox.TextProperty);
            numeTextBox.Text = "";
            prenumeTextBox.Text = "";
            Keyboard.Focus(numeTextBox);
        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempNume = numeTextBox.Text.ToString();
            string tempPrenume = prenumeTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            clientDataGrid.IsEnabled = false;
            numeTextBox.IsEnabled = true;
            prenumeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(numeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(prenumeTextBox, TextBox.TextProperty);
            numeTextBox.Text = tempNume;
            prenumeTextBox.Text = tempPrenume;
            Keyboard.Focus(numeTextBox);
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempNume = numeTextBox.Text.ToString();
            string tempPrenume = prenumeTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            clientDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(numeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(prenumeTextBox, TextBox.TextProperty);
            numeTextBox.Text = tempNume;
            prenumeTextBox.Text = tempPrenume;
        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            clientDataGrid.IsEnabled = true;
            numeTextBox.IsEnabled = false;
            prenumeTextBox.IsEnabled = false;

            numeTextBox.SetBinding(TextBox.TextProperty, numeTextBoxBinding);
            prenumeTextBox.SetBinding(TextBox.TextProperty, prenumeTextBoxBinding);
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Comenzi comenzi = null;
            if (action == ActionState.New)
            {
                try
                {
                    Produse produse = (Produse)cmbProduse.SelectedItem;
                    Client client = (Client)cmbClient.SelectedItem;

                    comenzi = new Comenzi()
                    {
                        ProdId = produse.ProdId,
                        ClientId = client.ClientId
                    };
                    ctx.Comenzis.Add(comenzi);
                    clientComenziViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                comenzisDataGrid.IsEnabled = true;
                cmbProduse.IsEnabled = false;
                cmbClient.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedComanda = comenzisDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedComanda.IdComanda;

                    var editedComanda = ctx.Comenzis.FirstOrDefault(s => s.IdComanda == curr_id);
                    if (editedComanda != null)
                    {
                        editedComanda.ProdId = Int32.Parse(cmbProduse.SelectedValue.ToString());
                        editedComanda.ClientId = Int32.Parse(cmbClient.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                clientViewSource.View.MoveCurrentTo(selectedComanda);

                clientComenziViewSource.View.Refresh();
                clientComenziViewSource.View.MoveCurrentTo(comenzi);

                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                comenzisDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbProduse.IsEnabled = false;
                cmbClient.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedComanda = comenzisDataGrid.SelectedItem;

                    int curr_id = selectedComanda.IdComanda;
                    var deletedComanda = ctx.Comenzis.FirstOrDefault(s => s.IdComanda == curr_id);
                    if (deletedComanda != null)
                    {
                        ctx.Comenzis.Remove(deletedComanda);
                        ctx.SaveChanges();
                        MessageBox.Show("Comanda stearsa cu success", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientComenziViewSource.View.Refresh();
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                comenzisDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbProduse.IsEnabled = false;
                cmbClient.IsEnabled = false;

                cmbProduse.SetBinding(ComboBox.TextProperty, cmbProduseBinding);
                cmbClient.SetBinding(ComboBox.TextProperty, cmbClientBinding);
            }
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            comenzisDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbProduse.IsEnabled = true;
            cmbClient.IsEnabled = true;

            BindingOperations.ClearBinding(cmbProduse, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbClient, TextBox.TextProperty);
            cmbProduse.Text = "";
            cmbClient.Text = "";
            Keyboard.Focus(cmbClient);
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempProduse = cmbProduse.Text.ToString();
            string tempClient = cmbClient.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            comenzisDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbProduse.IsEnabled = true;
            cmbClient.IsEnabled = true;

            BindingOperations.ClearBinding(cmbProduse, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbClient, TextBox.TextProperty);
            cmbProduse.Text = tempProduse;
            cmbClient.Text = tempClient;
            Keyboard.Focus(cmbClient);
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempProduse = cmbProduse.Text.ToString();
            string tempClient = cmbClient.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            comenzisDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;

            BindingOperations.ClearBinding(cmbProduse, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbClient, TextBox.TextProperty);
            cmbProduse.Text = tempProduse;
            cmbClient.Text = tempClient;
        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            comenzisDataGrid.IsEnabled = true;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;
            cmbProduse.IsEnabled = false;
            cmbClient.IsEnabled = false;

            cmbProduse.SetBinding(TextBox.TextProperty, cmbProduseBinding);
            cmbClient.SetBinding(TextBox.TextProperty, cmbClientBinding);
        }

        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            clientComenziViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            clientComenziViewSource.View.MoveCurrentToNext();
        }
        private void BindDataGrid()
        {
            var queryComanda = from cmd in ctx.Comenzis join client in ctx.Clients on cmd.ClientId equals client.ClientId join prod in ctx.Produses on cmd.ProdId equals prod.ProdId select new { cmd.IdComanda, cmd.ProdId, cmd.ClientId, client.Nume, client.Prenume, prod.Denumire, prod.Marime };
            clientComenziViewSource.Source = queryComanda.ToList();
        }
        private void SetValidationBinding()
        {
            Binding numeValidationBinding = new Binding();
            numeValidationBinding.Source = clientViewSource;
            numeValidationBinding.Path = new PropertyPath("Nume");
            numeValidationBinding.NotifyOnValidationError = true;
            numeValidationBinding.Mode = BindingMode.TwoWay;
            numeValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            numeValidationBinding.ValidationRules.Add(new StringNotEmpty());
            numeTextBox.SetBinding(TextBox.TextProperty, numeValidationBinding);

            Binding prenumeValidationBinding = new Binding();
            prenumeValidationBinding.Source = clientViewSource;
            prenumeValidationBinding.Path = new PropertyPath("Prenume");
            prenumeValidationBinding.NotifyOnValidationError = true;
            prenumeValidationBinding.Mode = BindingMode.TwoWay;
            prenumeValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            prenumeValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            prenumeTextBox.SetBinding(TextBox.TextProperty, prenumeValidationBinding);

        }
    }
}
