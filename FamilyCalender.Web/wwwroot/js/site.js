
function setModalValues(button) {
    var selectedDate = button.getAttribute("data-selected-date");
    var memberId = button.getAttribute("data-member-id");
    var isEditable = button.getAttribute("data-editable") === "true";

    var dateInput = document.getElementById("modalSelectedDate");

    if (selectedDate) {
        dateInput.value = selectedDate;
    } else {
        dateInput.value = "";
    }

    //dateInput.readOnly = !isEditable;

    document.querySelectorAll('.member-checkbox').forEach(checkbox => {
        checkbox.checked = checkbox.value === memberId;
    });

    var modal = new bootstrap.Modal(document.getElementById('eventModal'));
    modal.show();
}

$('#eventModal').on('hidden.bs.modal', function () {
    document.getElementById("modalSelectedDate").value = '';
    document.getElementById("modalEndDate").value = '';
    document.getElementById("eventTitle").value = '';

    document.querySelectorAll('.form-check-input').forEach(checkbox => checkbox.checked = false);

    document.querySelectorAll('.text-danger').forEach(error => error.classList.add('d-none'));

    document.querySelectorAll('.form-control').forEach(input => input.classList.remove('is-invalid'));

    const collapseOne = $('#collapseOne');
    if (collapseOne.hasClass('show')) {
        collapseOne.collapse('hide');
    }

    document.body.classList.remove('modal-open');
    document.querySelectorAll('.modal-backdrop').forEach(b => b.remove());
});


// #region validations 

function validateForm() {
    resetValidation();
    const repetitionType = document.getElementById("repetitionSelect").value;

    const isTitleValid = validateEventTitle();

    const areMembersValid = validateMembers();

    const isInterval = isIntervalSelected();
    const areDatesValid = validateDates(isInterval);
    const isRepetitionValid = validateRepetition();
    const isIntervalValid = isInterval && repetitionType === "Custom" ? validateInterval() : true;
    const areWeekdaysValid = isInterval && repetitionType === "Custom" ? validateWeekdaysWithinInterval() : true;

    if (!(isTitleValid && areMembersValid && areDatesValid && isIntervalValid && areWeekdaysValid && isRepetitionValid)) {
        return false;
    }

    return true;
}
function validateRepetition() {
    const repetitionType = document.getElementById("repetitionSelect").value;
    const endDate = document.getElementById("modalEndDate").value;
    const selectedDays = document.querySelectorAll(".day-checkbox:checked");

    let isValid = true;

    document.getElementById("error-end-date").classList.add("d-none");
    document.getElementById("error-right-weekdays").classList.add("d-none");

    if (repetitionType !== "None") {
        if (!endDate) {
            document.getElementById("error-end-date").textContent = "Du måste ange ett slutdatum.";
            document.getElementById("error-end-date").classList.remove("d-none");
            isValid = false;
        }

        if (repetitionType === "Custom" && selectedDays.length === 0) {
            document.getElementById("error-right-weekdays").textContent = "Välj minst en veckodag.";
            document.getElementById("error-right-weekdays").classList.remove("d-none");
            isValid = false;
        }
    }

    return isValid;
}
function resetValidation() {
    document.querySelectorAll('.text-danger').forEach(error => {
        error.classList.add('d-none');
    });

    document.querySelectorAll('.form-control').forEach(input => {
        input.classList.remove('is-invalid');
    });
}
function isIntervalSelected() {
    const repetitionSelect = document.getElementById("repetitionSelect");
    const selectedValue = repetitionSelect.value;

    return selectedValue !== "None";
}
function validateDates(isInterval) {
    let isValid = true;

    if (isInterval) {
        const isStartDateValid = validateDateField('modalSelectedDate', 'error-selected-date');
        const isEndDateValid = validateDateField('modalEndDate', 'error-end-date');
        const isDateRangeValid = validateDateRange('modalSelectedDate', 'modalEndDate', 'error-end-date');

        isValid = isStartDateValid && isEndDateValid && isDateRangeValid;
    } else {
        const isStartDateValid = validateDateField('modalSelectedDate', 'error-selected-date');
        if (!isStartDateValid) {
            isValid = false;
        }
    }

    return isValid;
}


