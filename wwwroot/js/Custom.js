document.addEventListener("DOMContentLoaded", function () {
    var imageModal = document.getElementById('imageModal');

    imageModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget; // img yang diklik
        var imgSrc = button.getAttribute('data-img');
        var modalImg = document.getElementById('modalImage');
        modalImg.src = imgSrc;
    // Toggle semua checkbox
    const selectAll = document.getElementById("selectAll");
    if (selectAll) {
        selectAll.addEventListener("change", function () {
            const checkboxes = document.querySelectorAll('input[name="selectedIds"]');
            checkboxes.forEach(cb => cb.checked = selectAll.checked);
        });
    }

    function toggleAll(source) {
        let checkboxes = document.querySelectorAll('input[name="selectedIds"]');
        checkboxes.forEach(cb => cb.checked = source.checked);
    }

    // Auto-submit ketika filter symptom berubah
    const symptomFilter = document.querySelector('select[name="symptomName"]');
    if (symptomFilter) {
        symptomFilter.addEventListener('change', function () {
            this.form.submit();
        });
    }

    // Step navigation
    const stepItems = document.querySelectorAll(".step-item");
    const stepDetails = document.querySelectorAll(".step-detail");

    function showStep(index) {
        stepDetails.forEach((el, i) => {
            el.style.display = i === index ? "block" : "none";
        });
        stepItems.forEach(el => el.classList.remove("active"));
        stepItems[index].classList.add("active");
        stepItems[index].scrollIntoView({ behavior: "smooth", block: "nearest" });
    }

    stepItems.forEach(item => {
        item.addEventListener("click", function () {
            const index = parseInt(this.getAttribute("data-step"));
            showStep(index);
        });
    });

    document.querySelectorAll(".prev-step").forEach(btn => {
        btn.addEventListener("click", function () {
            const current = parseInt(this.getAttribute("data-step"));
            if (current > 0) showStep(current - 1);
        });
    });

    document.querySelectorAll(".next-step").forEach(btn => {
        btn.addEventListener("click", function () {
            const current = parseInt(this.getAttribute("data-step"));
            if (current < stepDetails.length - 1) showStep(current + 1);
        });
    });
    });
});
