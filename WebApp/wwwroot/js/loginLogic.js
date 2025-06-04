document.addEventListener("DOMContentLoaded", function () {
    const token = sessionStorage.getItem("JwtToken") || localStorage.getItem("jwtToken");
    if (!token) return;

    fetch("http://localhost:5062/api/User/whoami", {
        headers: { "Authorization": "Bearer " + token }
    })
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch user info");
            return response.json();
        })
        .then(data => {
            if (data.role === "Admin") {
                document.getElementById("admin-panel-link").style.display = "block";
            }
        })
        .catch(error => console.error("whoami error:", error));
});