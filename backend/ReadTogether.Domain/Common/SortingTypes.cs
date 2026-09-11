using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadTogether.Domain.Common
{
    public class SortingTypes
    {
        public enum OrderBy
        {
            Title,
            Year,
            DateAdded
        }
        public enum OrderDir
        {
            Asc,
            Desc
        }
    }
}