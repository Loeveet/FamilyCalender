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

function validateForm() {
    const isTitleValid = validateEventTitle();
    const areMembersValid = validateMembers();
    return isTitleValid && areMembersValid; 




