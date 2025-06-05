document.addEventListener("DOMContentLoaded", function () {
    const logoutUrl = "./";
    const whoamiUrl = "http://localhost:5062/api/User/whoami";

    const isLoginPage = window.location.pathname.toLowerCase().includes("./");
    if (isLoginPage) return;

    function checkTokenAndRedirect() {
        const token = sessionStorage.getItem("JwtToken") || localStorage.getItem("jwtToken");
        if (!token) {
            console.warn("JWT token is missing. Redirecting to login.");
            window.location.href = logoutUrl;
        }
    }
    checkTokenAndRedirect();

    const interactionEvents = ["click", "keydown", "mousemove", "touchstart"];
    interactionEvents.forEach(event => {
        window.addEventListener(event, () => {
            checkTokenAndRedirect();
        }, { once: true });
    });

   
    setInterval(checkTokenAndRedirect, 900000);

    window.addEventListener("storage", function (event) {
        if (event.key === "logout-event") {
            window.location.href = logoutUrl;
        }
    });
});