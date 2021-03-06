﻿
using System.Collections.Generic;

namespace Foxsys.ApartmentBuilder
{ 
    public class CyclicalList<T> : List<T>
    {
        public new T this[int index]
        {
            get
            {
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                return base[index];
            }
            set
            {
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                base[index] = value;
            }
        }

        public new void RemoveAt(int index)
        {
            Remove(this[index]);
        }
    }
}