function validateDateField(dateFieldId, errorFieldId) {
    const dateValue = document.getElementById(dateFieldId).value;
    const dateError = document.getElementById(errorFieldId);

    if (dateValue === "") {
        dateError.classList.remove("d-none");
        document.getElementById(dateFieldId).classList.add("is-invalid");
        return false;
    }

    dateError.classList.add("d-none");
    document.getElementById(dateFieldId).classList.remove("is-invalid");
    return true;
}
function validateInterval() {
    const startDate = document.getElementById("modalSelectedDate");
    const endDate = document.getElementById("modalEndDate");
    const selectedDays = document.querySelectorAll('.day-checkbox:checked');

    const startDateValue = startDate.value;
    const endDateValue = endDate.value;
    const today = new Date().toISOString().split("T")[0];

    const startDateError = document.getElementById("error-selected-date");
    const endDateError = document.getElementById("error-end-date");
    const selectedDaysError = document.getElementById('error-interval-weekdays');

    let isValid = true;


    if (!startDateValue) {
        startDateError.textContent = "Startdatum krävs.";
        startDateError.classList.remove("d-none");
        isValid = false;
    }

    if (!endDateValue) {
        endDateError.textContent = "Slutdatum krävs.";
        endDateError.classList.remove("d-none");
        isValid = false;
    } else if (endDateValue <= startDateValue) {
        endDateError.textContent = "Slutdatum måste vara efter startdatum.";
        endDateError.classList.remove("d-none");
        isValid = false;
    }

    if (selectedDays.length < 1) {
        selectedDaysError.textContent = 'Välj minst en veckodag';
        selectedDaysError.classList.remove("d-none");
        isValid = false;
    }

    return isValid;
}
function validateWeekdaysWithinInterval() {
    const startDateValue = document.getElementById("modalSelectedDate").value;
    const endDateValue = document.getElementById("modalEndDate").value;
    const selectedDaysError = document.getElementById("error-right-weekdays");

    if (!startDateValue || !endDateValue) {
        selectedDaysError.classList.add("d-none");
        return true;
    }

    const validWeekdaysSet = new Set(["MÅNDAG", "TISDAG", "ONSDAG", "TORSDAG", "FREDAG", "LÖRDAG", "SÖNDAG"]);
    const selectedDays = Array.from(document.querySelectorAll(".day-checkbox:checked"))
        .map(checkbox => checkbox.value.trim().toUpperCase())
        .filter(day => validWeekdaysSet.has(day));

    const startDate = new Date(startDateValue);
    const endDate = new Date(endDateValue);

    const validWeekdays = new Set();
    for (let date = new Date(startDate); date <= endDate; date.setDate(date.getDate() + 1)) {
        const weekday = date.toLocaleDateString("sv-SE", { weekday: "long" }).toUpperCase();
        validWeekdays.add(weekday);
    }

    const invalidDays = selectedDays.filter(day => !validWeekdays.has(day));

    if (invalidDays.length > 0) {
        selectedDaysError.textContent = `Följande dagar finns inte i intervallet: ${invalidDays.join(", ")}.`;
        selectedDaysError.classList.remove("d-none");
        return false;
    }

    selectedDaysError.classList.add("d-none");
    return true;
}
function validateEventTitle() {
    const eventTitleInput = document.getElementById("eventTitle");
    const eventTitleError = document.getElementById("eventTitleError");
    const eventTitle = eventTitleInput.value.trim();

    if (eventTitle === "") {
        eventTitleError.textContent = 'Ange titel';
        eventTitleError.classList.remove("d-none");
        eventTitleInput.classList.add("is-invalid");
        return false;
    }

    return true;
}
function validateMembers() {
    const checkboxes = document.querySelectorAll(".member-checkbox:checked");
    const memberError = document.getElementById("memberError");
    let isAnyChecked = false;

    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            isAnyChecked = true;
        }
    });

    if (!isAnyChecked) {
        memberError.textContent = 'Du måste välja minst en medlem';
        memberError.classList.remove("d-none");
        return false;
    }

    return true;
}
function validateDateRange(startDateId, endDateId, errorFieldId) {
    const startDateValue = document.getElementById(startDateId).value;
    const endDateValue = document.getElementById(endDateId).value;
    const dateError = document.getElementById(errorFieldId);

    let isValid = true;

    if (startDateValue && endDateValue) {
        if (endDateValue < startDateValue) {
            dateError.textContent = "Slutdatum måste vara större än startdatum.";
            dateError.classList.remove("d-none");
            isValid = false;
        } else {
            dateError.classList.add("d-none");
        }
    }

    return isValid;
}

// #endregion

// #region validation-edit-event

function validateEditForm() {
    resetValidationEdit();

    return validateEventTitleEdit();
}

function resetValidationEdit() {
    document.querySelectorAll('.text-danger').forEach(error => {
        error.classList.add('d-none');
    });

    document.querySelectorAll('.form-control').forEach(input => {
        input.classList.remove('is-invalid');
    });
}

function validateEventTitleEdit() {
    const eventTitleInput = document.getElementById("editEventTitle");
    const eventTitleError = document.getElementById("editEventTitleError");
    const eventTitle = eventTitleInput.value.trim();

    if (eventTitle === "") {
        //eventTitleError.textContent = 'Måste ha en titel';
        eventTitleError.classList.remove("d-none");
        eventTitleInput.classList.add("is-invalid");
        return false;
    }

    return true;
}


