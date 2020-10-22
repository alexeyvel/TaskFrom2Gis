using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TaskFrom2Gis
{
    /// <summary>
    /// Класс-коллекция для хранения элементов, имеющих уникальный составной ключ [Id, Name].
    /// </summary>
    public class TaskCollection<Id, Name> : IDictionary<Id, Name>
    {
        private List<KeyValuePair<Id, Name>> collectionBase;
        private KeyValuePair<Id, Name> templateItem;
        private ICollection<Id> keys;
        private ICollection<Name> values;

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса TaskCollection, имеющий начальную емкость по умолчанию.
        /// </summary>
        public TaskCollection()
        {
            collectionBase = new List<KeyValuePair<Id, Name>>();
        }

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса TaskCollection, заданную начальную емкость.
        /// </summary>
        /// <param name="capasity">Начальное количество элементов, которое может содержать класс-коллекция.</param>
        public TaskCollection(int capasity)
        {
            if (capasity < 0)
                throw new ArgumentOutOfRangeException(nameof(capasity), "Значение не может быть отрицательным");
            collectionBase = new List<KeyValuePair<Id, Name>>(capasity);
        }

        /// <summary>Получает объект, с помощью которого можно синхронизировать доступ к коллекции.</summary>
        public object SyncRoot
        {
            get { return ((ICollection)collectionBase).SyncRoot; }
        }

        /// <summary>Возвращает значение, показывающее, является ли доступ к коллекции синхронизированным (потокобезопасным).</summary>
        public bool IsSynchronized
        {
            get { return ((ICollection)collectionBase).IsSynchronized; }
        }

        /// <summary>Возвращает число составных ключей [Id, Name], содержащихся в коллекции.</summary>
        public int Count
        {
            get { return collectionBase.Count; }
        }

        public bool IsReadOnly { get; private set; }

        /// <summary> Возвращает или задает значение, связанное с указанным составным ключом.</summary>
        /// <param name="key"> Ключ, значение которого требуется получить или задать.</param>
        /// <returns> Значение, связанное с указанным ключом. Если указанный ключ не найден, операция
        /// получения создает исключение System.Collections.Generic.KeyNotFoundException,
        /// а операция задания значения создает новый элемент с указанным ключом.
        /// </returns>
        public Name this[Id key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    Name val;
                    TryGetValue(key, out val);
                    return val;
                }
                throw new KeyNotFoundException("Данный ключ отсутствует в словаре");
            }
            set
            {
                bool isEqual = false;
                for (int i = 0; i < collectionBase.Count; i++)
                {
                    if (IsEqual(key, collectionBase[i].Key))
                    {
                        collectionBase[i] = new KeyValuePair<Id, Name>(collectionBase[i].Key, value);
                        isEqual = true;
                    }                  
                }
                if (!isEqual)
                {
                    collectionBase.Add(new KeyValuePair<Id, Name>(key, value));
                }
            }
        }

        /// <summary> Возвращает интерфейс ICollection<Id> коллекции, содержащий ключи из словаря.</summary>
        public ICollection<Id> Keys
        {
            get
            {
                keys = new List<Id>();
                foreach (var item in collectionBase)
                    keys.Add(item.Key);
                return keys;
            }
        }

        /// <summary> Возвращает интерфейс ICollection<Name> коллекции, содержащий значения из словаря.</summary>
        public ICollection<Name> Values
        {
            get
            {
                values = new List<Name>();
                foreach (var item in collectionBase)
                    values.Add(item.Value);
                return values;
            }
        }

        /// <summary> Добавляет указанные ключ и значение в класс-коллекцию.</summary>
        /// <param name="key"> Ключ добавляемого элемента.</param>
        /// <param name="value"> Добавляемое значение элемента. Для ссылочных типов допускается значение null.</param>
        public void Add(Id key, Name value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Элемент с таким ключом уже существует", nameof(key));
            collectionBase.Add(new KeyValuePair<Id, Name>(key, value));
        }

        /// <summary>
        /// Добавляет элемент в класс-коллекцию. Добавляемый элемент должен определять пару ключ-значение. 
        /// </summary>
        /// <param name="item">Объект, добавляемый в класс-коллекцию и определяющий внутри себя пару ключ-значение.</param>
        public void Add(KeyValuePair<Id, Name> item)
        {
            if (ContainsKey(item.Key))
                throw new ArgumentException("Элемент с таким ключом уже существует", nameof(item.Key));
            collectionBase.Add(item);
        }

        /// <summary> Удаляет все ключи и значения из класса-коллекции.</summary>
        public void Clear()
        {
            collectionBase.Clear();
        }

        /// <summary> Определяет, содержится ли указанный элемент в классе-коллекции.</summary>
        /// <param name="item"> Объект, который требуется найти.</param>
        /// <returns> true, если класс-коллекция содержит элемент с указанным ключом, в противном случае — false.</returns>
        public bool Contains(KeyValuePair<Id, Name> item)
        {
            Name val;
            return (TryGetValue(item.Key, out val) && val.Equals(item.Value));
        }

        /// <summary> Определяет, содержится ли указанный ключ в классе-коллекции.</summary>
        /// <param name="key"> Ключ, который требуется найти.</param>
        /// <returns> true, если класс-коллекция содержит элемент с указанным ключом, в противном случае — false.</returns>
        public bool ContainsKey(Id key)
        {
            foreach (var item in collectionBase)
            {
                if (IsEqual(key, item.Key))
                {
                    templateItem = item;
                    return true;
                }
            }
            templateItem = new KeyValuePair<Id, Name>(key, default(Name));
            return false;
        }

        /// <summary> Определяет, содержится ли указанное значение в классе-коллекции.</summary>
        /// <param name="value"> Значение, которое требуется найти.</param>
        /// <returns> true, если класс-коллекция содержит указанный объект, в противном случае — false</returns>
        public bool ContainsValue(Name value)
        {
            foreach (var item in collectionBase)
            {
                if (value.Equals(item.Value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Копирует элементы класса-коллекции в существующий одномерный массив System.Array, начиная с указанного значения индекса массива.
        /// </summary>
        /// <param name="array"> Одномерный массив System.Array, в который копируются элементы из коллекции.
        /// Массив System.Array должен иметь индексацию, начинающуюся с нуля.</param>
        /// <param name="arrayIndex"> Отсчитываемый от нуля индекс в массиве array, указывающий начало копирования.</param>
        public void CopyTo(KeyValuePair<Id, Name>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<Id, Name>>)collectionBase).CopyTo(array, arrayIndex);
        }

        /// <summary> Удаляет значение с указанным ключом из класса-коллекции.</summary>
        /// <param name="key"> Ключ удаляемого элемента.</param>
        /// <returns> Значение true, если элемент был найден и удален, в противном случае — значение false.</returns>
        public bool Remove(Id key)
        {
            if (ContainsKey(key))
            {
                collectionBase.Remove(templateItem);
                return true;
            }
            return false;
        }

        /// <summary> Удаляет элемент из класса-коллекции. Удаляемый элемент должен определять пару ключ-значение.</summary>
        /// <param name="item"> Объект, удаляемый из класса-коллекции и определяющий внутри себя пару ключ-значение.</param>
        /// <returns> Значение true, если элемент был найден и удален, в противном случае — значение false.</returns>
        public bool Remove(KeyValuePair<Id, Name> item)
        {
            if (Contains(item))
            {
                collectionBase.Remove(templateItem);
                return true;
            }
            return false;
        }

        /// <summary> Метод возвращает значение, связанное с заданным ключом.</summary>
        /// <param name="key"> Ключ значения, которое необходимо получить.</param>
        /// <param name="value"> Этот метод возвращает значение, связанное с указанным ключом, если он найден,
        /// в противном случае — значение по умолчанию для типа параметра value. Этот параметр
        /// передается неинициализированным.</param>
        /// <returns> true, если класс-коллекция содержит элемент с указанным ключом, в противном случае — false.</returns>
        public bool TryGetValue(Id key, out Name value)
        {
            if (ContainsKey(key))
            {
                value = templateItem.Value;
                return true;
            }
            value = default(Name);
            return false;
        }

        /// <summary>. Возвращает перечислитель, осуществляющий перебор элементов списка класса-коллекции.</summary>
        /// <returns> Перечислитель, который можно использовать для итерации по коллекции.</returns>
        public IEnumerator<KeyValuePair<Id, Name>> GetEnumerator()
        {
            return collectionBase.GetEnumerator();
        }

        /// <summary>. Возвращает перечислитель, осуществляющий перебор элементов списка класса-коллекции.</summary>
        /// <returns> Перечислитель, который можно использовать для итерации по коллекции.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Метод производящий сравнение полей и свойств двух объектов по значению при ппомощи рефлексии дерева выражений
        /// класса Expression.
        /// </summary>
        /// <param name="obj1"> Первый сравниваемый объект</param>
        /// <param name="obj2"> Второй сравниваемый объект</param>
        /// <returns> true, если поля и свойства в сравниваемых объектах равны, в противном случае — false.</returns>
        private bool IsEqual(Id obj1, Id obj2)
        {
            var parameters = new[] { Expression.Parameter(typeof(Id), "x"), Expression.Parameter(typeof(Id), "y") };
            Expression body = Expression.Constant(true, typeof(bool));
            var memberTypes = new[] { MemberTypes.Field, MemberTypes.Property };
            foreach (var member in typeof(Id).GetMembers().Where(m => memberTypes.Contains(m.MemberType)))
                body = Expression.AndAlso(body,
                    Expression.Equal(
                        Expression.MakeMemberAccess(parameters[0], member),
                        Expression.MakeMemberAccess(parameters[1], member)));
            var lambdaFunc = Expression.Lambda<Func<Id, Id, bool>>(body, parameters).Compile();
            return lambdaFunc(obj1, obj2);
        }
    }
}
