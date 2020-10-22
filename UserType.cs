using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFrom2Gis
{
    /// <summary>
    /// Класс пользовательского типа данных (служит ключем (Id) для нашего класса-коллекции)
    /// </summary>
    class UserType
    {
        private string description;
        public DayOfWeek Day { get; set; }
        public DateTime Time { get; set; }
        public string Description { get => description; set => description = value ?? throw new ArgumentNullException(nameof(value), "Ссылка на объект не указывает на экземпляр объекта"); }

        public UserType(DayOfWeek day, DateTime time, string description)
        {
            Day = day;
            Time = time;
            Description = description;
        }
    }
}
