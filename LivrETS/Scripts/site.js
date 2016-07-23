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
(function ($) {
    /**
     * @author Charles Levesque
     * @description Prevent the submission of a form when pressing the "enter" key.
     */
    $.preventEnterToSubmit = function () {
        function checkEnter(evt) {
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }

        document.onkeypress = checkEnter;
    }

    /**
     * @description Makes the elements passed in parameters the same width by finding
     * the highest width and applying it to the other elements.
     * @param Elements Array of jquery elements or string selectors.
     * @type string or object
     * @author Charles Levesque
     */
    $.sameWidthForElements = function () {
        if (arguments.length <= 0) {
            return
        }

        var elements = []

        $.each(arguments, function (index, value) {
            if (typeof value == "object") {
                elements.push(value)
            } else if (typeof value == "string") {
                elements.push($(value))
            }
        })

        var widths = $.map(elements, function (value, index) {
            return value.width()
        })
        var maxWidth = Math.max.apply(this, widths)

        $.each(elements, function (index, value) { value.width(maxWidth) })
    }

    /**
     * @description Notifies the user of an error with a banner in the top right
     * corner.
     * @param message The message to be displayed.
     * @type string
     * @author Charles Levesque
     */
    $.notifyError = function (message) {
        $.notify({
            icon: "glyphicon glyphicon-remove",
            message: message
        }, {
            type: "danger"
        })
    }
})(jQuery)