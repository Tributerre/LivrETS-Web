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
using System.Linq;
using System.Web;
using LivrETS.Models;

namespace LivrETS.Repositories
{
    /// <summary>
    /// Only repository of the system for now. Add a new one if needed.
    /// </summary>
    public class LivrETSRepository : IDisposable
    {
        private ApplicationDbContext _db;

        public LivrETSRepository()
        {
            _db = new ApplicationDbContext();
        }

        /// <summary>
        /// Returns an enumerable of all courses in the system.
        /// </summary>
        /// <returns>An enumerable of Course. Not Null.</returns>
        public IEnumerable<Course> GetAllCourses()
        {
            return (
                from course in _db.Courses
                orderby course.Acronym ascending
                select course
            );
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}