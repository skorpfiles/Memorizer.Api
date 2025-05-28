using Microsoft.EntityFrameworkCore.Metadata;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class RatingComponent(int coordinate, int length) : EntitiesListForWeighedSoftmaxChoice
    {
        public int Coordinate { get; set; } = coordinate;
        public int Length { get; set; } = length;
    }
}
