using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LivrETS.Models
{
    public class Calculator
        : Article
    {
        [Required]
        public CalculatorModel Model { get; set; }

        public Calculator()
            : base(articleCode: "C")
        {
            Model = CalculatorModel.NSPIRE;
        }
    }


    public enum CalculatorModel
    {
        VOYAGE200 = 0,
        NSPIRE
    }
}