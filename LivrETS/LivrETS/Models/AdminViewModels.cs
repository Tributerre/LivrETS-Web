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
using LivrETS.Models;

namespace LivrETS.Models
{
    public class AjaxManageUsersViewModel
    {
        private string _newRole;
        public string NewRole
        {
            get
            {
                return _newRole;
            }
            set
            {
                _newRole = (value == "User") ? null : value;
            }
        }

        public string UserId { get; set; }
        public string UserIds { get; set; }
        public List<string> UserIdsList
        {
            get
            {
                return UserIds.Split(',').ToList();
            }
        }
    }

    public class AjaxFairViewModel
    {
        public string Id { get; set; }
        public string Ids { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime PickingStartDate { get; set; }
        public DateTime PickingEndDate { get; set; }

        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }

        public DateTime RetrievalStartDate { get; set; }
        public DateTime RetrievalEndDate { get; set; }

        public List<string> IdsList
        {
            get
            {
                return Ids.Split(',').ToList();
            }
        }

        private string _trimester;
        public string Trimester
        {
            get
            {
                return _trimester;
            }
            set
            {
                switch (value)
                {
                    case "A":
                        _trimester = Models.Trimester.AUTUMN;
                        break;

                    case "E":
                        _trimester = Models.Trimester.SUMMER;
                        break;

                    case "H":
                        _trimester = Models.Trimester.WINTER;
                        break;
                }
            }
        }
    }
}