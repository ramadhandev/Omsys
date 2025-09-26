// wwwroot/js/Custom.js - FIXED VERSION

// Dashboard Specific Functionality
const Dashboard = {
    init: function () {
        console.log('Initializing Dashboard...');
        this.initChart();
        // Hanya panggil function yang ada di halaman dashboard
    },

    initChart: function () {
        const ctx = document.getElementById('chartSymptoms');

        if (!ctx) {
            console.log('Chart element not found on this page');
            return;
        }

        console.log('Chart element found, initializing...');

        // Get data from data attributes
        const labelsJson = ctx.getAttribute('data-labels');
        const dataJson = ctx.getAttribute('data-data');

        if (!labelsJson || !dataJson) {
            this.showNoDataMessage(ctx, 'Data attributes not found');
            return;
        }

        try {
            const labels = JSON.parse(labelsJson);
            const data = JSON.parse(dataJson);

            console.log('Parsed data:', { labels, data });

            if (!labels.length || !data.length) {
                this.showNoDataMessage(ctx, 'Data is empty');
                return;
            }

            this.createChart(ctx, labels, data);
        } catch (error) {
            console.error('Error parsing chart data:', error);
            this.showNoDataMessage(ctx, 'Error parsing data: ' + error.message);
        }
    },

    createChart: function (ctx, labels, data) {
        try {
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Jumlah Gejala per Komponen',
                        data: data,
                        backgroundColor: 'rgba(54, 162, 235, 0.8)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 2,
                        borderRadius: 5
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top'
                        },
                        title: {
                            display: true,
                            text: 'Distribusi Gejala per Komponen'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                stepSize: 1
                            }
                        }
                    }
                }
            });

            console.log('✅ Chart created successfully');
        } catch (error) {
            console.error('❌ Error creating chart:', error);
            this.showNoDataMessage(ctx, 'Chart creation error: ' + error.message);
        }
    },

    showNoDataMessage: function (ctx, reason) {
        console.warn('Showing no data message:', reason);
        ctx.innerHTML = `
            <div class="text-center p-5">
                <div class="text-muted">
                    <i class="fas fa-chart-bar fa-3x mb-3"></i>
                    <p>Tidak ada data gejala untuk ditampilkan</p>
                    <small>${reason}</small>
                </div>
            </div>
        `;
    }
};

// Global Functionality (untuk semua halaman)
const GlobalFunctions = {
    init: function () {
        this.initImageModal();
        this.initCheckboxes();
        this.initSymptomFilter();
        this.initStepNavigation();
    },

    // Image Modal Functionality
    initImageModal: function () {
        const imageModal = document.getElementById('imageModal');
        if (imageModal) {
            imageModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const imgSrc = button.getAttribute('data-img');
                const modalImg = document.getElementById('modalImage');
                if (modalImg && imgSrc) {
                    modalImg.src = imgSrc;
                }
            });
            console.log('Image modal initialized');
        }
    },

    // Checkbox Select All Functionality
    initCheckboxes: function () {
        const selectAll = document.getElementById('selectAll');
        if (selectAll) {
            selectAll.addEventListener('change', function () {
                const checkboxes = document.querySelectorAll('input[name="selectedIds"]');
                checkboxes.forEach(cb => cb.checked = selectAll.checked);
            });
            console.log('Checkboxes initialized');
        }

        // Manual toggle function
        window.toggleAll = function (source) {
            const checkboxes = document.querySelectorAll('input[name="selectedIds"]');
            checkboxes.forEach(cb => cb.checked = source.checked);
        };
    },

    // Auto-submit Symptom Filter
    initSymptomFilter: function () {
        const symptomFilter = document.querySelector('select[name="symptomName"]');
        if (symptomFilter) {
            symptomFilter.addEventListener('change', function () {
                this.form.submit();
            });
            console.log('Symptom filter initialized');
        }
    },

    // Step Navigation Functionality
    initStepNavigation: function () {
        const stepItems = document.querySelectorAll('.step-item');
        const stepDetails = document.querySelectorAll('.step-detail');

        if (stepItems.length === 0 || stepDetails.length === 0) {
            return; // Skip jika tidak ada step navigation di halaman ini
        }

        function showStep(index) {
            stepDetails.forEach((el, i) => {
                el.style.display = i === index ? 'block' : 'none';
            });
            stepItems.forEach(el => el.classList.remove('active'));
            if (stepItems[index]) {
                stepItems[index].classList.add('active');
                stepItems[index].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
        }

        stepItems.forEach(item => {
            item.addEventListener('click', function () {
                const index = parseInt(this.getAttribute('data-step'));
                showStep(index);
            });
        });

        document.querySelectorAll('.prev-step').forEach(btn => {
            btn.addEventListener('click', function () {
                const current = parseInt(this.getAttribute('data-step'));
                if (current > 0) showStep(current - 1);
            });
        });

        document.querySelectorAll('.next-step').forEach(btn => {
            btn.addEventListener('click', function () {
                const current = parseInt(this.getAttribute('data-step'));
                if (current < stepDetails.length - 1) showStep(current + 1);
            });
        });

        // Initialize first step
        showStep(0);
        console.log('Step navigation initialized');
    }
};

// Main initialization function
function initializePage() {
    console.log('Initializing page...');

    // Always initialize global functions
    GlobalFunctions.init();

    // Initialize dashboard only if on dashboard page
    const chartElement = document.getElementById('chartSymptoms');
    if (chartElement) {
        Dashboard.init();
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', initializePage);

// Export for global access if needed
window.Dashboard = Dashboard;
window.GlobalFunctions = GlobalFunctions;