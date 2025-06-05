document.addEventListener("DOMContentLoaded", function () {
    const logsLink = document.getElementById("platform-logs-link");
    if (!logsLink) return;

    const token = sessionStorage.getItem("JwtToken") || localStorage.getItem("jwtToken");

    if (!token) {
        console.warn("No JWT token found. Redirecting to home.");
        window.location.href = "./";
        return;
    }

    fetch("http://localhost:5062/api/User/whoami", {
        headers: { "Authorization": "Bearer " + token }
    })
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch user info");
            return response.json();
        })
        .then(data => {
            console.log("User data from whoami:", data);

            if (data.role === "Admin") {
                logsLink.style.display = "block";
            } else {
                logsLink.style.display = "none";
            }
        })
        .catch(error => console.error("whoami error:", error));
});
