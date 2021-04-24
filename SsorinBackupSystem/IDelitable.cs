using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork_4__REWORKED_
{
    public interface IDelitable<T>
    {
        List<int> DeleteByCriterion(T criterion, List<RestorePoint> points);
    }
}
