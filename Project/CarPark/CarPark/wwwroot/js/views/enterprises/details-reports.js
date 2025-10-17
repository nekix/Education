document.addEventListener("DOMContentLoaded", function () {
    const enterpriseId = window.enterpriseId;
    const enterpriseTimeZone = window.enterpriseTimeZone;
    const userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

    // Report type selection
    const reportTypeSelect = document.getElementById('reportTypeSelect');
    const reportFormsContainer = document.getElementById('reportFormsContainer');
    const reportResultsContainer = document.getElementById('reportResultsContainer');

    // Form elements
    const vehicleMileageForm = document.getElementById('vehicleMileageForm');
    const enterpriseRidesForm = document.getElementById('enterpriseRidesForm');
    const enterpriseModelsForm = document.getElementById('enterpriseModelsForm');

    // Initialize
    reportTypeSelect.addEventListener('change', function() {
        showReportForm(this.value);
    });

    // Show selected report form
    function showReportForm(reportType) {
        // Hide all forms
        const forms = reportFormsContainer.querySelectorAll('.report-form');
        forms.forEach(form => form.style.display = 'none');

        // Hide results
        reportResultsContainer.innerHTML = '';

        // Show selected form
        if (reportType) {
            const selectedForm = document.getElementById(reportType + 'Form');
            if (selectedForm) {
                selectedForm.style.display = 'block';
            }
        }
    }

    // Vehicle Mileage Report
    if (vehicleMileageForm) {
        vehicleMileageForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const formData = new FormData(this);
            const vehicleId = formData.get('vehicleId');
            const startDate = formData.get('startDate');
            const endDate = formData.get('endDate');
            const period = formData.get('period');

            try {
                showLoading();
                const report = await getVehicleMileageReport(vehicleId, startDate, endDate, period);
                displayVehicleMileageReport(report);
            } catch (error) {
                showError('Ошибка загрузки отчета о пробеге транспортного средства');
            } finally {
                hideLoading();
            }
        });
    }

    // Enterprise Rides Report
    if (enterpriseRidesForm) {
        enterpriseRidesForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const formData = new FormData(this);
            const startDate = formData.get('startDate');
            const endDate = formData.get('endDate');
            const period = formData.get('period');

            try {
                showLoading();
                const report = await getEnterpriseRidesReport(enterpriseId, startDate, endDate, period);
                displayEnterpriseRidesReport(report);
            } catch (error) {
                showError('Ошибка загрузки отчета о поездках предприятия');
            } finally {
                hideLoading();
            }
        });
    }

    // Enterprise Models Report
    if (enterpriseModelsForm) {
        enterpriseModelsForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            try {
                showLoading();
                const report = await getEnterpriseModelsReport(enterpriseId);

                console.log(report);

                for (const item of report.dataItems) {
                    const model = await getModel(item.modelId);
                    item.modelName = model.modelName;
                }       

                displayEnterpriseModelsReport(report);
            } catch (error) {
                showError('Ошибка загрузки отчета о моделях предприятия');
            } finally {
                hideLoading();
            }
        });
    }

    function showLoading() {
        reportResultsContainer.innerHTML = `
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Загрузка...</span>
                </div>
                <div class="mt-2">Загрузка отчета...</div>
            </div>
        `;
    }

    function hideLoading() {
        // Loading will be replaced by results
    }

    function showError(message) {
        reportResultsContainer.innerHTML = `
            <div class="alert alert-danger" role="alert">
                <i class="fas fa-exclamation-triangle me-2"></i>${message}
            </div>
        `;
    }

    function displayVehicleMileageReport(report) {
        const periodNames = { 0: 'Day', 1: 'Month', 2: 'Year' };
        const periodName = periodNames[report.period] || report.period;

        let html = `
            <div class="card">
                <div class="card-header">
                    <h5 class="mb-0">Отчет о пробеге транспортного средства</h5>
                    <small class="text-muted">Период: ${periodName}, ${formatDateForDisplay(report.startDate)} - ${formatDateForDisplay(report.endDate)}</small>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Период</th>
                                    <th>Пробег (км)</th>
                                </tr>
                            </thead>
                            <tbody>
        `;

        report.dataItems.forEach(item => {
            const date = formatDateForDisplay(item.date, report.period);
            html += `
                <tr>
                    <td>${date}</td>
                    <td>${item.data.mileageInKm.toFixed(2)}</td>
                </tr>
            `;
        });

        html += `
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;

        reportResultsContainer.innerHTML = html;
    }

    function displayEnterpriseRidesReport(report) {
        const periodNames = { 0: 'Day', 1: 'Month', 2: 'Year' };
        const periodName = periodNames[report.period] || report.period;

        let html = `
            <div class="card">
                <div class="card-header">
                    <h5 class="mb-0">Отчет о поездках предприятия</h5>
                    <small class="text-muted">Период: ${periodName}, ${formatDateForDisplay(report.startDate)} - ${formatDateForDisplay(report.endDate)}</small>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Период</th>
                                    <th>Активных поездок</th>
                                    <th>Общее время</th>
                                    <th>Среднее время</th>
                                    <th>Общий пробег (км)</th>
                                    <th>Средний пробег (км)</th>
                                </tr>
                            </thead>
                            <tbody>
        `;

        report.dataItems.forEach(item => {
            const date = formatDateForDisplay(item.date, report.period);
            html += `
                <tr>
                    <td>${date}</td>
                    <td>${item.data.activeRidesCount}</td>
                    <td>${formatTimeSpan(item.data.totalTime)}</td>
                    <td>${formatTimeSpan(item.data.avgTime)}</td>
                    <td>${item.data.totalMileageKm.toFixed(2)}</td>
                    <td>${item.data.avgMileageKm.toFixed(2)}</td>
                </tr>
            `;
        });

        html += `
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;

        reportResultsContainer.innerHTML = html;
    }

    function displayEnterpriseModelsReport(report) {
        let html = `
            <div class="card">
                <div class="card-header">
                    <h5 class="mb-0">Отчет о моделях транспортных средств предприятия</h5>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Название модели</th>
                                    <th>Количество транспортных средств</th>
                                </tr>
                            </thead>
                            <tbody>
        `;

        report.dataItems.forEach(item => {
            html += `
                <tr>
                    <td>${item.modelName}</td>
                    <td>${item.vehiclesCount}</td>
                </tr>
            `;
        });

        html += `
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;

        reportResultsContainer.innerHTML = html;
    }

    function formatDateForDisplay(dateValue, period) {
        // API returns dates in enterprise timezone or UTC if no timezone set
        // If enterprise timezone differs from user timezone, convert to user timezone
        const date = new Date(dateValue.value || dateValue);

        let convertedDate;
        if (enterpriseTimeZone && enterpriseTimeZone !== userTimeZone) {
            // Convert from enterprise timezone to user timezone
            convertedDate = new Date(date.toLocaleString("en-US", {timeZone: enterpriseTimeZone}));
        } else {
            // Same timezone or no enterprise timezone, show as is
            convertedDate = date;
        }

        // Format based on period type
        const options = { year: 'numeric' };

        switch (period) {
            case 0: // Day
                options.month = '2-digit';
                options.day = '2-digit';
                break;
            case 1: // Month
                options.month = 'long';
                break;
            case 2: // Year
                // Only year
                break;
        }

        return convertedDate.toLocaleDateString('ru-RU', options);
    }

    function formatTimeSpan(timeSpan) {
        // Handle TimeSpan format from .NET (e.g., "01:23:45" or "1.01:23:45")
        if (typeof timeSpan === 'string') {
            // If it's already a formatted string, return as is
            if (timeSpan.includes(':')) {
                return timeSpan;
            }
            // If it's a TimeSpan.ToString() format, parse it
            const parts = timeSpan.split(':');
            if (parts.length >= 3) {
                const hours = parseInt(parts[0]);
                const minutes = parseInt(parts[1]);
                const seconds = parseInt(parts[2]);
                return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
            }
        }
        // Fallback
        return timeSpan || '00:00:00';
    }
});