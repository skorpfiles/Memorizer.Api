using Microsoft.EntityFrameworkCore.Metadata;
using SkorpFiles.Memorizer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.BusinessLogic.Training
{
    internal class RatingComponent:EntitiesListForRandomChoice<Question>
    {
        public int Coordinate { get; set; }
        public int Length { get; set; }

        public RatingComponent(int coordinate, int length) 
        { 
            Coordinate = coordinate;
            Length = length; 
        }
    }
}
