var map;
var markers = [];
var currentPage = 1;
var currentLimit = 12;
var currentOpportunityType;
var currentTotalCount = 0;

function addOpportunityMarker(opportunity) {
    if (opportunity.latitude && opportunity.longitude) {
        var marker = map.addMarker({
            lat: opportunity.latitude,
            lng: opportunity.longitude,
            title: opportunity.name,
            infoWindow: { content: opportunity.name },
            click: function (e) {
                openOpportunityModal(opportunity);
            }
        });
        markers.push(marker);
        return marker;
    }
};

function openOpportunityModal(opportunity) {
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.categoryName);
    $("#OpportunityModalCause").html(opportunity.causeName);
    $("#OpportunityModalOrganization").html(opportunity.organizationName);
    $("#OpportunityModalOrganizationUrl").attr("href", opportunity.organizationLink);
    $("#ModalAddressHref").attr("href", "https://www.google.com/maps/search/?api=1&query=" + opportunity.address);
    $("#ModalContactEmail").attr("href", "mailto:" + opportunity.contactEmail);
    $("#ModalContactEmail").html(opportunity.contactEmail);
    $("#ModalAddress").html(opportunity.address);
    $("#ModalDescription").html(opportunity.description);
    var opportunityType = opportunity.opportunityType;
    if (opportunityType === 0) {
        $("#occurrencesSelect").css("visibility", "visible");
        $("#occurrencesSelect").css("position", "relative");
    } else {
        $("#occurrencesSelect").css("visibility", "hidden");
        $("#occurrencesSelect").css("position", "absolute");
    }
    $("#occurrencesInput").html(getOccurrenceSelectors(opportunity.occurrenceViews));

    $("#fb-share").attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + baseUrl + opportunity.id);
    $("#tw-share").attr("href", "https://twitter.com/share?url=" + baseUrl + opportunity.id + "&text=Volly - " + opportunity.name);
    document.getElementById("ln-share").innerHTML = baseUrl + opportunity.id;
    $("#OpportunityModal").modal('show');
};

function prettyFormatDateTimes(d1, d2, breakline) {
    var startDateTime = moment(d1, "DD-MM-YYYY H:mm");
    var endDateTime = moment(d2, "DD-MM-YYYY H:mm");
    var dateTimeString = "Coming soon!";
    if (startDateTime.year() >= 1970) {
        var startDateTimeOffset = startDateTime.isDST() ? "-06:00" : "-07:00";
        if (endDateTime.year() >= 1970) {
            var endDateTimeOffset = endDateTime.isDST() ? "-06:00" : "-07:00";
            if (startDateTime.year() === endDateTime.year()
                && startDateTime.month() === endDateTime.month()
                && startDateTime.day() === endDateTime.day()) {
                dateTimeString = startDateTime.utcOffset(startDateTimeOffset).format('ddd MMM D YYYY h:mm a')
                    + " - "
                    + endDateTime.utcOffset(endDateTimeOffset).format('h:mm a');
            } else {
                dateTimeString = startDateTime.utcOffset(startDateTimeOffset).format('ddd MMM D YYYY h:mm a')
                    + getSplit(breakline)
                    + endDateTime.utcOffset(endDateTimeOffset).format('ddd MMM D YYYY h:mm a')
            }
        } else {
            dateTimeString = startDateTime.utcOffset(startDateTimeOffset).format('ddd MMM D YYYY h:mm a');
        }
    }
    return dateTimeString;
}

function getSplit(breakline) {
    if (breakline) {
        return " -<br />";
    }
    return " - ";
}

