namespace XMLValidator.Utils
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    #endregion

    public class Mappings<T1, T2> : IEnumerable<Tuple<T1, T2>>
    {
        public Mappings()
        {
            this.ItemList1 = new List<T1>();
            this.ItemList2 = new List<T2>();
            this._I1_I2_Mappings = new Dictionary<T1, T2>();
            this._I2_I1_Mappings = new Dictionary<T2, T1>();
            this.indexMappings = new Dictionary<Tuple<T1, T2>, int>();
            this.Count = 0;
        }

        public Mappings(IEnumerable<Tuple<T1, T2>> tuples)
            : this()
        {
            foreach (var tuple in tuples)
            {
                this.Add(tuple.Item1, tuple.Item2);
            }
        }

        public int Count { get; set; }
        public List<T1> ItemList1 { get; set; }
        public List<T2> ItemList2 { get; set; }

        public IEnumerator<Tuple<T1, T2>> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return new Tuple<T1, T2>(this.ItemList1[i], this.ItemList2[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T1 item1, T2 item2)
        {
            this.ItemList1.Add(item1);
            this.ItemList2.Add(item2);
            this._I1_I2_Mappings.Add(item1, item2);
            this._I2_I1_Mappings.Add(item2, item1);
            this.indexMappings.Add(new Tuple<T1, T2>(item1, item2), this.Count);
            this.Count++;
        }

        public Tuple<T1, T2> GetMappingByItem1(T1 item1)
        {
            return new Tuple<T1, T2>(item1, this._I1_I2_Mappings[item1]);
        }

        public Tuple<T1, T2> GetMappingByItem2(T2 item2)
        {
            return new Tuple<T1, T2>(this._I2_I1_Mappings[item2], item2);
        }

        public int GetIndex(Tuple<T1, T2> tuple)
        {
            if (!tuple.Item1.GetType().IsPrimitive ||
                !tuple.Item2.GetType().IsPrimitive)
            {
                throw new NotSupportedException("Not supported");
            }

            if (this.indexMappings.ContainsKey(tuple))
            {
                return this.indexMappings[tuple];
            }
            else
            {
                return -1;
            }
        }

        public Tuple<T1, T2> this[int index]
        {
            get
            {
                return new Tuple<T1, T2>(this.ItemList1[index], this.ItemList2[index]);
            }
        }

        public T2 this[T1 item1]
        {
            get
            {
                return this._I1_I2_Mappings[item1];
            }
            set
            {
                this._I1_I2_Mappings[item1] = value;
                int index = this.GetIndex(new Tuple<T1, T2>(item1, value));
                this.ItemList2[index] = value;
            }
        }

        public T1 this[T2 item2]
        {
            get
            {
                return this._I2_I1_Mappings[item2];
            }
            set
            {
                this._I2_I1_Mappings[item2] = value;
                int index = this.GetIndex(new Tuple<T1, T2>(value, item2));
                this.ItemList1[index] = value;
            }
        }

        private Dictionary<T1, T2> _I1_I2_Mappings;
        private Dictionary<T2, T1> _I2_I1_Mappings;
        private Dictionary<Tuple<T1, T2>, int> indexMappings;
    }
}
