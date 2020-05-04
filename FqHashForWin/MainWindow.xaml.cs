using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FqHashForWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();
        DataTable dt = new DataTable();
        BackgroundWorker bgWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            bgWorker.WorkerSupportsCancellation = true;
            //MessageBox.Show();
        }

        private class SeqFileDetail
        {
            public string fullPath { get; set; }
            public string fileName { get; set; }
            public long fileSize { get; set; }
            public int totalSeq { get; set; }
            public string md5 { get; set; }
            public string verifyChecksum { get; set; }
            public bool match { get; set; }
        }

        void fillDataGridUsingDataTable()
        {
            var sfd = new SeqFileDetail();
            //here obj is instance of your node type
            // add columns
            foreach (var item in sfd.GetType().GetProperties())
            {
                dt.Columns.Add(item.Name, item.PropertyType);
            }

            // add rows
            DataRow _dr = dt.NewRow();
            _dr["fullPath"] = "/path/to/xx";
            _dr["fileName"] = "xx";
            _dr["fileSize"] = 123456789;
            _dr["totalSeq"] = 1234567;
            _dr["md5"] = "abc";
            //dt.Rows.Add(_dr);

            myDataGrid.ItemsSource = dt.DefaultView;
        }
        void fillDataGridUsingDataTable2()
        {
            DataColumn dc_fullpath = new DataColumn("Full Path", typeof(string));
            DataColumn dc_filename = new DataColumn("File Name", typeof(string));
            DataColumn dc_filesize = new DataColumn("File Size", typeof(long));
            DataColumn dc_seqcount = new DataColumn("Total Seq", typeof(int));
            DataColumn dc_md5calulated = new DataColumn("MD5", typeof(string));
            DataColumn dc_md5tocheck = new DataColumn("Verify Checksum", typeof(string));
            DataColumn dc_match = new DataColumn("Match", typeof(bool));
            dt.Columns.Add(dc_fullpath);
            dt.Columns.Add(dc_filename);
            dt.Columns.Add(dc_filesize);
            dt.Columns.Add(dc_seqcount);
            dt.Columns.Add(dc_md5calulated);
            dt.Columns.Add(dc_md5tocheck);
            dt.Columns.Add(dc_match);


            DataRow dr = dt.NewRow();
            dr[0] = "/path/to/xx";
            dr[1] = "xx";
            dr[2] = 123456;
            dr[3] = 876544;
            dr[4] = "alkjdglhlajdfkjalkdfjakdf";
            dr[5] = "";
            dr[6] = true;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "/path/to/yy";
            dr[1] = "yy";
            dr[2] = 1234567;
            dr[3] = 87654422;
            dr[4] = "123aefq345sf";
            dr[5] = "";
            dr[6] = false;
            dt.Rows.Add(dr);

            myDataGrid.ItemsSource = dt.DefaultView;
        }

        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // search folder(s) for fastq files
                var allowedExtensions = new[] {
                    ".fastq", ".fastq.gz", ".fastq.bz", ".fastq.bz2",
                    ".fq", ".fq.gz", ".fq.bz", ".fq.bz2",
                    ".bam", ".cram"
                };
                // sort
                var folderList = dialog.FileNames.ToList();
                folderList.Sort();
                // add files from each folder
                foreach (var folderpath in folderList)
                {
                    var filepaths = Directory
                        .GetFiles(folderpath, "*.*", SearchOption.AllDirectories)
                        .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                        .ToList();
                    // sort
                    filepaths.Sort();
                    foreach (var filepath in filepaths)
                    {
                        DataRow newrow = dt.NewRow();
                        newrow["fullPath"] = filepath;
                        newrow["fileName"] = System.IO.Path.GetFileName(filepath);
                        newrow["fileSize"] = new FileInfo(filepath).Length;
                        newrow["match"] = false;
                        dt.Rows.Add(newrow);
                    }
                }
            }
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            /*
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                MessageBox.Show("You selected: " + dialog.FileNames);
            }
            return;
            */

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.CheckFileExists = false;
            // openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "All files|*.*|fastq file|*.fastq;*.fq;*.fastq.gz;*.fq.gz;*.fastq.bz;*.fq.bz";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filepath in openFileDialog.FileNames)
                {
                    //MessageBox.Show(filepath);

                    /*
                    DataRow newrow = dt.NewRow();
                    newrow[0] = filepath;
                    newrow[1] = System.IO.Path.GetFileName(filepath);
                    newrow[2] = new FileInfo(filepath).Length;
                    newrow[6] = false;
                    // newrow[4] = GetHashFromFile(filename, Algorithms.MD5); // time consuming
                    dt.Rows.Add(newrow);
                    */
                    // add rows
                    DataRow newrow = dt.NewRow();
                    newrow["fullPath"] = filepath;
                    newrow["fileName"] = System.IO.Path.GetFileName(filepath);
                    newrow["fileSize"] = new FileInfo(filepath).Length;
                    newrow["match"] = false;
                    dt.Rows.Add(newrow);

                }
            }

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            myDataGrid.AutoGeneratingColumn += MyDataGrid_AutoGeneratingColumn;
            myDataGrid.AutoGeneratedColumns += MyDataGrid_AutoGeneratedColumns;
            myDataGrid.CellEditEnding += MyDataGrid_CellEditEnding;
            this.fillDataGridUsingDataTable();
            // customize header
            myDataGrid.Columns[0].Header = "File Name";
            myDataGrid.Columns[1].Header = "File Size";
            myDataGrid.Columns[2].Header = "Total Seq";
            myDataGrid.Columns[3].Header = "MD5";
            myDataGrid.Columns[4].Header = "Verify Checksum";
            myDataGrid.Columns[5].Header = "Match";
            Style s = new Style();
            s.Setters.Add(new Setter { Property = Control.FontWeightProperty, Value = FontWeights.Bold });
            s.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new FontFamily("Segoe UI") });
            s.Setters.Add(new Setter { Property = Control.FontSizeProperty, Value = Convert.ToDouble(12) });
            myDataGrid.ColumnHeaderStyle = s;
        }

        private void MyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // use background color to highlight match/mismatch checksums
            // and set match column
            if (e.EditAction == DataGridEditAction.Commit)
            {
                string md5_userinput = (e.EditingElement as TextBox).Text;
                FrameworkElement element = e.Column.GetCellContent(e.Row);
                DataGridColumn col = e.Column;
                DataGridRow dgRow = e.Row;
                DataRow dRow = (dgRow.DataContext as DataRowView).Row;
                // string md5_calculated = (myDataGrid.Columns[col_index - 1].GetCellContent(myDataGrid.Items[row_index]) as TextBlock).Text;
                string md5_calculated = dRow["md5"] as string;
                if (string.IsNullOrEmpty(md5_userinput) == false)
                {
                    if (md5_userinput == md5_calculated)
                    {
                        (element.Parent as DataGridCell).Background = new SolidColorBrush(Colors.LightGreen);
                        //dt.Rows[row_index][col_index + 2] = true;
                        dRow["match"] = true;
                    }
                    else
                    {
                        (element.Parent as DataGridCell).Background = new SolidColorBrush(Colors.LightPink);
                        //dt.Rows[row_index][col_index + 2] = false;
                        dRow["match"] = false;
                    }
                }
                (element.Parent as DataGridCell).Foreground = new SolidColorBrush(Colors.Black);

            }
        }

        private void MyDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            int ncols = ((DataGrid)sender).Columns.Count;
            // set column width
            for (int i = 0; i < ncols; i++)
            {
                if (i == 0 || i == 3 || i == 4)
                {
                    DataGridColumn col = ((DataGrid)sender).Columns[i];
                    col.Width = new DataGridLength(3, DataGridLengthUnitType.Star);
                    if (i == 0)
                    {
                        col.CellStyle = new Style();
                        //col.CellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightGray)));
                    }
                }
                else
                {
                    DataGridColumn col = ((DataGrid)sender).Columns[i];
                    col.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                }
            }

        }

        private void MyDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            // thousand separator
            DataGridTextColumn textcol = e.Column as DataGridTextColumn;
            if (headername == "fileSize" || headername == "totalSeq")
            {
                textcol.Binding = new Binding(e.PropertyName) { StringFormat = "{0:N0}" };
                Style s = new Style();
                s.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Right));
                textcol.ElementStyle = s;
            }

            if (headername.ToLower() == "fullpath")
            {
                // hide full path
                e.Cancel = true;
            }
            else if (headername.ToLower() == "verifychecksum")
            {
                // editable column
                e.Column.IsReadOnly = false;
            }
            else
            {
                e.Column.IsReadOnly = true;
            }

            // sorting
            if (headername.ToLower() == "md5" || headername.ToLower() == "verifychecksum")
            {
                e.Column.CanUserSort = false;
            }
            else
            {
                // allow user to sort by certain columns
                e.Column.CanUserSort = true;
            }
            // disable dragging colunn headers
            e.Column.CanUserReorder = false;
        }

        public static class Algorithms
        {
            public static readonly HashAlgorithm MD5 = new MD5CryptoServiceProvider();
            public static readonly HashAlgorithm SHA1 = new SHA1Managed();
            public static readonly HashAlgorithm SHA256 = new SHA256Managed();
            public static readonly HashAlgorithm SHA384 = new SHA384Managed();
            public static readonly HashAlgorithm SHA512 = new SHA512Managed();
            public static readonly HashAlgorithm RIPEMD160 = new RIPEMD160Managed();
        }

        public static string GetHashFromFile(string fileName, HashAlgorithm algorithm)
        {
            using (var stream = new BufferedStream(File.OpenRead(fileName), 1024 * 1024))
            {
                return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (bgWorker.IsBusy)
            {
                btnRun.IsEnabled = false;
                btnRun.Content = "❌ Stoping";
                bgWorker.CancelAsync();
            }
            else if (bgWorker.CancellationPending == false)
            {
                bgWorker.DoWork += BgWorker_DoWork;
                bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
                bgWorker.RunWorkerAsync(argument: cbCountSeq.IsChecked);
                // update UI
                btnRun.Content = "❌ Stop";
                btnAddFiles.IsEnabled = false;
                btnAddFolder.IsEnabled = false;
                btnDelete.IsEnabled = false;
                cbCountSeq.IsEnabled = false;
            }
        }

        // TODO add progress bar
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool countSeq = (bool)e.Argument;
            // myTextBox.AppendText("worker workiiiiing");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // statusLabel1.Content = "haha " + i;
                // MessageBox.Show(i + DateTime.Now.ToString());

                // cancel
                if (bgWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                string filepath = dt.Rows[i][0].ToString();
                // calculate md5 hash
                if (string.IsNullOrEmpty(dt.Rows[i]["md5"] as string))  // skip rows already calculated
                {
                    // show status
                    Dispatcher.Invoke(() =>
                    {
                        statusLabel1.Content = "Processing ... " + filepath;
                    });


                    // System.Threading.Thread.Sleep(1000);
                    string md5_calculated = GetHashFromFile(filepath, Algorithms.MD5);

                    // update table
                    if (bgWorker.CancellationPending == false)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dt.Rows[i]["md5"] = md5_calculated;
                        });
                    }
                }

                // count sequence
                // int x = (int) dt.Rows[i][3];
                var allowedFastqExtensions = new[] {
                            ".fastq", ".fastq.gz", ".fastq.bz", ".fastq.bz2",
                            ".fq", ".fq.gz", ".fq.bz", ".fq.bz2"
                        };
                bool isFastq = allowedFastqExtensions.Any((dt.Rows[i]["fileName"] as string).ToLower().EndsWith);
                if (countSeq == true && isFastq && (dt.Rows[i]["totalSeq"] == DBNull.Value || dt.Rows[i].Field<int>(3) < 0 ))
                {
                    int lines = -1;
                    try
                    {
                        if (filepath.ToLower().EndsWith("fastq") || filepath.ToLower().EndsWith("fq"))
                        {
                            lines = (int)FastqStream.CountLines(File.OpenRead(filepath));
                        }
                        else if (filepath.ToLower().EndsWith(".gz"))
                        {
                            using (GZipInputStream decompressionStream = new GZipInputStream(File.OpenRead(filepath)))
                            {
                                lines = (int)FastqStream.CountLines(decompressionStream);
                            }
                        }
                        else if (filepath.ToLower().EndsWith(".bz") || filepath.ToLower().EndsWith(".bz2"))
                        {
                            using (BZip2InputStream decompressionStream = new BZip2InputStream(File.OpenRead(filepath)))
                            {
                                lines = (int)FastqStream.CountLines(decompressionStream);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            statusLabel1.Content = "Error when counting sequences in file " + filepath;
                            //MessageBox.Show("Error counting sequences in file " + filepath);
                        });
                    }
                    finally
                    {
                        if (bgWorker.CancellationPending == false)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                dt.Rows[i]["totalSeq"] = lines > 0 ? lines / 4 : -1;

                            });
                        }
                    }
                }

            }
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                statusLabel1.Content = "Error";
                MessageBox.Show("There was an error! " + e.Error.ToString());
            }
            else
            {
                statusLabel1.Content = "Done";
                btnRun.IsEnabled = true;
                btnRun.Content = "▶️ Run";
                btnAddFiles.IsEnabled = true;
                btnAddFolder.IsEnabled = true;
                btnDelete.IsEnabled = true;
                cbCountSeq.IsEnabled = true;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            while (myDataGrid.SelectedCells.Count > 0)
            {
                DataRowView drv = myDataGrid.SelectedCells[0].Item as DataRowView;
                dt.Rows.Remove(drv.Row);
            }
        }

        private void datagrid_AutogeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLower() == "verify checksum")
            {
                e.Column.IsReadOnly = false;
            }
            else
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void versionLabel_Loaded(object sender, RoutedEventArgs e)
        {
            var label = sender as Label;
            label.Content = "v" + VERSION;
        }

        private void versionLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("FqHash for Win" + Environment.NewLine +
                "Version: " + VERSION + Environment.NewLine +
                "Developer: hliang" + Environment.NewLine +
                "Website: https://github.com/hliang/FqHashForWin",
                "About");
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            // TODO popup dialog to show status: searching/importing - done
            this.Activate();
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Array.Sort(files);
            foreach (string s in files)
            {
                if (Directory.Exists(s))
                {
                    var allowedExtensions = new[] {
                            ".fastq", ".fastq.gz", ".fastq.bz", ".fastq.bz2",
                            ".fq", ".fq.gz", ".fq.bz", ".fq.bz2",
                            ".bam", ".cram"
                        };
                    var filepaths = Directory
                    .GetFiles(s, "*.*", SearchOption.AllDirectories)
                    .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                    .ToList();
                    filepaths.Sort();
                    foreach (var filepath in filepaths)
                    {
                        DataRow newrow = dt.NewRow();
                        newrow["fullPath"] = filepath;
                        newrow["fileName"] = System.IO.Path.GetFileName(filepath);
                        newrow["fileSize"] = new FileInfo(filepath).Length;
                        newrow["match"] = false;
                        dt.Rows.Add(newrow);
                    }
                }
                else if (File.Exists(s))
                {
                    string filepath = s;
                    DataRow newrow = dt.NewRow();
                    newrow["fullPath"] = filepath;
                    newrow["fileName"] = System.IO.Path.GetFileName(filepath);
                    newrow["fileSize"] = new FileInfo(filepath).Length;
                    newrow["match"] = false;
                    dt.Rows.Add(newrow);
                }
            }
        }
    }

}
