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
    validateInterval();

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
        return true;
    }

    const validWeekdaysSet = new Set(["MÅNDAG", "TISDAG", "ONSDAG", "TORSDAG", "FREDAG", "LÖRDAG", "SÖNDAG"]);

    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked"))
        .map(checkbox => checkbox.value.toUpperCase())  // Konvertera till versaler
        .filter(day => validWeekdaysSet.has(day));  // Filtrera bort ogiltiga dagar

    // Logga valda dagar för felsökning
    console.log("Valda veckodagar:", selectedDays);

    const startDate = new Date(startDateValue);
    const endDate = new Date(endDateValue);

    const validWeekdays = new Set();
    for (let date = new Date(startDate); date <= endDate; date.setDate(date.getDate() + 1)) {
        const weekday = date.toLocaleDateString("sv-SE", { weekday: "long" }).toUpperCase();  // Konvertera till versaler
        validWeekdays.add(weekday);  // Lägg till giltiga veckodagar
    }

    const invalidDays = selectedDays.filter(day => !validWeekdays.has(day));

    // Logga ogiltiga dagar
    console.log("Ogiltiga veckodagar:", invalidDays);

    if (invalidDays.length > 0) {
        weekdayError.textContent = `Följande dagar finns inte i intervallet: ${invalidDays.join(", ")}.`;
        weekdayError.classList.remove("d-none");
        return false;
    }

    weekdayError.classList.add("d-none");
    return true;
}

function validateInterval() {
    const startDateValue = document.getElementById("startDate").value;
    const endDateValue = document.getElementById("endDate").value;
    const selectedDays = Array.from(document.querySelectorAll(".form-check-input:checked")).map(checkbox => checkbox.value);
    console.log(selectedDays)

    const startDateError = document.getElementById("startDateError");
    const endDateError = document.getElementById("endDateError");
    const selectedDaysError = document.getElementById("selectedDaysError");

    // Dölja alla felmeddelanden först
    startDateError.classList.add("d-none");
    endDateError.classList.add("d-none");
    selectedDaysError.classList.add("d-none");

    // Kontrollera att både startDate, endDate och selectedDays är ifyllda
    let isValid = true;

    if (!startDateValue) {
        startDateError.textContent = "Startdatum måste vara ifyllt.";
        startDateError.classList.remove("d-none");
        isValid = false;
    }

    if (!endDateValue) {
        endDateError.textContent = "Slutdatum måste vara ifyllt.";
        endDateError.classList.remove("d-none");
        isValid = false;
    }

    if (selectedDays.length === 1) {
        selectedDaysError.textContent = "Du måste välja minst en veckodag.";
        selectedDaysError.classList.remove("d-none");
        isValid = false;
    }

    // Om något saknas, visa ett allmänt felmeddelande för intervall
    if (startDateValue && endDateValue && selectedDays.length === 1) {
        selectedDaysError.textContent = "Du måste välja veckodagar för att skapa ett intervall.";
        selectedDaysError.classList.remove("d-none");
        isValid = false;
    }

    // Kontrollera att slutdatum inte är tidigare än startdatum
    if (startDateValue && endDateValue && endDateValue < startDateValue) {
        endDateError.textContent = "Slutdatum kan inte vara tidigare än startdatum.";
        endDateError.classList.remove("d-none");
        isValid = false;
    }

    return isValid;
}






