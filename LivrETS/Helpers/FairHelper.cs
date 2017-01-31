using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivrETS.Helpers
{
    public class FairHelper
    {
        /// <summary>
        /// Get selector edit form fair 
        /// </summary>
        /// /// <param name="text">phase</param>
        /// /// <returns>HtmlString</returns>
        public static IHtmlString GetSelectPhaseFairStep (string phase)
        {
            //string select = 
            string html = "<select class='form-control input-sm activity_activity'>";

            if(phase == "S")
            {
                html += "<option value='P'>Ceuillete</option>";
                html += "<option value='S' selected >Vente</option>";
                html += "<option value='R'>Récuperation</option>";
            }else if(phase == "R")
            {
                html += "<option value='P'>Ceuillete</option>";
                html += "<option value='S'>Vente</option>";
                html += "<option value='R' selected>Récuperation</option>";
            }
            else
            {
                html += "<option value='P' selected>Ceuillete</option>";
                html += "<option value='S'>Vente</option>";
                html += "<option value='R'>Récuperation</option>";
            }
            

            html += "</select>";

            return new HtmlString(String.Format(html));
        }

        /// <summary>
        /// Get selector trimester edit form fair 
        /// </summary>
        /// /// <param name="text">url parameter</param>
        /// <returns>HtmlString</returns>
        public static IHtmlString GetSelectTrimesterFairStep(string trimester)
        {
            string html = "<select class='form-control input-sm' id='session'>";

            if (trimester == "H")
            {
                html += "<option value='A'>Automne</option>";
                html += "<option value='H' selected >Hiver</option>";
                html += "<option value='E'>Été</option>";
            }
            else if (trimester == "E")
            {
                html += "<option value='A'>Automne</option>";
                html += "<option value='H'>Hiver</option>";
                html += "<option value='E' selected>Été</option>";
            }
            else
            {
                html += "<option value='A' selected>Automne</option>";
                html += "<option value='H'>Hiver</option>";
                html += "<option value='E'>Été</option>";
            }

            html += "</select>";

            return new HtmlString(String.Format(html));
        }
    }
}