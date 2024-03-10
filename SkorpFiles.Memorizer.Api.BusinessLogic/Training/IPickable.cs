using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal interface IPickable<T>
    {
        bool Consumed { get; }

        T Pick(Random random);
        T PickAndDelete(Random random);
        bool Return(T item);
    }
}
