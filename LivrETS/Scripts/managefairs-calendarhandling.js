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
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
var fairId = null;
var startDate = null,
    endDate = null,
    pickingStartDate = null,
    pickingEndDate = null,
    saleStartDate = null,
    saleEndDate = null,
    retrievingStartDate = null,
    retrievingEndDate = null;

var fairEvent = null,
    pickingEvent = null,
    saleEvent = null,
    retrievalEvent = null;

var START_DATE_ID = "dp-start-date",
    END_DATE_ID = "dp-end-date",
    PICKING_START_DATE_ID = "dp-picking-start-date",
    PICKING_END_DATE_ID = "dp-picking-end-date",
    SALE_START_DATE_ID = "dp-sale-start-date",
    SALE_END_DATE_ID = "dp-sale-end-date",
    RETRIEVAL_START_DATE_ID = "dp-retrieval-start-date",
    RETREIVAL_END_DATE_ID = "dp-retrieval-end-date";

$(document).ready(function () {
    $(".date").datetimepicker({
        locale: "fr-ca",
        format: "DD-MM-YYYY",
        minDate: new Date()
    });
    $(".date").each(function () {
        $(this).data("DateTimePicker").date(null);
    });

    $("#NewFairModal").on("shown.bs.modal", function () {
        $("#calendar").fullCalendar({
            lang: "fr-ca",
            allDayDefault: true
        });
        $("#calendar").fullCalendar("today");
    });

    $(".date").on("dp.change", function (date, oldDate) {
        var pickerId = $(this).attr("id");
        var events = [];

        switch (pickerId) {
            case START_DATE_ID:
                // When event is triggered manually, date.date contains the called element instead
                // of the new date. startDate will already be initialized. It is in this case an edit
                // of a fair.
                if (date.date !== undefined) {
                    startDate = date.date.startOf("day");
                }
                break;

            case END_DATE_ID:
                endDate = date.date.startOf("day").add(1, 'days').subtract(1,'seconds');
                break;

            case PICKING_START_DATE_ID:
                pickingStartDate = date.date.startOf("day");
                break;

            case PICKING_END_DATE_ID:
                pickingEndDate = date.date.startOf("day").add(1, 'days').subtract(1, 'seconds');;
                break;

            case SALE_START_DATE_ID:
                saleStartDate = date.date.startOf("day");
                break;

            case SALE_END_DATE_ID:
                saleEndDate = date.date.startOf("day").add(1, 'days').subtract(1, 'seconds');;
                break;

            case RETRIEVAL_START_DATE_ID:
                retrievingStartDate = date.date.startOf("day");
                break;

            case RETREIVAL_END_DATE_ID:
                retrievingEndDate = date.date.startOf("day").add(1, 'days').subtract(1, 'seconds');;
                break;
        }

        if (startDate !== null && endDate !== null) {
            fairEvent = {
                title: "Foire aux livres",
                start: startDate.toDate(),
                end: endDate.clone().add(1, "days").toDate(),
                color: "blue"
            };
            events.push(fairEvent);
        }

        if (pickingStartDate !== null && pickingEndDate !== null) {
            pickingEvent = {
                title: "Cueillette",
                start: pickingStartDate.toDate(),
                end: pickingEndDate.clone().add(1, "days").toDate(),
                color: "green"
            };
            events.push(pickingEvent);
        }

        if (saleStartDate !== null && saleEndDate !== null) {
            saleEvent = {
                title: "Vente",
                start: saleStartDate.toDate(),
                end: saleEndDate.clone().add(1, "days").toDate(),
                color: "yellow",
                textColor: "black"
            };
            events.push(saleEvent);
        }

        if (retrievingStartDate !== null && retrievingEndDate !== null) {
            retrievalEvent = {
                title: "Récupération",
                start: retrievingStartDate.toDate(),
                end: retrievingEndDate.clone().add(1, "days").toDate(),
                color: "red"
            };
            events.push(retrievalEvent);
        }

        $("#calendar").fullCalendar("removeEvents");
        $("#calendar").fullCalendar("addEventSource", events);
        
        if (startDate !== null && endDate !== null) {
            $("#calendar").fullCalendar("gotoDate", startDate);
        }

        if (fairEvent !== null && pickingEvent !== null && saleEvent !== null && retrievalEvent !== null) {
            $("#btn-save-fair").prop("disabled", false);
        } else {
            $("#btn-save-fair").prop("disabled", true);
        }
    });

    $("#btn-save-fair").on("click", function () {
        $("#save-spinner").spinner({
            radius: 15
        });
        $("#btn-save-fair,#btn-close-modal").each(function () {
            $(this).prop("disabled", true);
        });
        $.ajax({
            method: "POST",
            contentType: "application/json",
            dataType: "json",
            url: "/Admin/Fair",
            data: JSON.stringify({
                Id: fairId,
                StartDate: startDate.toDate(),
                EndDate: endDate.toDate(),
                PickingStartDate: pickingStartDate.toDate(),
                PickingEndDate: pickingEndDate.toDate(),
                SaleStartDate: saleStartDate.toDate(),
                SaleEndDate: saleEndDate.toDate(),
                RetrievalStartDate: retrievingStartDate.toDate(),
                RetrievalEndDate: retrievingEndDate.toDate(),
                Trimester: $("select[name='select-trimester'] option:selected").val()
            }),
            success: function () {
                location.reload();
            },
            error: function () {
            }
        });
    });
});
