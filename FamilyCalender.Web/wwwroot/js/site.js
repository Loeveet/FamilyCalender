function setModalValues(element) {
    const selectedDate = element.getAttribute("data-selected-date");
    const memberId = element.getAttribute("data-member-id");
    const calendarId = element.getAttribute("data-calendar-id");
    const memberName = element.getAttribute("data-member-name");

    document.getElementById("modalSelectedDate").value = selectedDate;
    document.getElementById("modalMemberId").value = memberId;
    document.getElementById("modalCalenderId").value = calendarId;
    document.getElementById("modalMemberLabel").innerText = memberName;
}
$('#eventModal').on('hidden.bs.modal', function () {
    document.getElementById("startDate").value = '';
    document.getElementById("endDate").value = '';
    document.getElementById("eventTitle").value = '';

    document.querySelectorAll('.form-check-input').forEach(checkbox => checkbox.checked = false);

    document.querySelectorAll('.text-danger').forEach(error => error.classList.add('d-none'));

    document.querySelectorAll('.form-control').forEach(input => input.classList.remove('is-invalid'));

    var collapseOne = $('#collapseOne');
    if (collapseOne.hasClass('show')) {
        collapseOne.collapse('hide');
    }

});

function validateForm() {
    const isTitleValid = validateEventTitle();
    const areMembersValid = validateMembers();
    const areDatesValid = validateDates();
    const areWeekdaysValid = validateWeekdaysWithinInterval();

    return isTitleValid && areMembersValid && areDatesValid && areWeekdaysValid;
}

function validateEventTitle() {
    const eventTitleInput = document.getElementById("eventTitle");
    const eventTitleError = document.getElementById("eventTitleError");
    const eventTitle = eventTitleInput.value.trim();

    if (eventTitle === "") {
        eventTitleError.classList.remove("d-none");
        eventTitleInput.classList.add("is-invalid");
        return false;
    }

    eventTitleError.classList.add("d-none");
    eventTitleInput.classList.remove("is-invalid");
    return true;
}

function validateMembers() {
    const checkboxes = document.querySelectorAll(".member-checkbox");
    const memberError = document.getElementById("memberError");
    let isAnyChecked = false;

    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            isAnyChecked = true;
        }
    });

    if (!isAnyChecked) {
        memberError.classList.remove("d-none");
        return false;
    }

    memberError.classList.add("d-none");
    return true;
}

function validateDateField(dateFieldId, errorFieldId) {
    const dateValue = document.getElementById(dateFieldId).value;
    const today = new Date().toISOString().split("T")[0];
    const dateError = document.getElementById(errorFieldId);

    if (dateValue < today) {
        dateError.classList.remove("d-none");
        document.getElementById(dateFieldId).classList.add("is-invalid");
        return false;
    }

    dateError.classList.add("d-none");
    document.getElementById(dateFieldId).classList.remove("is-invalid");
    return true;
}

function validateDates() {
    const isSelectedDateValid = validateDateField("modalSelectedDate", "selectedDateError");
    const isStartDateValid = validateDateField("startDate", "startDateError");
    const isEndDateValid = validateDateField("endDate", "endDateError");
    const isDateRangeValid = validateDateRange("startDate", "endDate", "endDateBeforeStartDateError");

    return isSelectedDateValid && isStartDateValid && isEndDateValid && isDateRangeValid;
}

function validateDateRange(startDateFieldId, endDateFieldId, errorFieldId) {
    const startDateValue = document.getElementById(startDateFieldId).value;
    const endDateValue = document.getElementById(endDateFieldId).value;
    const dateRangeError = document.getElementById(errorFieldId);

    if (startDateValue && endDateValue && endDateValue < startDateValue) {
        dateRangeError.classList.remove("d-none");
        document.getElementById(endDateFieldId).classList.add("is-invalid");
        return false;
    }

    dateRangeError.classList.add("d-none");
    document.getElementById(endDateFieldId).classList.remove("is-invalid");
    return true;
}

function validateWeekdaysWithinInterval() {
    const startDateValue = document.getElementById("startDate").value;
    const endDateValue = document.getElementById("endDate").value;
    const weekdayError = document.getElementById("weekdayError");

    if (!startDateValue || !endDateValue) {
        weekdayError.classList.add("d-none");
        return true; // Skip validation if dates are not set
    }

    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked"))
    .map(checkbox => checkbox.value.toLowerCase()); // Normalize to lowercase

    const startDate = new Date(startDateValue);
    const endDate = new Date(endDateValue);

    // Generate a set of weekdays within the interval
    // Generate a set of weekdays within the interval
    const validWeekdays = new Set();
    for (let date = new Date(startDate); date <= endDate; date.setDate(date.getDate() + 1)) {
        const weekday = date.toLocaleDateString("sv-SE", { weekday: "long" }); // Get day name in Swedish
        validWeekdays.add(weekday.toLowerCase()); // Normalize to lowercase for easier comparison
    }


    // Check if any selected weekday is not in the valid weekdays
    const invalidDays = selectedDays.filter(day => !validWeekdays.has(day));

    if (invalidDays.length > 0) {
        weekdayError.textContent = `Följande dagar finns inte i intervallet: ${invalidDays.join(", ")}.`;
        weekdayError.classList.remove("d-none");
        return false;
    }

    weekdayError.classList.add("d-none");
    return true;
}





