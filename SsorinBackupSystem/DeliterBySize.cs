using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWork_4__REWORKED_.Deliters
{
    public class DeliterBySize : IDelitable<double>
    {
        public List<int> DeleteByCriterion(double kb, List<RestorePoint> Points)
        {
            List<int> pointsToSave = new List<int>();
            double size = 0;

            for (int i = Points.Count - 1; i >= 0; i--) {
                if (size < kb) {
                    pointsToSave.Add(Points[i].ID);
                    size += Points[i].PointSize;
                }
            }
            if (size > kb) { // Если размер превышает
                pointsToSave.Remove(pointsToSave.Last()); // то удаляем последний элемент
            }
            // Check
            List<int> pointsToDelete = new List<int>();
            foreach (var p in Points) {
                if (!pointsToSave.Contains(p.ID)) {
                    pointsToDelete.Add(p.ID);
                }
            }
            return pointsToDelete;
        }
    }
}
