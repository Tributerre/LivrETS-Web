/*
LivrETS - Centralized system that manages selling of pre-owned ETS goods.
Copyright (C) 2016  TribuTerre

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/> 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivrETS.Models
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }

        [ForeignKey(nameof(Fair))]
        public Guid FairID { get; set; }
        public virtual Fair Fair { get; set; }

        [ForeignKey(nameof(Seller))]
        public string SellerID { get; set; }
        public virtual ApplicationUser Seller { get; set; }

        [NotMapped]
        public double Subtotal => SaleItems.Sum(item => item.Offer.Price);
        [NotMapped]
        public double Total => Subtotal + TotalCommission;
        [NotMapped]
        public double TotalCommission => Subtotal + (Fair?.CommissionOnSale ?? 0d);

        public Sale()
        {
            Date = DateTime.Now;
            SaleItems = new List<SaleItem>();
        }
    }
}