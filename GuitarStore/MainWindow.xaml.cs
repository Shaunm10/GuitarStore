using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using NHibernate.GuitarStore.Common;
using NHibernate.GuitarStore.DataAccess;

namespace GuitarStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int totalInventoryCount;
        private int maxResultsPerPage = 5;
        private int firstResultInPage = 0;

        public MainWindow()
        {
            InitializeComponent();

            // test to be sure that Nhibernate can load the mapping files.
            NHibernateBase nHibernateBase = new NHibernateBase();
            nHibernateBase.Initilize("NHibernate.GuitarStore");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //test
            NHibernateInventory nHibernateInventory = new NHibernateInventory();
            var resultSet = nHibernateInventory.ExecuteNamedQuery("GuitarValueByTypeHQL");

            // populate the combo box with the guitars/inventory type.
            PopulateComboBox();

            // load the Inventory with all guitar types.
            LoadInventoryGrid();

            this.SetDatabaseRoundTripImage();
        }

        private void LoadInventoryGrid()
        {
            NHibernateInventory nHibernateInventory = new NHibernateInventory();
           
            // create a list of fields we want to get.
            List<string> fields = new List<string>
            {
                "Id", "Builder", "Model","Qoh","Cost","Price","Received", "Profit"
            };

            // get the selected guitar type, if one is selected.
            var typeId = this.GetSelectedGuitarType();
            
            // get the inventory from the DB.
            var inventoryTuple = nHibernateInventory.GetDynamicInventory(typeId: typeId, maxResult:this.maxResultsPerPage, firstResult:this.firstResultInPage);

            // get the total inventory count.
            this.totalInventoryCount = inventoryTuple.Item1;
            
            // set the data source to the list of inventory
            inventoryDataGrid.ItemsSource = BuildDataTable(fields, inventoryTuple.Item2).DefaultView;
            
            //var inventoryList = nHibernateInventory.ExecuteICriteriaOrderBy("Builder");

            // if we got results back hid the Id column
            if (inventoryTuple.Item2 != null && inventoryTuple.Item2.Count > 0)
            {
                inventoryDataGrid.Columns[0].Visibility = Visibility.Hidden;
            }
            gridCountLabel.Content = "Retrieved " + firstResultInPage.ToString() + " to " + (firstResultInPage + inventoryTuple.Item2.Count).ToString() + " of " + totalInventoryCount.ToString();

            // figure out which pagination buttons need to be available
            EnablePaginationButtons();
        }

        private Guid? GetSelectedGuitarType()
        {
            try
            {
                // if there is a selected item, and the combo box is not empty (this can happen when the combo box is cleared.)
                if (guitarTypesComboBox.SelectionBoxItem != null && guitarTypesComboBox.Items.Count > 0 && guitarTypesComboBox.SelectionBoxItem != string.Empty)
                {

                    // empty the grid if we are tring to fill it with the items filled b
                    //inventoryDataGrid.ItemsSource = null;

                    // get the guitar from the drop down list.
                    Guitar guitar = (Guitar)guitarTypesComboBox.SelectedItem;
                    Guid guitarType = new Guid(guitar.Id.ToString());

                    return guitarType;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                messagelabel.Content = ex.Message;
                throw;
            }

        }

        public void SetDatabaseRoundTripImage()
        {
            if (Utils.QueryCounter < 0)
            {
                databaseCounterImage.Source = (ImageSource)FindResource("databaseCounterRed");
                databaseCounterImage.ToolTip = "Error";
            }
            else if(Utils.QueryCounter==0)
            {
                // image is reset when, for the configuration is changed.
                databaseCounterImage.Source = (ImageSource) FindResource("databaseCounterGreen");
                databaseCounterImage.ToolTip = "";
            }
            else if (Utils.QueryCounter == 1)
            {
                databaseCounterImage.Source = (ImageSource) FindResource("databaseCounterGreen");
                databaseCounterImage.ToolTip = "1 round trip to database";
            }
            else if (Utils.QueryCounter == 2)
            {
                databaseCounterImage.Source = (ImageSource)FindResource("databaseCounterYellow");
                databaseCounterImage.ToolTip = "2 round trip to database";
            }
            else if (Utils.QueryCounter > 2)
            {
                databaseCounterImage.Source = (ImageSource)FindResource("databaseCounterRed");
                databaseCounterImage.ToolTip = Utils.QueryCounter +" round trip to database";
            }

            // reset the value each time this method is called.
            //Utils.QueryCounter = 0;
        }

        public DataTable BuildDataTable(List<string> columns, IList results)
        {
            DataTable dataTable = new DataTable();
            foreach (string column in columns)
            {
                dataTable.Columns.Add(column, typeof (string));
            }

            if (columns.Count > 1)
            {
                foreach (object[] row in results)
                {
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }


        private void PopulateComboBox()
        {
            NHibernateBase nhb = new NHibernateBase();

            // get all the guitar types from the DB.
            IList<Guitar> guitarTypes = nhb.ExecuteICriteria<Guitar>();

            // clear all items from the combo box.
            this.guitarTypesComboBox.Items.Clear();

            // add them to the combo box.
            foreach (var item in guitarTypes)
            {
                Guitar guitar = new Guitar {Id = item.Id, Type = item.Type};
                guitarTypesComboBox.DisplayMemberPath = "Type";
                guitarTypesComboBox.SelectedValuePath = "Id";
                guitarTypesComboBox.Items.Add(guitar);
            }
        }

       
        /// <summary>
        /// Selection Change event handler for the guitarTypesComboBox.
        /// </summary>
        /// <param name="sender">The object(ComboBox) firing the event.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void guitarTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // reload the grid.
            this.LoadInventoryGrid();

            this.SetDatabaseRoundTripImage();
        }

     
        /// <summary>
        /// Click event for the view Sql Button
        /// </summary>
        /// <param name="sender">The sender(button) sending the event</param>
        /// <param name="e"></param>
        private void viewSqlButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Utils.FormatSQL(), "Most recent Nhibernate Sql executed.",
                                           MessageBoxButton.OK, MessageBoxImage.Information);

        }

        /// <summary>
        /// Click event for the delete button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventoryItem = (Inventory) inventoryDataGrid.SelectedItem;
            Guid itemId = inventoryItem.Id;

            NHibernateInventory nhi = new NHibernateInventory();
            if (nhi.DeleteInventoryItem(itemId))
            {
                inventoryDataGrid.ItemsSource = null;
                
                // now we need to reload the grid.
                this.LoadInventoryGrid();

                messagelabel.Content = "Item deleted.";
            }
            else
            {
                messagelabel.Content = "Item deletion failed."; 
            }
        }

        /// <summary>
        /// Next button event handler.
        /// </summary>
        /// <param name="sender">The button performing the click.</param>
        /// <param name="e">The routed event arguments.</param>
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            previousButton.IsEnabled = true;
            this.firstResultInPage = this.firstResultInPage + (int)maxResultsPerPage;

            LoadInventoryGrid();

            // figure out which pagination buttons need to be available
           // EnablePaginationButtons();

        }

        /// <summary>
        /// Previous button event handler.
        /// </summary>
        /// <param name="sender">The button performing the click.</param>
        /// <param name="e">The routed event arguments.</param>
        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.firstResultInPage > 0)
            {
                this.firstResultInPage = firstResultInPage - maxResultsPerPage;
                if (this.firstResultInPage < 0)
                {
                    this.firstResultInPage = 0;
                }

                LoadInventoryGrid();

                // figure out which pagination buttons need to be available
                //EnablePaginationButtons();
            }
        }

        /// <summary>
        /// Enables the pagination buttons determined by where we are in the pagination.
        /// </summary>
        private void EnablePaginationButtons()
        {
            // the previous button should be enabled if the start position is greater than 0
            this.previousButton.IsEnabled = (this.firstResultInPage > 0);

            // the next button should be enabled if the hypothetical end position is less than
            // the total count of items available.
            this.nextButton.IsEnabled = (this.firstResultInPage + this.maxResultsPerPage < totalInventoryCount);
        }

        /// <summary>
        /// sum button click event handler.
        /// </summary>
        /// <param name="sender">The button sending the event (sum button)</param>
        /// <param name="e">The event arguments</param>
        private void sumButton_Click(object sender, RoutedEventArgs e)
        {
            NHibernateInventory nhi = new NHibernateInventory();
            List<string> fields = new List<string>
            {
                "Guitar Type","Total Value"
            };

            IList guitarInventory = nhi.ExecuteNamedQuery("GuitarValueByTypeHQL");
            inventoryDataGrid.ItemsSource = this.BuildDataTable(fields, guitarInventory).DefaultView;

            SetDatabaseRoundTripImage();
        }

        private void averageButton_Click(object sender, RoutedEventArgs e)
        {
            var nhi = new NHibernateInventory();
            List<string> fields = new List<string>
            {
                "Guitar Type","Total Value"
            };

            IList guitarInventory = nhi.ExecuteNamedQuery("GuitarAVGValueByTypeHQL");
            inventoryDataGrid.ItemsSource = this.BuildDataTable(fields, guitarInventory).DefaultView;

            SetDatabaseRoundTripImage();
        }

        private void minimumButton_Click(object sender, RoutedEventArgs e)
        {
            NHibernateInventory nhi = new NHibernateInventory();
            List<string> fields = new List<string>
            {
                "Guitar Type","Total Value"
            };

            IList guitarInventory = nhi.ExecuteNamedQuery("GuitarMINValueByTypeHQL");
            inventoryDataGrid.ItemsSource = this.BuildDataTable(fields, guitarInventory).DefaultView;

            SetDatabaseRoundTripImage();

        }

        private void maximumButton_Click(object sender, RoutedEventArgs e)
        {
            NHibernateInventory nhi = new NHibernateInventory();
            List<string> fields = new List<string>
            {
                "Guitar Type","Total Value"
            };

            IList guitarInventory = nhi.ExecuteNamedQuery("GuitarMAXValueByTypeHQL");
            inventoryDataGrid.ItemsSource = this.BuildDataTable(fields, guitarInventory).DefaultView;

            SetDatabaseRoundTripImage();

        }

        private void countButton_Click(object sender, RoutedEventArgs e)
        {
            NHibernateInventory nhi = new NHibernateInventory();
            List<string> fields = new List<string>
            {
                "Guitar Type","Total Value"
            };

            IList guitarInventory = nhi.ExecuteNamedQuery("GuitarCOUNTByTypeHQL");
            inventoryDataGrid.ItemsSource = this.BuildDataTable(fields, guitarInventory).DefaultView;

            SetDatabaseRoundTripImage();

        }
    }
}
