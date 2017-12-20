using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadXML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //private void Form1_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        foreach (string f in files)
        //        {
        //            listBox1.Items.Add(f);
        //        }
        //    }
        //}
        public void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            listBox1.Items.Add(path);
            DataTable dt = new DataTable();
            Program.ReadAll(path, ref dt);
            ////2.赋值，第一种方法  
            //dt = new DataTable() { };
            //DataRow newRow = dt.NewRow();
            //newRow["id"] = "0";
            //dt.Rows.Add(newRow);
            //FillDataGridViewWithDataSource(dataGridView1, dt);

            CreateNewDataRow();

        }
        public void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private DataTable MakeNamesTable()
        {
            // Create a new DataTable titled 'Names.'
            DataTable namesTable = new DataTable("Names");

            // Add three column objects to the table.
            DataColumn idColumn = new DataColumn();
            idColumn.DataType = System.Type.GetType("System.Int32");
            idColumn.ColumnName = "id";
            idColumn.AutoIncrement = true;
            namesTable.Columns.Add(idColumn);

            DataColumn fNameColumn = new DataColumn();
            fNameColumn.DataType = System.Type.GetType("System.String");
            fNameColumn.ColumnName = "Fname";
            fNameColumn.DefaultValue = "Fname";
            namesTable.Columns.Add(fNameColumn);

            DataColumn lNameColumn = new DataColumn();
            lNameColumn.DataType = System.Type.GetType("System.String");
            lNameColumn.ColumnName = "LName";
            namesTable.Columns.Add(lNameColumn);

            // Create an array for DataColumn objects.
            DataColumn[] keys = new DataColumn[1];
            keys[0] = idColumn;
            namesTable.PrimaryKey = keys;

            // Return the new DataTable.
            return namesTable;
        }


        private void CreateNewDataRow()
        {
            // Use the MakeTable function below to create a new table.
            DataTable table;
            table = MakeNamesTable();

            // Once a table has been created, use the 
            // NewRow to create a DataRow.
            DataRow row;
            row = table.NewRow();

            // Then add the new row to the collection.
            row["fName"] = "John";
            row["lName"] = "Smith";
            table.Rows.Add(row);

            foreach (DataColumn column in table.Columns)
                Console.WriteLine(column.ColumnName);
            dataGridView1.DataSource = table;
        }

        private void FillDataGridViewWithDataSource(DataGridView dataGridView, DataTable dTable)
        {
            //1.清空旧数据  
            dataGridView.Rows.Clear();
            //2.填充新数据  
            if (dTable != null && dTable.Rows.Count > 0)
            {
                //设置DataGridView列数据  
                dataGridView.Columns["ITEM_NO"].DataPropertyName = "ITEM_NO";
                dataGridView.Columns["ITEM_NAME"].DataPropertyName = "ITEM_NAME";
                dataGridView.Columns["INPUT_CODE"].DataPropertyName = "INPUT_CODE";

                //设置数据源，部分显示数据  
                dataGridView.DataSource = dTable;
                dataGridView.AutoGenerateColumns = false;
            }
        }





    }
}
