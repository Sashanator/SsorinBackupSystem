using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork_4__REWORKED_
{
    interface IDeletable<T>
    {
        List<int> DeletePointsBy(T obj);
    }
}
