document.addEventListener("DOMContentLoaded", function () {
    const vehicleId = window.vehicleId;

    // Load rides on page load and date changes
    async function loadRides() {
        const startTime = document.getElementById("startTimeInput").value;
        const endTime = document.getElementById("endTimeInput").value;

        const rides = await getRides(vehicleId, startTime, endTime);

        const container = document.getElementById("ridesContainer");
        container.innerHTML = "";

        if (!rides || rides.length === 0) {
            container.innerHTML = '<div class="alert alert-info">No rides found for the selected period.</div>';
            return;
        }

        rides.forEach(ride => {
            const rideCard = document.createElement("div");
            rideCard.classList.add("card", "mb-3");

            rideCard.innerHTML = `
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <dl class="row">
                                <dt class="col-sm-4">Start time</dt>
                                <dd class="col-sm-8">${new Date(ride.startTime).toLocaleString()}</dd>

                                <dt class="col-sm-4">End time</dt>
                                <dd class="col-sm-8">${new Date(ride.endTime).toLocaleString()}</dd>
                            </dl>
                        </div>
                        <div class="col-md-6">
                            <dl class="row">
                                <dt class="col-sm-4">Start point</dt>
                                <dd class="col-sm-8">${ride.startGeoPoint.address}<br><small>(${ride.startGeoPoint.x}, ${ride.startGeoPoint.y})</small></dd>

                                <dt class="col-sm-4">End point</dt>
                                <dd class="col-sm-8">${ride.endGeoPoint.address}<br><small>(${ride.endGeoPoint.x}, ${ride.endGeoPoint.y})</small></dd>
                            </dl>
                        </div>
                    </div>
                </div>
            `;

            container.appendChild(rideCard);
        });
    }

    // Handle GPX file upload
    async function uploadGpxFile(event) {
        event.preventDefault();

        const fileInput = document.getElementById("gpxFile");
        const statusDiv = document.getElementById("uploadStatus");

        if (!fileInput.files[0]) {
            statusDiv.innerHTML = '<div class="alert alert-warning"><i class="fas fa-exclamation-triangle me-2"></i>Please select a GPX file.</div>';
            return;
        }

        const formData = new FormData();
        formData.append("file", fileInput.files[0]);

        statusDiv.innerHTML = '<div class="alert alert-info"><i class="fas fa-spinner fa-spin me-2"></i>Uploading track...</div>';

        try {
            const response = await fetch(`/api/vehicles/${vehicleId}/rides`, {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                statusDiv.innerHTML = '<div class="alert alert-success"><i class="fas fa-check-circle me-2"></i>Track uploaded successfully! Ride created.</div>';
                fileInput.value = '';
                // Reload rides list
                loadRides();
            } else if (response.status === 400) {
                statusDiv.innerHTML = '<div class="alert alert-danger"><i class="fas fa-exclamation-circle me-2"></i>Upload failed: Invalid track data or overlaps with existing tracks/rides.</div>';
            } else if (response.status === 403) {
                statusDiv.innerHTML = '<div class="alert alert-danger"><i class="fas fa-lock me-2"></i>Access denied.</div>';
            } else if (response.status === 404) {
                statusDiv.innerHTML = '<div class="alert alert-danger"><i class="fas fa-search me-2"></i>Vehicle not found.</div>';
            } else {
                statusDiv.innerHTML = '<div class="alert alert-danger"><i class="fas fa-times-circle me-2"></i>Upload failed. Please try again.</div>';
            }
        } catch (error) {
            statusDiv.innerHTML = '<div class="alert alert-danger"><i class="fas fa-wifi-slash me-2"></i>Network error. Please try again.</div>';
        }
    }

    // Initialize
    loadRides();

    // Event listeners
    document.getElementById("startTimeInput").addEventListener("change", loadRides);
    document.getElementById("endTimeInput").addEventListener("change", loadRides);
    document.getElementById("uploadForm").addEventListener("submit", uploadGpxFile);
});