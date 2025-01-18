function setModalValues(element) {
    const selectedDate = element.getAttribute("data-selected-date");
    //const calendarId = element.getAttribute("data-calendar-id");

    document.getElementById("modalSelectedDate").value = selectedDate;
}

$('#eventModal').on('hidden.bs.modal', function () {
    document.getElementById("startDate").value = '';
    document.getElementById("endDate").value = '';
    document.getElementById("eventTitle").value = '';

    document.querySelectorAll('.form-check-input').forEach(checkbox => checkbox.checked = false);

    document.querySelectorAll('.text-danger').forEach(error => error.classList.add('d-none'));

    document.querySelectorAll('.form-control').forEach(input => input.classList.remove('is-invalid'));

    const collapseOne = $('#collapseOne');
    if (collapseOne.hasClass('show')) {
        collapseOne.collapse('hide');
    }
});

function validateForm() {
    const isInterval = isIntervalSelected();
    const areDatesValid = validateDates(isInterval); 
    const isIntervalValid = isInterval ? validateInterval() : true; 
    const areWeekdaysValid = isInterval ? validateWeekdaysWithinInterval() : true;
    const isTitleValid = validateEventTitle();
    const areMembersValid = validateMembers();

    return isIntervalValid && areWeekdaysValid && isTitleValid && areMembersValid && areDatesValid;
}
function isIntervalSelected() {
    const startDateValue = document.getElementById("startDate").value;
    const endDateValue = document.getElementById("endDate").value;
    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked")).map(checkbox => checkbox.value);

    return startDateValue || endDateValue || selectedDays.length > 0;
}
function validateDates(isInterval) {
    const isSelectedDateValid = validateDateField("modalSelectedDate", "error-selected-date");
    const isStartDateValid = validateDateField("startDate", "error-start-date");
    const isEndDateValid = validateDateField("endDate", "error-end-date");
    const isDateRangeValid = isInterval ? validateDateRange("startDate", "endDate", "error-end-date") : true;

    return isSelectedDateValid && isStartDateValid && isEndDateValid && isDateRangeValid;
}
function validateDateField(dateFieldId, errorFieldId) {
    const dateValue = document.getElementById(dateFieldId).value;
    const today = new Date().toISOString().split("T")[0];
    const dateError = document.getElementById(errorFieldId);

    if (!dateValue) {
        dateError.classList.add("d-none");
        document.getElementById(dateFieldId).classList.remove("is-invalid");
        return true;
    }

    if (dateValue < today) {
        dateError.classList.remove("d-none");
        document.getElementById(dateFieldId).classList.add("is-invalid");
        return false;
    }

    dateError.classList.add("d-none");
    document.getElementById(dateFieldId).classList.remove("is-invalid");
    return true;
}
function validateInterval() {
    const startDateValue = document.getElementById("startDate").value;
    const endDateValue = document.getElementById("endDate").value;
    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked")).map(checkbox => checkbox.value);

    const startDateError = document.getElementById("error-start-date");
    const endDateError = document.getElementById("error-end-date");
    const selectedDaysError = document.getElementById("error-interval-weekdays");


    const startDate = document.getElementById("startDate");

    console.log("StartDate:", startDate);


    if (!startDateValue && !endDateValue && selectedDays.length < 2) {
        return true;
    }

    startDateError.classList.add("d-none");
    endDateError.classList.add("d-none");
    selectedDaysError.classList.add("d-none");

    let isValid = true;

    if (!startDateValue) {
        startDateError.textContent = "Startdatum måste vara ifyllt.";
        startDateError.classList.remove("d-none");
        isValid = false;
    } else {
        const today = new Date().toISOString().split("T")[0];
        if (startDateValue < today) {
            startDateError.textContent = "Startdatum kan inte vara tidigare än dagens datum.";
            startDateError.classList.remove("d-none");
            isValid = false;
        }
    }

    if (!endDateValue) {
        endDateError.textContent = "Slutdatum måste vara ifyllt.";
        endDateError.classList.remove("d-none");
        isValid = false;
    } else {
        const today = new Date().toISOString().split("T")[0];
        if (endDateValue < today) {
            endDateError.textContent = "Slutdatum kan inte vara tidigare än dagens datum.";
            endDateError.classList.remove("d-none");
            isValid = false;
        } else if (startDateValue && endDateValue < startDateValue) {
            endDateError.textContent = "Slutdatum kan inte vara tidigare än startdatum.";
            endDateError.classList.remove("d-none");
            isValid = false;
        }
    }
    if (selectedDays.length < 2) {
        selectedDaysError.textContent = "Du måste välja minst en veckodag.";
        selectedDaysError.classList.remove("d-none");
        isValid = false;
    }

    return isValid;
}
function validateWeekdaysWithinInterval() {
    const startDateValue = document.getElementById("startDate").value;
    const endDateValue = document.getElementById("endDate").value;
    const selectedDaysError = document.getElementById("error-right-weekdays");

    if (!startDateValue || !endDateValue) {
        selectedDaysError.classList.add("d-none");
        return true;
    }

    const validWeekdaysSet = new Set(["MÅNDAG", "TISDAG", "ONSDAG", "TORSDAG", "FREDAG", "LÖRDAG", "SÖNDAG"]);
    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked"))
        .map(checkbox => checkbox.value.toUpperCase())
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










