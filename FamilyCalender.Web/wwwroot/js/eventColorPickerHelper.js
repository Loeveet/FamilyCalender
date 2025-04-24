var eventColorPickerHelper = function () {

    var _hiddenColorInputFieldId = void 0;

    function init(hiddenColorInputFieldId) {
        _hiddenColorInputFieldId = hiddenColorInputFieldId;
    }

    //Color DropDown logic
    function toggleDropdown(event) {
        const dropdown = event.currentTarget;
        const options = dropdown.querySelector('.options');

        // Toggle dropdown visibility
        options.style.display = options.style.display === 'block' ? 'none' : 'block';

        // Add click event to options
        options.querySelectorAll('div').forEach(option => {
            option.onclick = () => {
                const selected = dropdown.querySelector('.selected');
                const hiddenInput = document.getElementById(_hiddenColorInputFieldId);

                selected.innerHTML = option.innerHTML;
                hiddenInput.value = option.dataset.color;

            };
        });

        // Stop event from closing immediately
        event.stopPropagation();
    }


    return {
        Init: init,
        ToggleDropdown: toggleDropdown
    };
}();