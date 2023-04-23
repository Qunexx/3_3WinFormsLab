using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyClass
{
    //Класс для описания товаров с параметрами:
    //  Название, Цена, Остаток
    public class SampleRow
    {
        public string Name { get; set; } //обязательно нужно использовать get;set конструкции
        public int Year { get; set; }
        public int Count { get; set; }

        public string Hidden = ""; //Данное свойство не будет отображаться как колонка

        public SampleRow(string name, int year, int count)
        {
            this.Name = name;
            this.Year = year;
            this.Count = count;
        }

        public SampleRow()
        { }
    }
}
