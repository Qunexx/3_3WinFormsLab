using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Windows.Forms;
/*Легенда: получив заказ от центра статистических исследований на
разработку единой базы данных, вам потребовалось разработать класс, каждый
объект которого хранит необходимый минимум данных о населенном пункте.
    Данные:
    - название города
    - год основания
    - количество жителей в данный момент
Методы:
- вычислить средний прирост населения в год
Отображение в таблице:
-выделить цветом город с самым высоким темпом роста.
    */

namespace MyClass
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //BindingList - это специальный список List
        //с вызовом события обновления внутреннего состояния,
        //необходимого для автообновления datagridview
        private BindingList<SampleRow> data;

        private void Form1_Load(object sender, EventArgs e)
        {

            //Создание пустого Хранилища
            data = new BindingList<SampleRow>();

            //Создание Объектов и сохранение их в Хранилище
            data.Add(new SampleRow("Saint's Petersburg", 1703, 5598486));
            data.Add(new SampleRow("Moscow", 1147, 13097539));
            data.Add(new SampleRow("Yoshkar-Ola", 1584, 281248));

            //Привязка Хранилища к dataGridView1
            dataGridView1.DataSource = data;

            //Настройка dataGridView1
            dataGridView1.Width = 250; //Ширина 

            //Настройка колонок
            var column1 = dataGridView1.Columns[0];
            column1.HeaderText = "Название"; //текст в шапке
            column1.Width = 100; //ширина колонки
            column1.ReadOnly = true; //значение в этой колонке нельзя править
            column1.Name = "Name"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell(); //тип колонки

            var column2 = dataGridView1.Columns[1];
            column2.HeaderText = "Год основания";
            column2.Width = 50;
            column2.Name = "Year";
            column2.CellTemplate = new DataGridViewTextBoxCell();

            var column3 = dataGridView1.Columns[2];
            column3.HeaderText = "Кол-во жителей сейчас";
            column2.Width = 50;
            column3.Name = "Count";
            column3.CellTemplate = new DataGridViewTextBoxCell();

            //Назначаем метода-обработчика изменения значения ячейки "Цена"
            dataGridView1.CellValueChanged += CellValueChanged;

            //Принудительный вызов обрабочика для отрисовки цвета строк
            CellValueChanged(null, null);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Year = 0;
            int Count = 0;
            string Name = tbName.Text.Trim(); //Обрезка ведущих и хвостовых пробелов

            try
            {
                Year = Int32.Parse(tbPrice.Text);
                Count = Int32.Parse(tbCount.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //Добавляем в Хранилище новый объект
            data.Add(new SampleRow(Name, Year, Count));

            //Принудительный вызов обрабочика для перерисовки цвета строк
            CellValueChanged(null, null);
        }


        /// <summary>
        /// Метод-обработчик отрисовки цвета строк dataGridView1
        /// Должен вызываться при изменении состава строк или изменении цены любого объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null || e.ColumnIndex == 1) //Изменилась цена - переопределить цвет
            {
                //Ищем Объект и строку с максимальной ценой
                float maxYear = 0.0f;
                int maxRow = -1;
                for (int i = 0; i < data.Count; i++)
                    if (data[i].Year > maxYear)
                    {
                        maxYear = data[i].Year;
                        maxRow = i;
                    }

                // Изменение цвета строк
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (i == maxRow)
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Beige;
                    else
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;

                }
            }
        }

        /// <summary>
        /// Удаление выбранной строки данных и Объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDel_Click(object sender, EventArgs e)
        {
            //Номер выбранной строки
            int selRowNum = dataGridView1.SelectedCells[0].RowIndex;
            if (MessageBox.Show("Удалить " + dataGridView1[selRowNum, 0].Value + " ?", "Удаление данных",
                    MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //Удаляем выбранный объект из Хранилища
                data.RemoveAt(selRowNum);
                //Принудительный вызов обрабочика для перерисовки цвета строк
                CellValueChanged(null, null);
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            tbName.Text = data[iRow].Name;
            tbPrice.Text = data[iRow].Year.ToString();
            tbCount.Text = data[iRow].Count.ToString();
        }

        private void btReadXML_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string xmlFile = openFileDialog1.FileName;
                data = XMLClass.ReadFromXmlFile<BindingList<SampleRow>>(xmlFile);
                //Привязка Хранилища к dataGridView1
                dataGridView1.DataSource = data;

                //Принудительный вызов обрабочика для перерисовки цвета строк
                CellValueChanged(null, null);

                this.Text = "Работа с Объектами " + xmlFile;
            }
        }

        private void btWriteXML_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                XMLClass.WriteToXmlFile<BindingList<SampleRow>>(saveFileDialog1.FileName, data);
        }

        private void btnCalculate_Click(object sender, EventArgs e) // Считает прирост населения в год
        {

            var currow = dataGridView1.CurrentCell.RowIndex;
            int curdata; //текущий год нужного нам города
            int timeofentity = 0; //время существования с момента создания города
            int nacelenie = 0; //текущее население
            int prirost; //прирост в год за время существования
            int rowscount;
            int clmnscount;
            int maxprirost; //максимальный прирост в таблице
            int maxprirost_ind;

            curdata = (int)dataGridView1[1, currow].Value;
            if (curdata != 0)
            {
                timeofentity = 2023 - curdata;
                nacelenie = (int)dataGridView1[2, currow].Value;                   //поиск прироста выбранного города
                prirost = nacelenie / timeofentity;

                MessageBox.Show(Convert.ToString(prirost) + "человек в год за " + timeofentity + "лет");
            }

            else
            {
                MessageBox.Show("Что-то пошло не так, попробуйте снова");
            }

            // поиск самого быстрорастущего среди всей таблицы

            rowscount = dataGridView1.RowCount; //максимальное кол-во строк
           

            List<int> a;
            a = new List<int>();

            for (int i = 0; i < rowscount-1; i++) //поиск прироста каждого города в таблице
            {
                var value = dataGridView1[1, i].Value;
                
                
                    curdata = Convert.ToInt32(value);
                    timeofentity = 2023 - curdata;
                    nacelenie = Convert.ToInt32(dataGridView1[2, i].Value);


                    prirost = nacelenie / timeofentity;
                    MessageBox.Show(prirost.ToString());
                    
                    a.Add(prirost);
                    
            }
            
            int b =a.Max();
            MessageBox.Show(b + "- максимальный прирост");
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                
               int f = Convert.ToInt32(row.Cells[2].Value);
                int c =2023- Convert.ToInt32(row.Cells[1].Value);
                if(f==0 || c==0)
                    continue;
                 if((f/c)==b)
                 row.Selected = true;
             }
 


            int Indexof(int[] array, int value) //ф-я нахождения индекса нужного нам значения
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == value)
                        return i;
                }

                return -1;
            }

        }
    }
}
