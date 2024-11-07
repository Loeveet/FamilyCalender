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