function appendOpportunityPanel(opportunity, marker) {
    var dateTimeStringWrapper = "";
    if (opportunity.opportunityType === 0) {
        var dateTimeString = "Multiple Shifts";
        if (opportunity.occurrenceViews) {
            if (opportunity.occurrenceViews.length === 1) {
                var firstOccurrence = opportunity.occurrenceViews[0];
                dateTimeString = prettyFormatDateTimes(firstOccurrence.startTime, firstOccurrence.endtime, true);
            } else if (opportunity.occurrenceViews.length === 0) {
                dateTimeString = "Ongoing";
            }
        }
        dateTimeStringWrapper = '<div class="wrap-center"><div class="result-datetime">' + dateTimeString + '</div></div>';
    }

    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-xl-3 col-lg-4 col-md-6 col-sm-12 result-card hide"><div class="result-card-inner">' +
        dateTimeStringWrapper +
        '<div class="img-opp"><img src="' + opportunity.imageUrl + '" /></div>' +
        '<div class="result-details"><div class="result-address">' + opportunity.address + '</div>' +
        '<div class="result-org-name">' + opportunity.organizationName + '</div>' +
        '<div class="result-name">' + opportunity.name + '</div>' +
        '</div></div></div>');
    var imagesTimer = setTimeout("$('#opportunity-" + opportunity.id + "').removeClass('hide')", 5000);
    $("#opportunity-" + opportunity.id + " img").on("load", function () {
        clearTimeout(imagesTimer);
        $("#opportunity-" + opportunity.id).removeClass("hide");
    });
    $("#opportunity-" + opportunity.id).click(function (e) {
        openOpportunityModal(opportunity);
    });

    if (marker) {
        $("#opportunity-" + opportunity.id)
            .hover(function (e) {
                marker.setAnimation(google.maps.Animation.BOUNCE);
            }, function (e) {
                marker.setAnimation(null);
            });
    }

    if ($("#InitialOpportunity").html() == opportunity.id) {
        openOpportunityModal(opportunity);
    }
};

function getOccurrenceSelectors(occurrences) {
    var element = "";
    if (occurrences) {
        for (var i = 0; i < occurrences.length; i++) {
            var selected = "";
            if (i === 0) {
                selected = "selected";
            }
            var occurrence = occurrences[i];
            element = element + "<option value='" + occurrence.id +
                "'" + selected +
                ">" + prettyFormatDateTimes(occurrence.startTime, occurrence.endTime, false);
            if (occurrence.openings && occurrence.openings > 0) {
                element = element +
                    " (" + occurrence.openings + " spot" + (occurrence.openings === 1 ? "" : "s") + " remaining) ";
            }
            element = element + "</option>";
        }
    }
    return element;
}

function initMap() {
    map = new GMaps({
        div: '#map',
        lat: 51.044308,
        lng: -114.0652801,
        zoom: 10
    });
    $('#nothingFoundAlert').hide();
    $('#loader').show();
    $('#searchNearMe').click(function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (p) {
                    map.panTo(new google.maps.LatLng(p.coords.latitude, p.coords.longitude));
                },
                function (e) {
                    alert(e.message);
                },
                { timeout: 10000 }
            );
        } else {
            alert('Geolocation services must be enabled.')
        }
    });

    $('#All').click();
    enableDatePicker();
};

function enableDatePicker() {
    $('#dateSelect').datepicker({
        multidate: true,
        clearBtn: true,
        todayHighlight: true
    });
}

function clearOpportunities() {
    deleteMarkers();
    $("#opportunityList").empty();
};

function addOpportunityMarkers(opportunities) {
    for (var i = 0; i < opportunities.length; i++) {
        var opportunity = opportunities[i];
        var marker = addOpportunityMarker(opportunity);
        appendOpportunityPanel(opportunity, marker);
    }
};

function deleteMarkers() {
    clearMarkers();
    markers = [];
};

function clearMarkers() {
    setMapOnAll(null);
};

function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
};

$(".opportunityType").click(function () {
    var opportunityType = parseInt($(this).attr('value'));
    filter(opportunityType, 1);
});

