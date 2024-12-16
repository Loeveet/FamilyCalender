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
        eventTitleError.classList.remove("d-none"); // Visa felmeddelande
        eventTitleInput.classList.add("is-invalid"); // Markera input som röd
        return false; // Stoppa formuläret från att skickas
    }

    eventTitleError.classList.add("d-none"); // Dölj felmeddelande
    eventTitleInput.classList.remove("is-invalid"); // Ta bort röd markering
    return true; // Tillåt formuläret att skickas
}

function validateMembers() {
    const checkboxes = document.querySelectorAll(".member-checkbox");
    const memberError = document.getElementById("memberError");
    let isAnyChecked = false;

    // Kontrollera om minst en checkbox är markerad
    checkboxes.forEach(checkbox => {
        if (checkbox.checked) {
            isAnyChecked = true;
        }
    });

    if (!isAnyChecked) {
        memberError.classList.remove("d-none"); // Visa felmeddelande
        return false; // Stoppa formuläret från att skickas
    }

    memberError.classList.add("d-none"); // Dölj felmeddelande
    return true; // Tillåt formuläret att skickas
}

function validateForm() {
    const isTitleValid = validateEventTitle();
    const areMembersValid = validateMembers();
    return isTitleValid && areMembersValid; // Stoppar formuläret om något är fel
}