// #endregion

// #region handle-members-in-new-calendar

var members = [];
function addMember() {
    var memberName = document.getElementById('memberName').value;

    if (memberName) {
        members.push(memberName);

        document.getElementById('memberName').value = '';

        updateMemberList();

        updateHiddenInputs();
    }
}
function removeMember(index) {
    members.splice(index, 1);

    updateMemberList();
    updateHiddenInputs();
}
function updateMemberList() {
    var memberList = document.getElementById('memberList');
    memberList.innerHTML = '';

    members.forEach(function (member, index) {
        var li = document.createElement('li');
        li.classList.add('list-group-item');
        li.textContent = member;

        var removeBtn = document.createElement('button');
        removeBtn.textContent = '✖';
        removeBtn.classList.add('btn', 'btn-sm', 'float-end');
        removeBtn.onclick = function () { removeMember(index); };

        li.appendChild(removeBtn);
        memberList.appendChild(li);
    });
}
function updateHiddenInputs() {
    var hiddenMembersInputs = document.getElementById('hiddenMembersInputs');
    hiddenMembersInputs.innerHTML = '';

    members.forEach(function (member, index) {
        var input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'Members[' + index + '].Name';
        input.value = member;
        hiddenMembersInputs.appendChild(input);
    });
}

// #endregion

function updateDayNames() {
    let elements = document.querySelectorAll("span[data-full]");

    elements.forEach(el => {
        if (window.innerWidth < 576) {
            el.textContent = el.getAttribute("data-short");
        } else {
            el.textContent = el.getAttribute("data-full");
        }
    });
}

updateDayNames();

window.addEventListener("resize", updateDayNames);


// #region swipe-for-change-month
//document.addEventListener("DOMContentLoaded", function () {
//    let touchStartX = 0;
//    let touchEndX = 0;

//    function startSwipe(event) {
//        touchStartX = event.touches ? event.touches[0].clientX : event.clientX;
//    }

//    function endSwipe(event) {
//        touchEndX = event.changedTouches ? event.changedTouches[0].clientX : event.clientX;
//        handleSwipe();
//    }

//    function handleSwipe() {
//        let swipeDistance = touchEndX - touchStartX;
//        let threshold = window.innerWidth <= 500 ? window.innerWidth * 0.33 : window.innerWidth * 0.1; // krävs svep 33% på liten skärm, 10% på stor skärm


//        if (swipeDistance > threshold) {
//            changeMonth(-1);
//        } else if (swipeDistance < -threshold) {
//            changeMonth(1);
//        }
//    }

//    function changeMonth(delta) {
//        let url = new URL(window.location.href);
//        let month = parseInt(url.searchParams.get("month")) || new Date().getMonth() + 1;
//        let year = parseInt(url.searchParams.get("year")) || new Date().getFullYear();
//        let calendarId = url.searchParams.get("calendarId");

//        month += delta;
//        if (month < 1) {
//            month = 12;
//            year--;
//        } else if (month > 12) {
//            month = 1;
//            year++;
//        }

//        url.searchParams.set("month", month);
//        url.searchParams.set("year", year);
//        if (calendarId) {
//            url.searchParams.set("calendarId", calendarId);
//        }

//        window.location.href = url.toString();
//    }

//    document.addEventListener("touchstart", startSwipe);
//    document.addEventListener("touchend", endSwipe);

//});

// #endregion

function CopyShareLink()
{
    // Get the text field
    var copyText = document.getElementById("ShareLinkInput");

    // Select the text field
    copyText.select();
    copyText.setSelectionRange(0, 99999); // For mobile devices

    // Copy the text inside the text field
    navigator.clipboard.writeText(copyText.value).then(function () {
        // Optional: Add a feedback mechanism for the user that the link was copied.
        Swal.fire({
            title: "Länken kopierad",
            text: "Du kan nu enkelt dela den via ett sms eller mail genom att klistra in den från minnet",
            icon: "info"
        });
        
    }).catch(function (err) {
        alert("Det gick inte att kopiera länken.");
    });

    //ToggleShowShareLink();

}

function ToggleShowShareLink() {
    var shareLinkContainer = $("#ShareLinkContainer");
    // Toggle the 'visually-hidden' class to show/hide the container
    shareLinkContainer.toggleClass('visually-hidden');
}

//function ToggleShowShareLink() {

//    if ($("#ShareLinkContainer").hasClass('visually-hidden')) {
//        $("#ShareLinkContainer").removeClass('visually-hidden');
//    }
//    else {
//        $("#ShareLinkContainer").addClass('visually-hidden');
//    }

//}