function filter(opportunityType, page = 1) {
    currentPage = page;
    var opportunityTypeChanged = currentOpportunityType != opportunityType;
    currentOpportunityType = opportunityType;
    var data = {
        "OpportunityType": opportunityType,
        "Page": page,
        "Limit": currentLimit,
        "Sort": 1
    };

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: '/api/Search/Opportunities',
        data: JSON.stringify(data),
        dataType: "json",
        success: function (data) {
            var opportunities = data.opportunities;
            currentTotalCount = data.totalCount;
            if (opportunityTypeChanged) {
                clearOpportunities();
            }
            $('#loader').show();
            if (opportunities.length === 0) {
                $('#loader').hide();
                if (opportunityTypeChanged) {
                    $('#nothingFoundAlert').show();
                }
            } else {
                $('#nothingFoundAlert').hide();
                addOpportunityMarkers(opportunities);
            }
        }
    }).then(() => $('#loader').hide());
}

$("#causes-a").click(function (e) {
    toggleFilterVisibility(e.target.id);
});
$("#categories-a").click(function (e) {
    toggleFilterVisibility(e.target.id);
});
$("#organizations-a").click(function (e) {
    toggleFilterVisibility(e.target.id);
});

function toggleFilterVisibility(filterid) {
    if ($("#" + filterid).hasClass("active")) {
        if ($("#filter-wrapper").hasClass("filter-wrapper-hide")) {
            $("#filter-wrapper").removeClass("filter-wrapper-hide")
            $("#filter-wrapper").addClass("filter-wrapper-show")
        } else {
            $("#filter-wrapper").removeClass("filter-wrapper-show")
            $("#filter-wrapper").addClass("filter-wrapper-hide")
        }
    } else {
        $("#filter-wrapper").removeClass("filter-wrapper-hide")
        $("#filter-wrapper").addClass("filter-wrapper-show")
    }
}

(function () {
    window.addEventListener('scroll', () => {
        const {
            scrollTop,
            scrollHeight,
            clientHeight
        } = document.documentElement;

        const hasMoreResults = (page) => {
            const startIndex = (page - 1) * currentLimit + 1;
            return currentTotalCount === 0 || startIndex < currentTotalCount;
        };

        if (scrollTop + clientHeight >= scrollHeight - 5 && hasMoreResults(currentPage)) {
            currentPage++;
            filter(currentOpportunityType, currentPage);
        }
    }, {
        passive: true
    })
    $("#toggleMap").click(function () {
        var dataShow = parseInt($('#toggleMap').attr('data-show'));
        if (dataShow === 1) {
            $('#map').css('height', '85vh');
        }
        $("#map").animate({
            opacity: dataShow
        }, 500, function () {
            if (dataShow === 1) {
                $("#toggleMap").attr('value', 'Hide Map');
                $("#toggleMap").attr('data-show', '0');
                $("#wrap-main").removeClass('col-lg-12');
                $("#wrap-main").addClass('col-lg-8');
                $("#wrap-main").removeClass('col-md-12');
                $("#wrap-main").addClass('col-md-8');
                $("#searchNearMe").css('bottom', 0);
                $("#searchNearMe").css('display', 'unset');
            } else {
                $("#searchNearMe").css('bottom', 'unset');
                $("#searchNearMe").css('display', 'none');
                $("#toggleMap").attr('value', 'Show Map');
                $("#toggleMap").attr('data-show', '1');
                $("#wrap-main").removeClass('col-lg-8');
                $("#wrap-main").addClass('col-lg-12');
                $("#wrap-main").removeClass('col-md-8');
                $("#wrap-main").addClass('col-md-12');
                $('#map').css('height', 0);
            }
        });
    });
    $("#ln-share").click(function () {
        var lnShare = document.getElementById("ln-share");
        var el = document.createElement("input");
        el.setAttribute("id", "ln-share-clipboard");
        el.setAttribute("type", "text");
        el.setAttribute("value", lnShare.innerHTML);
        lnShare.appendChild(el);
        document.getElementById("ln-share-clipboard").select();
        document.execCommand("copy");
        lnShare.removeChild(el);
        var copyConfirm = document.getElementById("copy-confirm");
        copyConfirm.style.display = 'inline-block';
        setTimeout(() => {
            copyConfirm.style.display = 'none';
        }, 2000);
    })
})();
