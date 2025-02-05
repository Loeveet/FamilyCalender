﻿function setModalValues(element) {
    const selectedDate = element.getAttribute("data-selected-date");

    document.getElementById("modalSelectedDate").value = selectedDate;
}

$('#eventModal').on('hidden.bs.modal', function () {
    document.getElementById("modalStartDate").value = '';
    document.getElementById("modalEndDate").value = '';
    document.getElementById("eventTitle").value = '';

    document.querySelectorAll('.form-check-input').forEach(checkbox => checkbox.checked = false);

    document.querySelectorAll('.text-danger').forEach(error => error.classList.add('d-none'));

    document.querySelectorAll('.form-control').forEach(input => input.classList.remove('is-invalid'));

    const collapseOne = $('#collapseOne');
    if (collapseOne.hasClass('show')) {
        collapseOne.collapse('hide');
    }
});


// #region validations 

function validateForm() {
    resetValidation();

    const isTitleValid = validateEventTitle();

    const areMembersValid = validateMembers();

    const isInterval = isIntervalSelected();
    const areDatesValid = validateDates(isInterval);
    const isIntervalValid = isInterval ? validateInterval() : true;
    const areWeekdaysValid = isInterval ? validateWeekdaysWithinInterval() : true;

    return isTitleValid && areMembersValid && areDatesValid && isIntervalValid && areWeekdaysValid;
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
    const startDateValue = document.getElementById("modalStartDate").value;
    const endDateValue = document.getElementById("modalEndDate").value;
    const selectedDays = Array.from(document.querySelectorAll(".day-checkbox:checked")).map(checkbox => checkbox.value);
    console.log("Selected days:", selectedDays); // ovanför här

    return startDateValue || endDateValue || selectedDays.length > 0;
}
function validateDates(isInterval) {
    let isValid = true;

    if (isInterval) {
        const isStartDateValid = validateDateField('modalStartDate', 'error-start-date');
        const isEndDateValid = validateDateField('modalEndDate', 'error-end-date');
        const isDateRangeValid = validateDateRange('modalStartDate', 'modalEndDate', 'error-end-date');

        isValid = isStartDateValid && isEndDateValid && isDateRangeValid;
    } else {
        isValid = validateDateField('modalSelectedDate', 'error-selected-date');
    }

    return isValid;
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
        dateError.textContent = "Datum kan inte vara bakåt i tiden.";
        dateError.classList.remove("d-none");
        document.getElementById(dateFieldId).classList.add("is-invalid");
        return false;
    }

    dateError.classList.add("d-none");
    document.getElementById(dateFieldId).classList.remove("is-invalid");
    return true;
}
function validateInterval() {
    const startDate = document.getElementById("modalStartDate");
    const endDate = document.getElementById("modalEndDate");
    const selectedDays = document.querySelectorAll('.form-check-input:checked');

    const startDateValue = startDate.value;
    const endDateValue = endDate.value;
    const today = new Date().toISOString().split("T")[0];

    const startDateError = document.getElementById("error-start-date");
    const endDateError = document.getElementById("error-end-date");
    const selectedDaysError = document.getElementById('error-interval-weekdays');

    let isValid = true;


    if (!startDateValue) {
        startDateError.textContent = "Startdatum krävs.";
        startDateError.classList.remove("d-none");
        isValid = false;
    } else if (startDateValue < today) {
        startDateError.textContent = "Startdatum kan inte vara bakåt i tiden.";
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
    const startDateValue = document.getElementById("modalStartDate").value;
    const endDateValue = document.getElementById("modalEndDate").value;
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
        eventTitleError.textContent = 'Ange titel';
        eventTitleError.classList.remove("d-none");
        eventTitleInput.classList.add("is-invalid");
        return false;
    }

    return true;
}
function validateMembers() {
    const checkboxes = document.querySelectorAll(".member-checkbox"); //TTT
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
