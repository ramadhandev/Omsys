document.addEventListener("DOMContentLoaded", function () {
    const selectAll = document.getElementById("selectAll");
    if (selectAll) {
        selectAll.addEventListener("change", function () {
            const checkboxes = document.querySelectorAll('input[name="selectedIds"]');
            checkboxes.forEach(cb => cb.checked = selectAll.checked);
        });
    }
});

// Versi 2 (langsung di luar)
document.getElementById("selectAll").addEventListener("change", function () {
    var checkboxes = document.querySelectorAll('input[name="selectedIds"]');
    for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = this.checked;
    }
});

function toggleAll(source) {
    let checkboxes = document.querySelectorAll('input[name="selectedIds"]');
    checkboxes.forEach(cb => cb.checked = source.checked);
}

// Auto-submit when symptom filter changes
const symptomFilter = document.querySelector('select[name="symptomName"]');
if (symptomFilter) {
    symptomFilter.addEventListener('change', function () {
        this.form.submit();
    });
}

function toggleAll(source) {
    let checkboxes = document.querySelectorAll('input[name="selectedIds"]');
    checkboxes.forEach(cb => cb.checked = source.checked);
}