using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    /// <summary>
    /// Provides container for wrapping objects. If you need to read by index and then change elements in big collections, use the wrapper not to search the element by index twice.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Wrapper<T>
    {
        public T Value { get; set; }
        public Wrapper(T value) {  Value = value; }
    }
}
