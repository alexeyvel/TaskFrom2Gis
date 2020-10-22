using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaskFrom2Gis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Создаем уникальные ключи для добавления в класс-коллекцию:
                var key_1 = new UserType(DayOfWeek.Thursday, new DateTime(2020, 10, 22), "Работать");
                var key_2 = new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 08, 30, 0), "Работать");
                var key_3 = new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 12, 30, 0), "Обедать");
                var key_4 = new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 13, 30, 0), "Прокрастинировать - пятница же");
                var key_5 = new KeyValuePair<UserType, string>(key_1, "Должен выдать исключение");
                var key_6 = new KeyValuePair<UserType, string>(new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 08, 30, 0), "Работать"), "Должен выдать исключение");
                var key_7 = new UserType(DayOfWeek.Thursday, new DateTime(2020, 10, 22), "Работать");

                // Создаем экземпляр класса-коллекции TaskCollection и заполняем его:
                var myStore = new TaskCollection<UserType, string>();
                myStore.Add(key_1, "Успешно");
                myStore.Add(key_2, "Успешно");
                myStore.Add(key_3, "Успешно");
                myStore.Add(key_4, "Успешно");
                myStore.Add(new UserType(DayOfWeek.Thursday, DateTime.Now, "Написать примеры"), "Успешно");

                // Если расскоментировать хотя бы одну строчку кода ниже - получим Exception
                //myStore.Add(key_5);
                //myStore.Add(key_6);
                //myStore.Add(key_7, "Должен выдать исключение");

                // Проверим, что добавилось в нашу коллекию:
                ShowDetailCollections(myStore, ConsoleColor.DarkGreen, "Добавление элементов");           

                // Удалим пару элементов
                myStore.Remove(new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 13, 30, 0), "Прокрастинировать - пятница же"));
                myStore.Remove(key_1);

                // Проверим, что получилось:
                ShowDetailCollections(myStore, ConsoleColor.DarkYellow, "Удаление элементов");          

                //Поменяем для какоголибо Id значение Name в нашей коллекции
                myStore[new UserType(DayOfWeek.Friday, new DateTime(2020, 10, 23, 08, 30, 0), "Работать")] = "Какое то новое значение для Name";

                // Проверим, что добавилось в нашу коллекию:
                ShowDetailCollections(myStore, ConsoleColor.White, "Выбор элемента по ключу и замена у него значения Name");      
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static void ShowDetailCollections(TaskCollection<UserType, string> collections, ConsoleColor fontColor, String title = "")
        {
            Console.ForegroundColor = fontColor;
            Console.WriteLine(title);
            foreach (var item in collections.Select((value, i) => new { i, value }))
            {
                var value = item.value;
                var index = item.i + 1;
                Console.WriteLine($"{index} элемент коллекции: {value}, в составе:");
                Console.WriteLine($"\tId:\n\t  {{");
                Console.WriteLine($"\t\t{nameof(value.Key.Day)} = {(value.Key.Day)};");
                Console.WriteLine($"\t\t{nameof(value.Key.Time)} = {(value.Key.Time)};");
                Console.WriteLine($"\t\t{nameof(value.Key.Description)} = {(value.Key.Description)};");
                Console.WriteLine($"\t  }}");
                Console.WriteLine($"\tName:\n\t  {{");
                Console.WriteLine($"\t\t{nameof(value.Value)} = {(value.Value)};");
                Console.WriteLine($"\t  }}");
            }
            Console.ResetColor();
        }
    }
}
